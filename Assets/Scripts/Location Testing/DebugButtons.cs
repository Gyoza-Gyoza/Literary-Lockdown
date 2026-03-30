using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.UI;

//[System.Serializable]
public class DebugButtons : MonoBehaviour
{
    public void TriggerToggleDebugMode()
    {
        DebugMode.Instance.ToggleDebugMode();
    }

    public void TriggerToggleLocationMode()
    {
        DebugMode.Instance.ToggleMapWindow();
    }

}