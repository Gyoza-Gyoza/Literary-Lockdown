using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class PlayerMetadata
{
    public string playerName = "";
    public int pagesHeld = 0;
    public List<TowerLevelData> levels = new List<TowerLevelData>()
    {
        new TowerLevelData("Rapunzel"),
        new TowerLevelData("Wolf"),
        new TowerLevelData("Frog Prince")
    };

    public TowerLevelData GetLevelData(string name)
    {
        foreach (TowerLevelData level in levels)
        {
            if (level.name == name) return level;
        }

        return null;
    }
}

[System.Serializable]
public class TowerLevelData
{
    public string name; 
    public int level;
    public TowerLevelData(string name)
    {
        this.name = name;
        level = 1;
    }
}