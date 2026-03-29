using System;
using System.Linq;
using UnityEngine;

public class DebugMode : MonoBehaviour
{
    private CanvasGroup mainWindow;
    [SerializeField] private CanvasGroup mapWindow;
    public bool debugMode { get; private set; } = true;
    public static DebugMode Instance;
    
    private int locationIndex;
    private int LocationIndex
    {
        get { return locationIndex; }
        set
        {
            locationIndex = value;
            if (locationIndex >= Database.Instance.database["LocationData"].Count) locationIndex = 0; 
            else if (locationIndex < 0) locationIndex = Database.Instance.database["LocationData"].Count - 1;
            // Jia Le was working till here
            //LocationManager.Instance.SetLocation();
        }
    }
    public TargetLocationData CurrentLocationData
    {
        get { return (TargetLocationData)Database.Instance.database["LocationData"].ElementAt(LocationIndex).Value; }
    }
    private void Awake()
    {
        if (Instance == null) Instance = this; 
        else Destroy(gameObject);
        
        DontDestroyOnLoad(this);
        mainWindow = GetComponent<CanvasGroup>();
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
            if (Input.GetKeyDown(KeyCode.P))
            {
                LocationIndex++;
                Debug.Log("Getting next location");
            }

            if (Input.GetKeyDown(KeyCode.O))
            {
                LocationIndex--;
                Debug.Log("Getting previous location");
            }
            
            if (Input.GetKeyDown(KeyCode.L)) ToggleMapWindow();
        }
    }

    private void ToggleDebugMode()
    {
        debugMode = !debugMode;
        mainWindow.alpha =  debugMode ? 1 : 0;
    }
    private void ToggleMapWindow()
    {
        mapWindow.alpha = mapWindow.alpha == 0 ? 1 : 0;
    }
}
