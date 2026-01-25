using UnityEngine;
using System.Collections.Generic;

public abstract class Tower
{
    private string name;
    private Stats baseStats;
    private List<Stats> bonusStats = new();
    public Stats Stats
    {
        get
        {
            Stats finalStats = baseStats;
            foreach (Stats stats in bonusStats) baseStats += stats;
            return finalStats;
        }
    }
}
