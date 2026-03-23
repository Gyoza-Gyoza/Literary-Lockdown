using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.UI;
using static Unity.Netcode.NetworkSceneManager;

[System.Serializable]
public class TargetLocation
{
    public string name;
    public float latitude;
    public float longitude;
    public float radiusMeters;
}

public class LocationManager : MonoBehaviour
{
    public TMP_Text locationText;
    private bool forceLoc = false;

    public List<TargetLocation> targetLocations = new List<TargetLocation>();

    private float currentLat = 0f;
    private float currentLon = 0f;
    public float zoom = 10;

    public float forceLocControl_Speed = 100f;
    private bool updatingLoc = false;

    public static LocationManager Instance;


    private bool locationValid = false;
    public bool isLocationValid {  get { return locationValid; } }

    public RawImage mapImageComp;

    private float width = 0f;
    private float height = 0f;

    public void ToggleForceLoc()
    {
        forceLoc = !forceLoc;

        if (forceLoc && targetLocations.Count != 0)
        {
            currentLat = targetLocations[0].latitude;
            currentLon = targetLocations[0].longitude;

            StartCoroutine(UpdateLocation());
        }
        else
        {
            updatingLoc = false;
        }
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        width = (int)Mathf.Round(mapImageComp.rectTransform.rect.width);
        if (width > 512)
        {   width = 512;}
        height = (int)Mathf.Round(mapImageComp.rectTransform.rect.height);
        if (height > 512)
        { height = 512; }

        // Start updating location
        StartCoroutine(StartTimeOut());
    }

    private void FixedUpdate()
    {
        if (forceLoc)
        {
            //Check input and adjust location based on input

            currentLat += Time.fixedDeltaTime * Joystick.current.stick.x.value * forceLocControl_Speed;
            currentLon += Time.fixedDeltaTime * Joystick.current.stick.y.value * forceLocControl_Speed;

            StartCoroutine (GetOneMap());
            //Debug.Log("Current stick value: " + Joystick.current.stick.x.value + ", " + Joystick.current.stick.y.value);
            //Debug.Log("Lat calc: " + (Time.fixedDeltaTime * Joystick.current.stick.x.value * forceLocControl_Speed));
        }

    }

    public void StartLocationTracking()
    {
        StartCoroutine(StartTimeOut());
    }

    IEnumerator StartTimeOut()
    {
        // Check if location service is enabled
        if (!Input.location.isEnabledByUser)
        {
            locationText.text = "Location not enabled";
            yield break;
        }

        // Start location service
        Input.location.Start(1f, 1f);

        int maxWait = 10;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            locationText.text = "Initializing...";
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait <= 0)
        {
            locationText.text = "Location timeout";
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            locationText.text = "Location failed";
            yield break;
        }

        // Start updating location
        StartCoroutine(UpdateLocation());
    }

    IEnumerator UpdateLocation()
    {
        updatingLoc = true;
        StartCoroutine(GetOneMap());
        while (updatingLoc)
        {
            if (!forceLoc)
            {
                currentLat = Input.location.lastData.latitude;
                currentLon = Input.location.lastData.longitude;
            }

            string result = $"Lat: {currentLat}\nLon: {currentLon}\n";

            bool insideAny = false;

            foreach (var target in targetLocations)
            {
                float distance = GetDistanceMeters(
                    currentLat, currentLon,
                    target.latitude, target.longitude
                );

                if (distance <= target.radiusMeters)
                {
                    result += $"Inside: {target.name}\n";
                    insideAny = true;
                    locationValid = true;
                }
            }

            if (!insideAny)
            {
                result += "Not in any target area";
                locationValid = false;
            }

            locationText.text = result;

            yield return new WaitForSeconds(1f);
        }
        updatingLoc = false;
        yield break;
    }

    // Haversine formula to calculate distance between two GPS points
    float GetDistanceMeters(float lat1, float lon1, float lat2, float lon2)
    {
        float R = 6371000f; // Earth radius in meters

        float dLat = Mathf.Deg2Rad * (lat2 - lat1);
        float dLon = Mathf.Deg2Rad * (lon2 - lon1);

        float a = Mathf.Sin(dLat / 2) * Mathf.Sin(dLat / 2) +
                  Mathf.Cos(Mathf.Deg2Rad * lat1) *
                  Mathf.Cos(Mathf.Deg2Rad * lat2) *
                  Mathf.Sin(dLon / 2) * Mathf.Sin(dLon / 2);

        float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));

        return R * c;
    }

    private string url = "";
    private bool mapIsLoading = false;
    private bool updateMap = false;

    IEnumerator GetOneMap()
    {
        Debug.Log("Starting On Map");
        //url = "https://maps.googleapis.com/maps/api/staticmap?center=" + currentLat + "," + currentLon + "&zoom=" + zoom + "&size=" + 500 + "x" + 500 + "&scale=" + 600 + "&maptype=";// + mapType + "&key=" + apiKey;
        url = "https://www.onemap.gov.sg/api/staticmap/getStaticImage?layerchosen=default&zoom=" + zoom + "&height=" + height + "&width=" + width + "&lat=" + currentLat + "&lng=" + currentLon;
        Debug.Log("url formed, " + url);
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

}