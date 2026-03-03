using TMPro;
using UnityEngine;

public class UpdatePages : MonoBehaviour
{
    private TextMeshProUGUI text;
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        text.text = SaveLoadManager.PlayerData.pagesHeld.ToString();
    }
}
