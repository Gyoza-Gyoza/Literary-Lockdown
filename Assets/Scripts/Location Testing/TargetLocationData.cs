using UnityEngine;

[System.Serializable]
public class TargetLocationData
{
    private string name;
    private float latitude;
    private float longitude;
    private float radiusMeters;

    public string Name => name;

    public float Latitude => latitude;

    public float Longitude => longitude;

    public Vector2 Location
    {
        get { return new Vector2(latitude, longitude); }
    }

    public float RadiusMeters => radiusMeters;

    public TargetLocationData(string name, float latitude, float longitude, float radiusMeters)
    {
        this.name = name;
        this.latitude = latitude;
        this.longitude = longitude;
        this.radiusMeters = radiusMeters;
    }
}