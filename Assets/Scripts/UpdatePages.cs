using TMPro;
using UnityEngine;

public class UpdatePages : MonoBehaviour
{
    private TextMeshProUGUI text;
    public void Update()
    {
        text = GetComponent<TextMeshProUGUI>();
        text.text = SaveLoadManager.PlayerData.pagesHeld.ToString();
    }
}
