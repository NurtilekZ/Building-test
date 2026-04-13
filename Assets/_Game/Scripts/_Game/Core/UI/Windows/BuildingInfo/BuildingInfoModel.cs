using Core.UI.CoreMVP;
using Game.Buildings.Core;

namespace Core.UI.Windows.BuildingInfo
{
    public class BuildingInfoModel : Model
    {
        public Building Building { get; }

        public BuildingInfoModel(Building building)
        {
            Building = building;
        }
    }
}