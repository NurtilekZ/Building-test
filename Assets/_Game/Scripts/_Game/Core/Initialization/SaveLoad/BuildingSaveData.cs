using System;

namespace Core.Initialization.SaveLoad
{
    [Serializable]
    public class BuildingSaveData
    {
        public string Id;
        public string DisplayName;
        public int CellX;
        public int CellY;
        public int RotationSteps;
        public int Level;
    }
}
