using TMPro;
using UnityEngine;

public class UpdateName : MonoBehaviour
{
    private TextMeshProUGUI text;
    public void Update()
    {
        text = GetComponent<TextMeshProUGUI>();
        // text.text = SaveLoadManager.PlayerData.playerName.ToString();
    }
}
