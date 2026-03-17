using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class PlayerMetadata
{
    public string playerName = "";
    public int pagesHeld = 0;
    public Dictionary<string, int> levels =  new Dictionary<string, int>()
    {
        { "Rapunzel",1 },
        { "Wolf",1 }
    };
}
