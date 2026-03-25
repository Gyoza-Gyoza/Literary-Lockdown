using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.UI;
using static Unity.Netcode.NetworkSceneManager;

public class LocationManager : MonoBehaviour
{
    public RawImage mapImageComp;
    public TMP_Text locationText;

    public List<TargetLocationData> targetLocations = new List<TargetLocationData>();
    private TargetLocationData closest;
    public CanvasGroup popUpGroup;

    private double currentLat = 0f;
    private double currentLon = 0f;
    public float zoom = 10;
    private float width = 0f;
    private float height = 0f;

    public float forceLocControl_Speed = 100f;

    public static LocationManager Instance;

    public int pointsInCircle = 8;
    public Color32 validLocCol = new Color32(); 
    public Color32 validRadCol = new Color32();


    private bool updatingLoc = false;
    private bool forceLoc = false;
    private bool locationValid = false;
    public bool isLocationValid {  get { return locationValid; } }

    public Vector2 Location
    {
        get { return new Vector2((float)currentLat, (float)currentLon); }
    }

    public string LibraryBranch
    {
        get
        {
            foreach (var data in Database.Instance.database["LocationData"])
            {
                TargetLocationData locationData = (TargetLocationData)data.Value;
                if (Vector2.Distance(Location, locationData.Location) <= locationData.RadiusMeters)
                {
                    return locationData.Name;
                }
            }
            return "Invalid Branch";
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(this);
        }
        else if (Instance != this)
        {
            // Destroy(this.gameObject);
        }
    }
    private void Start()
    {
        width = (int)Mathf.Round(mapImageComp.rectTransform.rect.width);
        if (width > 512) {width = 512;}
        height = (int)Mathf.Round(mapImageComp.rectTransform.rect.height);
        if (height > 512) { height = 512; }

        // Start updating location
        StartCoroutine(StartTimeOut());

        //CloseLocPopUp();
    }

    private void FixedUpdate()
    {
        if (forceLoc)
        {
            //Check input and adjust location based on input

            currentLon += Time.fixedDeltaTime * Joystick.current.stick.x.value * forceLocControl_Speed;
            currentLat += Time.fixedDeltaTime * Joystick.current.stick.y.value * forceLocControl_Speed;

            if (!mapIsLoading)
            {
                StartCoroutine(GetOneMap());
            }
            //Debug.Log("Current stick value: " + Joystick.current.stick.x.value + ", " + Joystick.current.stick.y.value);
            //Debug.Log("Lon calc: " + (Time.fixedDeltaTime * Joystick.current.stick.x.value * forceLocControl_Speed));
            //Debug.Log("Lat calc: " + (Time.fixedDeltaTime * Joystick.current.stick.y.value * forceLocControl_Speed));
        }

    }


    public void OpenLocPopUp()
    {
        popUpGroup.alpha = 1.0f;
    }


    public void CloseLocPopUp()
    {
        popUpGroup.alpha = 0f;
    }

    public void ToggleForceLoc()
    {
        forceLoc = !forceLoc;

        if (forceLoc && targetLocations.Count != 0)
        {
            currentLat = targetLocations[0].Latitude;
            currentLon = targetLocations[0].Longitude;

            StartCoroutine(UpdateLocation());
        }
        else
        {
            updatingLoc = false;
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

            string result = "";//$"Lat: {currentLat}\nLon: {currentLon}\n";

            bool insideAny = false;


            float currClosest = 999999f;

            foreach (TargetLocationData target in targetLocations)
            {
                // Add check that checks for the closest location
                float distance = GetDistanceMeters(
                    (float)currentLat, (float)currentLon,
                    target.Latitude, target.Longitude
                );

                if (distance < currClosest)
                {
                    currClosest = distance;
                    closest = target;
                }

                if (distance <= target.RadiusMeters)
                {
                    result += $"Inside: {target.Name}\n";
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

            if (!mapIsLoading)
            {
                StartCoroutine(GetOneMap());
            }

            yield return new WaitForSeconds(1f);
        }
        updatingLoc = false;
        yield break;
    }

    private string url = "";
    private bool mapIsLoading = false;
    private bool updateMap = false;

    IEnumerator GetOneMap()
    {
        Debug.Log("Starting On Map");
        //url = "https://maps.googleapis.com/maps/api/staticmap?center=" + currentLat + "," + currentLon + "&zoom=" + zoom + "&size=" + 500 + "x" + 500 + "&scale=" + 600 + "&maptype=";// + mapType + "&key=" + apiKey;
        url = "https://www.onemap.gov.sg/api/staticmap/getStaticImage?layerchosen=default&zoom=" + zoom + "&height=" + height + "&width=" + width + "&lat=" + currentLat + "&lng=" + currentLon + "&points=%5B" + closest.Latitude + "%2C%20" + closest.Longitude + "%2C%20%22" + validLocCol.r + "%2C%20" + validLocCol.g + "%2C%20"+ validLocCol.b + "%22%5D";
        

       /* string polygonURL*/  url += "&polygons=" + GenerateCirclePointsASCIIString(closest.Latitude, closest.Longitude, closest.RadiusMeters, pointsInCircle) + "%3A" + validRadCol.r + "%2C" + validRadCol.g + "%2C" + validRadCol.b;
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