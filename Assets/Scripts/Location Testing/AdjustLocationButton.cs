using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.UI;

//[System.Serializable]
public class AdjustLocationButton : MonoBehaviour
{
    public void OpenDebugModeLocation()
    {
        DebugMode.Instance.ToggleStraightIntoMapWindow();
    }

}