using System;
using System.Linq;
using UnityEngine;

public class DebugMode : MonoBehaviour
{
    private CanvasGroup mainWindow;
    [SerializeField] private CanvasGroup mapWindow;
    public bool debugMode { get; private set; } = true;
    public bool locationMode  { get; private set; } = true;
    public static DebugMode Instance;
    private GameObject[] ads;
    private int locationIndex;
    private int LocationIndex
    {
        get { return locationIndex; }
        set
        {
            locationIndex = value;
            if (locationIndex >= Database.Instance.database["LocationData"].Count) locationIndex = 0; 
            else if (locationIndex < 0) locationIndex = Database.Instance.database["LocationData"].Count - 1;
            
            TargetLocationData locationData = (TargetLocationData)Database.Instance.database["LocationData"].ElementAt(locationIndex).Value;
            LocationManager.Instance.SetLocation(locationData.Location);
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
            if (Input.GetKeyDown(KeyCode.E))
            {
                LocationIndex++;
                Debug.Log("Getting next location");
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                LocationIndex--;
                Debug.Log("Getting previous location");
            }
            
            if (Input.GetKeyDown(KeyCode.R)) ToggleMapWindow();
            if (Input.GetKeyDown(KeyCode.P)) SaveLoadManager.PlayerData.pagesHeld += 100000;
            if (Input.GetKeyDown(KeyCode.L))
            {
                if (ads == null) ads = GameObject.FindGameObjectsWithTag("Ads");
                foreach (GameObject ad in ads) ad.SetActive(!ad.activeSelf);
            }
        }
    }

    public void ToggleDebugMode()
    {
        debugMode = !debugMode;
        
        mainWindow.alpha =  debugMode ? 1 : 0;

        if (debugMode)
        {
            LocationIndex = 0;
        }
        else
        {
            if (locationMode) ToggleMapWindow();
        }
    }
    public void ToggleMapWindow()
    {
        locationMode = !locationMode;
        
        mapWindow.alpha = locationMode ? 1 : 0;
        mapWindow.interactable = locationMode;
        mapWindow.blocksRaycasts = locationMode;
    }

    public void ToggleStraightIntoMapWindow()
    {
        ToggleDebugMode();
        ToggleMapWindow();
    }
}
