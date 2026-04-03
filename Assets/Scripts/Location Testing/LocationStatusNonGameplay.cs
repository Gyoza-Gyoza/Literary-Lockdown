using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.UI;

//[System.Serializable]
public class LocationStatusNonGameplay : MonoBehaviour
{
    public TMP_Text locText;
    public Image validMarker;
    public Sprite validSprite;
    public Sprite invalidSprite;

    private void Start()
    {
        // Start updating location
        //StartCoroutine(StartTimeOut());

        //CloseLocPopUp();
    }

    private void FixedUpdate()
    { 
        if (Input.location.isEnabledByUser || DebugMode.Instance.debugMode)
        {
            if (LocationManager.Instance.isLocationValid)
            {
                locText.text = $"{LocationManager.Instance.Closest.Name}";
                validMarker.sprite = validSprite;
            }
            else
            {
                locText.text = $"OUT OF RANGE\nPlease Proceed to {LocationManager.Instance.Closest.Name}";
                validMarker.sprite = invalidSprite;
            }
        }
        else
        {
            locText.text = " Location not enabled"; 
            validMarker.sprite = invalidSprite;
        }
        
        // if (LocationManager.Instance.isUpdatingLoc)
        // {
        //     if (LocationManager.Instance.isLocationValid)
        //     {
        //         //nearestHeaderText.text = "Currently In:";
        //         //nearestLocText.text = $"{LocationManager.Instance.closest.name}";
        //         locText.text = $"{LocationManager.Instance.Closest.Name}";
        //     }
        //     else
        //     {
        //         //nearestHeaderText.text = "Nearest Location Is:";
        //         //nearestLocText.text = $"{LocationManager.Instance.closest.name}";
        //         locText.text = $"Please Proceed to {LocationManager.Instance.Closest.Name}";
        //         validMarker.sprite = invalidSprite;
        //     }
        // }
        //
        // // Check if location service is enabled
        // else if (!Input.location.isEnabledByUser)
        // {
        //     //nearestHeaderText.text = "Error";
        //     //nearestLocText.text = "Location not enabled";
        //     locText.text = "Location not enabled";
        //     validMarker.sprite = invalidSprite;
        //
        //     return;
        // }
        // else
        // {
        //     locText.text = $"{LocationManager.Instance.Closest.Name}";
        //     validMarker.sprite = validSprite;
        // }
    }

    public void LeaveLocation()
    {
        LocationManager.Instance.SetLocation(new double2(0,0));
    }
}