using System;
using TMPro;
using UnityEngine;

public class UpdatePages : MonoBehaviour
{
    private TextMeshProUGUI text;
    public void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void Update()
    {
        text.text = SaveLoadManager.PlayerData.pagesHeld.ToString();
    }
}
