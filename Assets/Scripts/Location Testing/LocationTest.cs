using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class TargetLocation
{
    public string name;
    public float latitude;
    public float longitude;
    public float radiusMeters;
}

public class LocationTest : MonoBehaviour
{
    public TMP_Text locationText;

    public List<TargetLocation> targetLocations = new List<TargetLocation>();

    private float currentLat;
    private float currentLon;

    IEnumerator Start()
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
        while (true)
        {
            currentLat = Input.location.lastData.latitude;
            currentLon = Input.location.lastData.longitude;

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
                }
            }

            if (!insideAny)
            {
                result += "Not in any target area";
            }

            locationText.text = result;

            yield return new WaitForSeconds(1f);
        }
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
}