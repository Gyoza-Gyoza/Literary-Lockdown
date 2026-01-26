using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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
    public Dictionary<string, List<object>> database = new();
    public static Database instance;

    private void Awake()
    {
        Bootstrap.SingletonInitialization += () =>
        {
            if (instance == null) instance = this;
            else Destroy(this);
        };
        Bootstrap.AcceptLoadRegistrations += CreateDatabases;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            string input = "Tower data count is" + database["Towers"].Count + "\n";
            foreach (TowerData item in database["Towers"])
            {
                input += $"Name: {item.Name}, Damage: {item.Stats.Damage}, Range: {item.Stats.Range}, Fire Rate: {item.Stats.AttackSpeed}\n";
            }
            Debug.Log(input);
        }
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
                    database.Add(links.databaseName, new List<object>());
                }

                for (int i = 1; i < data.Length; i++)
                {
                    // Add more database types here as needed
                    switch (links.databaseName)
                    {
                        case "Towers":
                            string[] values = data[i].Split(',');
                            database[links.databaseName].Add(new TowerData(values[0], int.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3])));
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
        LoadingManager.instance.StartLoading(name);

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            callback?.Invoke(webRequest.downloadHandler.text);
            LoadingManager.instance.FinishLoading(name);
        }
        else
        {
            Debug.LogError($"Failed to download CSV: {webRequest.error}");
        }
    }
}
