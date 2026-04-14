using System;
using System.Collections.Generic;
using System.Linq;
using Core.Initialization.SaveLoad;
using Core.UI.CoreMVP;
using Core.UI.CoreMVP.Overlay;
using Core.UI.Overlays.BuildingActions;
using Core.UI.Windows.BuildingInfo;
using Core.UI.Windows.BuildingList;
using Core.UI.Windows.BuildMode;
using Core.UI.Windows.HUD;
using Core.UI.Windows.Map;
using Game.Assets;
using Game.Buildings.Core;
using Game.Movement;
using UnityEngine;

namespace Core.UI.Windows
{
    [DisallowMultipleComponent]
    public class UIWindowManager : MonoBehaviour
    {
        [Serializable]
        public struct WindowEntry
        {
            public WindowID Id;
            public GameObject Prefab;
        }

        [SerializeField] private RectTransform _windowRoot;
        [SerializeField] private WindowDataListOS _windows;
        [SerializeField] private Move _move;
        [SerializeField] private Rotate _rotate;

        private readonly Dictionary<WindowID, GameObject> _windowPrefabs = new();
        
        private readonly Dictionary<WindowID, IController> _cachedWindows = new();

        private void Awake()
        {
            BuildWindowMap();
        }
        
        public void OpenOverlay(WindowID id, Model model, Transform targetTransform)
        {
            IController overlay = OpenWindow(id, model);
            if (overlay != null)
                ((IOverlayController)overlay).BindTransform(targetTransform);
        }

        public IController OpenWindow(WindowID id, Model model, bool disableControls = true, bool closeOthers = false)
        {
            if (closeOthers)
            {
                CloseAll();
            }

            if (_cachedWindows.TryGetValue(id, out IController existingWindow))
            {
                existingWindow.Bind(model);
                existingWindow.Open();
                
                EnableControls(!disableControls);

                return existingWindow;
            }

            IController controller = id switch
            {
                // Window
                WindowID.HUD => new HUDController(),
                WindowID.BuildingsList => new BuildingListController(),
                WindowID.BuildingMode => new BuildModeController(),
                WindowID.BuildingInfo => new BuildingInfoController(),
                WindowID.Map => new MapController(),
                
                // Overlay
                WindowID.ActionsOverlay => new ActionsOverlayController(),
                _ => throw new ArgumentOutOfRangeException(nameof(controller))
            };

            if (!_windowPrefabs.TryGetValue(id, out GameObject prefab))
            {
                return null;
            }

            Transform parent = !_windowRoot ? transform : _windowRoot;
            IView instanceView = Instantiate(prefab, parent).GetComponent<IView>();
            controller.Bind(model, instanceView);
            controller.Open();
            _cachedWindows[id] = controller;
            
            EnableControls(!disableControls);

            return controller;
        }

        private void EnableControls(bool enable)
        {
            _move.enabled = enable;
            _rotate.enabled = enable;
        }

        public bool CloseWindow(WindowID id)
        {
            if (_cachedWindows.TryGetValue(id, out IController window) && window.IsOpen)
            {
                window.Close();
                EnableControls(true);
                return true;
            }

            return false;
        }

        public void CloseAll()
        {
            foreach (var pair in _cachedWindows.Where(pair => pair.Value.IsOpen))
            {
                pair.Value.Close();
            }
            
            EnableControls(true);
        }

        private void BuildWindowMap()
        {
            _windowPrefabs.Clear();
            foreach (var entry in _windows.windowEntries)
            {
                if (!entry.Prefab)
                {
                    continue;
                }

                _windowPrefabs[entry.Id] = entry.Prefab;
            }
        }
    }
}
