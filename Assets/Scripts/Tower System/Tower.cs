using UnityEngine;
using System.Collections.Generic;
public abstract class Tower 
{
    private string name;
    private Stats baseStats;
    private List<Stats> bonusStats = new();
    public Stats BaseStats => baseStats;
    public Stats BonusStats
    {
        get
        {
            Stats totalBonus = new();
            foreach (Stats stats in bonusStats) totalBonus += stats;
            return totalBonus;
        }
    }
    public Stats TotalStats
    {
        get
        {
            Stats finalStats = baseStats;
            foreach (Stats stats in bonusStats) baseStats += stats;
            return finalStats;
        }
    }
}
