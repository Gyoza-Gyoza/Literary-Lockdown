using System.Collections;
using System.Collections.Generic;
using TMPro;
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
        if(LocationManager.Instance.isUpdatingLoc)
        {
            if (LocationManager.Instance.isLocationValid)
            {
                //nearestHeaderText.text = "Currently In:";
                //nearestLocText.text = $"{LocationManager.Instance.closest.name}";
                locText.text = $"{LocationManager.Instance.closest.name}";
                validMarker.sprite = validSprite;
            }
            else
            {
                //nearestHeaderText.text = "Nearest Location Is:";
                //nearestLocText.text = $"{LocationManager.Instance.closest.name}";
                locText.text = $"Please Proceed to {LocationManager.Instance.closest.name}";
                validMarker.sprite = invalidSprite;
            }
        }

        // Check if location service is enabled
        else if (!Input.location.isEnabledByUser)
        {
            //nearestHeaderText.text = "Error";
            //nearestLocText.text = "Location not enabled";
            locText.text = "Location not enabled";
            validMarker.sprite = invalidSprite;

           return;
        }
    }

}