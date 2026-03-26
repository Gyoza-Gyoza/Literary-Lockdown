using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.UI;
using static Unity.Netcode.NetworkSceneManager;
using static UnityEngine.GraphicsBuffer;

[System.Serializable]
public class LocationPopUp : MonoBehaviour
{
    //public TMP_Text locText;
    //public Image validMarker;
    //public Sprite validSprite;
    //public Sprite invalidSprite;


    public RawImage mapImageComp;
    public TMP_Text nearestHeaderText;
    public TMP_Text nearestLocText;

    public float zoom = 10;
    private float width = 0f;
    private float height = 0f;

    public float forceLocControl_Speed = 100f;

    //public static LocationManager Instance;

    public int pointsInCircle = 8;
    public Color32 validLocCol = new Color32(); 
    public Color32 validRadCol = new Color32();

    public CanvasGroup popUpGroup;

    public void OpenLocPopUp()
    {
        //Debug.Log("Pop up locatiuonc called");
        popUpGroup.alpha = 1.0f;
        popUpGroup.interactable = true;
        popUpGroup.blocksRaycasts = true;
    }


    public void CloseLocPopUp()
    {
        popUpGroup.alpha = 0f;
        popUpGroup.interactable = false;
        popUpGroup.blocksRaycasts = false;
    }

    private void Start()
    {
        width = (int)Mathf.Round(mapImageComp.rectTransform.rect.width);
        if (width > 512) {width = 512;}
        height = (int)Mathf.Round(mapImageComp.rectTransform.rect.height);
        if (height > 512) { height = 512; }

        // Start updating location
        //StartCoroutine(StartTimeOut());

        //CloseLocPopUp();
    }

    private void FixedUpdate()
    {
        if (LocationManager.Instance.isForceLoc)
        {
            //Check input and adjust location based on input

            LocationManager.Instance.ForceEditLon(Time.fixedDeltaTime * Joystick.current.stick.x.value * forceLocControl_Speed);
            LocationManager.Instance.ForceEditLat(Time.fixedDeltaTime * Joystick.current.stick.y.value * forceLocControl_Speed);


            //Debug.Log("Current stick value: " + Joystick.current.stick.x.value + ", " + Joystick.current.stick.y.value);
            //Debug.Log("Lon calc: " + (Time.fixedDeltaTime * Joystick.current.stick.x.value * forceLocControl_Speed));
            //Debug.Log("Lat calc: " + (Time.fixedDeltaTime * Joystick.current.stick.y.value * forceLocControl_Speed));
        }


        if(LocationManager.Instance.isUpdatingLoc)
        {
            if (!mapIsLoading)
            {
                StartCoroutine(GetOneMap());
            }

            if (LocationManager.Instance.isLocationValid)
            {
                nearestHeaderText.text = "Currently In:";
                nearestLocText.text = $"{LocationManager.Instance.closest.name}";
                //locText.text = $"{LocationManager.Instance.closest.name}";
                //validMarker.sprite = validSprite;
            }
            else
            {
                nearestHeaderText.text = "Nearest Location Is:";
                nearestLocText.text = $"{LocationManager.Instance.closest.name}";
                //locText.text = $"Please Proceed to {closest.name}";
                //validMarker.sprite = invalidSprite;
            }
        }

        // Check if location service is enabled
        else if (!Input.location.isEnabledByUser)
        {
            nearestHeaderText.text = "Error";
            nearestLocText.text = "Location not enabled";
            //locText.text = "Location not enabled";
            //validMarker.sprite = invalidSprite;

           return;
        }
    }

    public void ToggleForceLocButton()
    {
        LocationManager.Instance.ToggleForceLoc();
        //forceLoc = !forceLoc;
    }

    private string url = "";
    private bool mapIsLoading = false;
    private bool updateMap = false;

    IEnumerator GetOneMap()
    {
        Debug.Log("Starting On Map");
        TargetLocation closest = LocationManager.Instance.closest;
        url = "https://www.onemap.gov.sg/api/staticmap/getStaticImage?layerchosen=default&zoom=" + zoom + "&height=" + height + "&width=" + width + "&lat=" + LocationManager.Instance.currentLat + "&lng=" + LocationManager.Instance.currentLon + "&points=%5B" + closest.latitude + "%2C%20" + closest.longitude + "%2C%20%22" + validLocCol.r + "%2C%20" + validLocCol.g + "%2C%20"+ validLocCol.b + "%22%5D";


       /* string polygonURL*/  url += "&polygons=" + GenerateCirclePointsASCIIString(closest.latitude, closest.longitude, closest.radiusMeters, pointsInCircle) + "%3A" + validRadCol.r + "%2C" + validRadCol.g + "%2C" + validRadCol.b;
        Debug.Log("url formed, " + url);

        //Debug.Log("Polygon section: " + polygonURL);

        mapIsLoading = true;
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);

        Debug.Log("Startied Yield");

        yield return www.SendWebRequest();

        Debug.Log("yield failed");

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("WWW ERROR: " + www.error);
        }
        else
        {
            Debug.Log("result not success");
            mapIsLoading = false;
            mapImageComp.texture = ((DownloadHandlerTexture)www.downloadHandler).texture;

            //apiKeyLast = apiKey;
            //latLast = lat;
            //lonLast = lon;
            //zoomLast = zoom;
            //mapResolutionLast = mapResolution;
            //mapTypeLast = mapType;
            updateMap = true;
        }
    }
    //########## Utils #######################

    public static double[,] GenerateCirclePoints(double centerLat, double centerLon, double radiusMeters, int numPoints)
    {
        double[,] points = new double[numPoints, 2];

        // Convert to radians
        double lat1 = DegreesToRadians(centerLat);
        double lon1 = DegreesToRadians(centerLon);
        double angularDistance = radiusMeters / 6371000f;

        for (int i = 0; i < numPoints; i++)
        {
            double tetha = 2.0 * Mathf.PI * i / numPoints; // evenly spaced

            double sinlat1 = Mathf.Sin((float)lat1);
            double coslat1 = Mathf.Cos((float)lat1);
            double sinAd = Mathf.Sin((float)angularDistance);
            double cosAd = Mathf.Cos((float)angularDistance);

            double lat2 = Mathf.Asin((float)(
                sinlat1 * cosAd +
                coslat1 * sinAd * Mathf.Cos((float)tetha))
            );

            double lon2 = lon1 + Mathf.Atan2(
                (float)(Mathf.Sin((float)tetha) * sinAd * coslat1),
                (float)(cosAd - sinlat1 * Mathf.Sin((float)lat2))
            );

            // Convert back to degrees
            points[i, 0] = RadiansToDegrees(lat2); // latitude
            points[i, 1] = RadiansToDegrees(lon2); // longitude
        }

        return points;
    }

    public static string GenerateCirclePointsASCIIString(double centerLat, double centerLon, double radiusMeters, int numPoints)
    {
        string toReturn = "%5B";

        double[,] points = new double[numPoints, 2];

        // Convert to radians
        double lat1 = DegreesToRadians(centerLat);
        double lon1 = DegreesToRadians(centerLon);
        double angularDistance = radiusMeters / 6371000f;

        string first = "";

        for (int i = 0; i < numPoints; i++)
        {
            if (i != 0)
            {
                toReturn += "%2C";
            }
            double tetha = 2.0 * Mathf.PI * i / numPoints; // evenly spaced

            double sinlat1 = Mathf.Sin((float)lat1);
            double coslat1 = Mathf.Cos((float)lat1);
            double sinAd = Mathf.Sin((float)angularDistance);
            double cosAd = Mathf.Cos((float)angularDistance);

            double lat2 = Mathf.Asin((float)(
                sinlat1 * cosAd +
                coslat1 * sinAd * Mathf.Cos((float)tetha))
            );

            double lon2 = lon1 + Mathf.Atan2(
                (float)(Mathf.Sin((float)tetha) * sinAd * coslat1),
                (float)(cosAd - sinlat1 * Mathf.Sin((float)lat2))
            );

            // Convert back to degrees
            points[i, 0] = RadiansToDegrees(lat2); // latitude
            points[i, 1] = RadiansToDegrees(lon2); // longitude

            if(i == 0)
            {
                first = "%5B" + RadiansToDegrees(lat2) + "%2C" + RadiansToDegrees(lon2) + "%5D";
            }

            toReturn += "%5B" + RadiansToDegrees(lat2) + "%2C" + RadiansToDegrees(lon2) + "%5D";
        }
        toReturn += "%2C";
        toReturn += first;
        toReturn += "%5D";
        return toReturn;
    }

    static double DegreesToRadians(double deg)
    {
        return deg * Mathf.PI / 180.0;
    }

    static double RadiansToDegrees(double rad)
    {
        return rad * 180.0 / Mathf.PI;
    }
}