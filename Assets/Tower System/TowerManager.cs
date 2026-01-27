using UnityEngine;
using System.Collections.Generic;

public class TowerManager : MonoBehaviour
{
    [SerializeField] private GameObject towerTemplate;
    [SerializeField] private int maxTowers; 
    private List<Tower> towerList = new();

    private void Start()
    {

    }
    public void CreateTower(string name)
    {
        
    }
}
