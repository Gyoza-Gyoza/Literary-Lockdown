using TMPro;
using UnityEngine;

public class ShopItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI cost;

    public void Initialize(ShopItemData shopItemData)
    {
        this.title.text = shopItemData.Title;
        this.description.text = shopItemData.Description;
        this.cost.text = shopItemData.Cost.ToString();
    }
}