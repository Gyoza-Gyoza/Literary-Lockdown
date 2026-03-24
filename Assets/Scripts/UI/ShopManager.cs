using System;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private GameObject shopPrefab;
    private void Start()
    {
        InitializeShopItems();
    }

    private void InitializeShopItems()
    {
        foreach (var shopItemData in Database.Instance.database["ShopItems"])
        {
            ShopItem shopItem = Instantiate(shopPrefab, transform).GetComponent<ShopItem>();
            shopItem.Initialize((ShopItemData)shopItemData.Value);
        }
    }
}
