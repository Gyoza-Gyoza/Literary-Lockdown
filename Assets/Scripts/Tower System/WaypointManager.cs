using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    public Transform startPoint; 
    public Transform[] waypoints;
    public static WaypointManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
}
