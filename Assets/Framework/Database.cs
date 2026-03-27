using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using Object = System.Object;

public class Database : MonoBehaviour
{
    [System.Serializable]
    private class DatabaseLink
    {
        [Tooltip("The name to reference this database by.")]
        public string databaseName;
        [Tooltip("The Google Sheets GID code for this database.")]
        public string sheetCode;
    }
    [SerializeField]
    private DatabaseLink[] databaseLinks;
    [HideInInspector]
    public Dictionary<string, Dictionary<string, Object>> database = new();
    public static Database Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
            CreateDatabases();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        
    }
    private void CreateDatabases()
    {
        if (database != null) database.Clear(); 

        foreach (DatabaseLink links in databaseLinks)
        {
            StartCoroutine(GetDatabase(links.databaseName, links.sheetCode, (csv) =>
            {
                string[] data = csv.Split("\r\n");

                if (!database.ContainsKey(links.databaseName))
                {
                    database.Add(links.databaseName, new Dictionary<string, Object>());
                }

                for (int i = 1; i < data.Length; i++)
                {
                    string[] values = data[i].Split(',');
                    // Add more database types here as needed
                    switch (links.databaseName)
                    {
                        case "Towers": 
                            database[links.databaseName].Add(values[0], 
                                new TowerData(values[0], 
                                values[1],
                                values[2] == "" ? values[0] : values[2],
                                values[3], 
                                values[4], 
                                int.Parse(values[5])));
                            Debug.Log("Intialized towers database");
                            break;
                        
                        case "ShopItems":
                            database[links.databaseName].Add(values[0],
                                new ShopItemData(values[1],
                                    values[2],
                                    int.Parse(values[3])));
                            Debug.Log("Intialized shop items database");
                            break;
                        
                        case "LocationData":
                            database[links.databaseName].Add(values[0], 
                                    new TargetLocationData(values[1],
                                        float.Parse(values[2]),
                                        float.Parse(values[3]),
                                        float.Parse(values[4])));
                            break;
                        
                        default:
                            Debug.Log($"No database type defined for {links.databaseName}");
                            break;
                    }
                }
            }));
        }
    }
    public static IEnumerator GetDatabase(string name, string sheetCode, Action<string> callback)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get($"https://docs.google.com/spreadsheets/d/18vfbpEUDMCO6SDPVQl8QoMBk489zCTX43JtCtNc4mHE/export?gid={sheetCode}&format=csv");

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            callback?.Invoke(webRequest.downloadHandler.text);
        }
        else
        {
            Debug.LogError($"Failed to download CSV: {webRequest.error}");
        }
    }
}
