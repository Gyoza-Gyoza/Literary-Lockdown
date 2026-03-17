using TMPro;
using UnityEngine;

public class UpdatePages : MonoBehaviour
{
    private TextMeshProUGUI text;
    public void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        text.text = SaveLoadManager.PlayerData.pagesHeld.ToString();
    }
}
