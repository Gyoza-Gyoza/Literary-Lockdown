using UnityEngine;
using System.Collections.Generic;

public class TowerManager : MonoBehaviour
{
    [SerializeField] private GameObject baseTowerObject;
    [SerializeField] private int maxTowers; 
    private List<Tower> towerList = new();

    private void Start()
    {

    }
}
