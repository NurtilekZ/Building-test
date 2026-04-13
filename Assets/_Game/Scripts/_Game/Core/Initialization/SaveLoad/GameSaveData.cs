using System;
using System.Collections.Generic;

namespace Core.Initialization.SaveLoad
{
    [Serializable]
    public class GameSaveData
    {
        public List<BuildingSaveData> Buildings = new();
    }
}
