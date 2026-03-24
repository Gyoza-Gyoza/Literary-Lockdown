using UnityEngine;

[System.Serializable]
public struct ShopItemData
{
    private string title;
    private string description;
    private int cost;

    public string Title => title;
    public string Description => description;
    public int Cost => cost;

    public ShopItemData(string title, string description, int cost)
    {
        this.title = title;
        this.description = description;
        this.cost = cost;
    }
}
