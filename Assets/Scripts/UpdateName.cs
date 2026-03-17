using System;
using TMPro;
using UnityEngine;

public class UpdateName : MonoBehaviour
{
    private TextMeshProUGUI text;
    public void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        text.text = SaveLoadManager.PlayerData.playerName.ToString();
    }
}
