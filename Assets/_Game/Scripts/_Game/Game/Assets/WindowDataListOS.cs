using Core.UI.Windows;
using UnityEngine;

namespace Game.Assets
{
    [CreateAssetMenu(fileName = "WindowDataList", menuName = "_GameCustomSO/WindowDataList")]

    public class WindowDataListOS : ScriptableObject
    {
        public UIWindowManager.WindowEntry[] windowEntries;
        public UIWindowManager.WindowEntry[] overlayEntries;
    }
}