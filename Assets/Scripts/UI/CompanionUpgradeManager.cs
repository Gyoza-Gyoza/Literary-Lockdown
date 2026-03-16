using System;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class CompanionUpgradeManager : MonoBehaviour
{
    [SerializeField] private Image spriteUI;
    [SerializeField] private TextMeshProUGUI costText, damageText, attackSpeedText;
    [SerializeField] private SerializableTowerData[] towerData;

    private int currentPage;

    public int CurrentPage
    {
        get { return currentPage; }
        set
        {
            if (value < 0) currentPage = towerData.Length - 1;
            else if (value >= towerData.Length) currentPage = 0;
            else currentPage = value;
            SwitchPage();
        }
    }

    private void Start()
    {
        CurrentPage = 0;
    }

    public void NextPage() => CurrentPage++;
    public void PrevPage() => CurrentPage--;
    private void SwitchPage()
    {
        SerializableTowerData tower =  towerData[CurrentPage];
        spriteUI.sprite = tower.icon;
        damageText.text = tower.damage.ToString();
        attackSpeedText.text = tower.attackSpeed.ToString();
        costText.text = tower.cost.ToString();
    }

    public void Upgrade()
    {
        damageText.text = (int.Parse(damageText.text) + 1).ToString();
        attackSpeedText.text = (int.Parse(attackSpeedText.text) + 1).ToString();
        SaveLoadManager.PlayerData.pagesHeld -= int.Parse(costText.text);
        SaveLoadManager.SaveData();
        costText.text = (int.Parse(costText.text) + 200).ToString();
    }
}
[System.Serializable]
public class SerializableTowerData
{
    public Sprite icon;
    public float damage, attackSpeed;
    public int cost;
}