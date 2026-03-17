using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class CompanionUpgradeManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI nextLevelText;
    [SerializeField] private Image spriteUI;
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI damagePerLevelText;
    [SerializeField] private TextMeshProUGUI attackSpeedText;
    [SerializeField] private TextMeshProUGUI attackSpeedPerLevelText;
    [SerializeField] private TextMeshProUGUI costText;
    
    [SerializeField] private RectTransform[] layoutGroups;
        
    private readonly Dictionary<string, TowerData> towerData = new();

    private TowerData currentTowerDisplayed;

    // public int CurrentPage
    // {
    //     get { return currentPage; }
    //     set
    //     {
    //         if (value < 0) currentPage = towerData.Count - 1;
    //         else if (value >= towerData.Count) currentPage = 0;
    //         else currentPage = value;
    //         SwitchPage();
    //     }
    // }

    private void Start()
    {
        foreach (var data in Database.Instance.database["Towers"])
        {
            towerData.Add(data.Key, (TowerData)data.Value);
        }
        SwitchPage("Rapunzel");
    }
    public void SwitchPage(string name)
    {
        currentTowerDisplayed = towerData[name];
        UpdateUI();
    }
    public void Upgrade()
    {
        SaveLoadManager.PlayerData.pagesHeld -= int.Parse(costText.text);
        SaveLoadManager.PlayerData.levels[currentTowerDisplayed.Name]++;
        SaveLoadManager.SaveData();
        UpdateUI();
    }

    private void UpdateUI()
    {
        PlayerMetadata playerMetadata = SaveLoadManager.LoadData();

        string towerName = currentTowerDisplayed.Name;
        nameText.text = towerName;
        descriptionText.text = currentTowerDisplayed.Description;
        levelText.text = playerMetadata.levels[towerName].ToString();
        nextLevelText.text = (playerMetadata.levels[towerName] + 1).ToString();
        spriteUI.sprite = currentTowerDisplayed.Sprite;
        damageText.text = (currentTowerDisplayed.Damage + currentTowerDisplayed.DamagePerLevel * playerMetadata.levels[towerName]).ToString();
        damagePerLevelText.text = $"+{currentTowerDisplayed.DamagePerLevel.ToString()}";
        attackSpeedText.text = (currentTowerDisplayed.AttackSpeed + currentTowerDisplayed.AttackSpeedPerLevel * playerMetadata.levels[towerName]).ToString();
        attackSpeedPerLevelText.text = $"+{currentTowerDisplayed.AttackSpeedPerLevel.ToString()}";
        costText.text = currentTowerDisplayed.UpgradeCost.ToString();

        for (int i = 0; i < layoutGroups.Length; i++)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroups[i]);
        }
    }
}
[System.Serializable]
public class SerializableTowerData
{
    public Sprite icon;
    public float damage, attackSpeed;
    public int cost;
}