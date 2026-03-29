using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Mathematics;

public class LocationManager : MonoBehaviour
{
    List<TargetLocationData> targetLocations = new List<TargetLocationData>();
    private TargetLocationData closest;

    public TargetLocationData Closest
    {
        get
        {
            TargetLocationData result = null;
            double closestDistance = math.INFINITY;
            foreach (var data in Database.Instance.database["LocationData"])
            {
                TargetLocationData locationData = (TargetLocationData)data.Value;
                if (result == null) result = locationData;
                
                double currentBranchDistance = math.distance(Location, locationData.Location);
                if (currentBranchDistance <= closestDistance)
                {
                    result = locationData;
                    closestDistance = currentBranchDistance;
                }
            }
            return result;
        }
    }
    public double currentLat { get; private set; } = 0f;
    public void ForceEditLat(double input) { currentLat += input; }
    public double currentLon { get; private set; } = 0f;
    public void ForceEditLon(double input) { currentLon += input; }

    public float forceLocControl_Speed = 100f;

    public static LocationManager Instance;

    private bool updatingLoc = false;
    public bool isUpdatingLoc { get { return updatingLoc; } }
    
    private bool forceLoc = false;
    public bool isForceLoc {  get { return forceLoc; } }

    private bool locationValid = false;

    public bool isLocationValid
    {
        get
        {
            foreach (var data in Database.Instance.database["LocationData"])
            {
                TargetLocationData locationData = (TargetLocationData)data.Value;
                if (GetDistanceMeters((float)Location.x, (float)Location.y, locationData.Latitude, locationData.Longitude) < locationData.RadiusMeters)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public double2 Location
    {
        get { return new double2(currentLat, currentLon); }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject);
        }
    }
    private void Start()
    {

        // Start updating location
        // StartCoroutine(StartTimeOut());
    }

    private void FixedUpdate()
    {
        if (DebugMode.Instance.debugMode && DebugMode.Instance.locationMode)
        {
            //Check input and adjust location based on input

            currentLon += Time.fixedDeltaTime * Joystick.current.stick.x.value * forceLocControl_Speed;
            currentLat += Time.fixedDeltaTime * Joystick.current.stick.y.value * forceLocControl_Speed;

            //Debug.Log("Current stick value: " + Joystick.current.stick.x.value + ", " + Joystick.current.stick.y.value);
            //Debug.Log("Lon calc: " + (Time.fixedDeltaTime * Joystick.current.stick.x.value * forceLocControl_Speed));
            //Debug.Log("Lat calc: " + (Time.fixedDeltaTime * Joystick.current.stick.y.value * forceLocControl_Speed));
        }
    }

    public void SetLocation(double2 location)
    {
        currentLat = location.x;
        currentLon = location.y;
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
            yield break;
        }

        // Start location service
        Input.location.Start(1f, 1f);

        int maxWait = 10;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            //nearestLocText.text = "Initializing...";
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait <= 0)
        {
            //nearestLocText.text = "Location timeout";
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            //nearestLocText.text = "Location failed";
            yield break;
        }

        // Start updating location
        StartCoroutine(UpdateLocation());
    }

    IEnumerator UpdateLocation()
    {
        updatingLoc = true;
        while (updatingLoc)
        {

            if (!forceLoc)
            {
                currentLat = Input.location.lastData.latitude;
                currentLon = Input.location.lastData.longitude;
            }

            //string result = "";//$"Lat: {currentLat}\nLon: {currentLon}\n";

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
                    insideAny = true;
                    locationValid = true;
                    closest = target;
                }

            }

            if (!insideAny)
            {
                //Change sprite
                locationValid = false;
            }

            //nearestLocText.text = result;

            yield return new WaitForSeconds(1f);
        }
        updatingLoc = false;
        yield break;
    }
    private bool updateMap = false;

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
}