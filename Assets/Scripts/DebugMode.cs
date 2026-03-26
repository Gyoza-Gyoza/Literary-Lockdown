using System;
using UnityEngine;

public class DebugMode : MonoBehaviour
{
    public bool debugMode { get; private set; } = true;
    public static DebugMode Instance;
    private int locationIndex = 0;
    
    private void Awake()
    {
        if (Instance == null) Instance = this; 
        else Destroy(gameObject);
    }

    private void Start()
    {
        ToggleDebugMode();
    }

    private void Update()
    {
        if ((Input.GetKey(KeyCode.LeftControl)
             || Input.GetKey(KeyCode.RightControl))
            && Input.GetKeyDown(KeyCode.Q))
        {
            ToggleDebugMode();
        }

        if (debugMode)
        {
            if (Input.GetKeyDown(KeyCode.P)) LocationManager.Instance.NextLocation();
            if (Input.GetKeyDown(KeyCode.O)) LocationManager.Instance.PreviousLocation();
        }
    }

    private void ToggleDebugMode()
    {
        debugMode = !debugMode;
        gameObject.SetActive(debugMode);
    }
}
