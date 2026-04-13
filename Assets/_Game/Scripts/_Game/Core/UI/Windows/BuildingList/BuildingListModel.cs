using Core.UI.CoreMVP;
using Game.Buildings.Data;
using UnityEngine;

namespace Core.UI.Windows.BuildingList
{
    public class BuildingListModel : Model
    {
        public BuildingData[] BuildingDataList { get; }
        public Vector2Int SelectedCell { get; }
        
        public BuildingListModel(BuildingData[] buildings, Vector2Int selectedCell)
        {
            BuildingDataList = buildings;
            SelectedCell = selectedCell;
        }
        
        public override void Dispose()
        {
            
        }
    }
}