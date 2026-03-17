using System;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SaveLoadManager : MonoBehaviour
{
    private static PlayerMetadata m_playerData;
    public static PlayerMetadata PlayerData
    {
        get
        {
            return m_playerData;
        }
    }

    [Header("UI Elements")]
    //public TMP_InputField inputField;
    //public TextMeshProUGUI TMPInputUsername;
    //public TextMeshProUGUI TMPInputUsername_Placeholder;

    //private static SaveLoadManager instance;
    public static SaveLoadManager Instance { get; private set ;}
    //public static SaveLoadManager Instance { get { return instance; } private set { instance = value; DontDestroyOnLoad(value.gameObject); } }

    public static void SaveData()
    {
        // Convert the C# object to a JSON string
        string json = JsonUtility.ToJson(m_playerData, true); // The 'true' pretty-prints the JSON for readability

        // Define the file path using Application.persistentDataPath for cross-platform compatibility
        string path = Path.Combine(Application.persistentDataPath, "PlayerMetadata.json");

        // Write the JSON string to a file
        File.WriteAllText(path, json);

        Debug.Log("Data saved to: " + path);
    }

    public static PlayerMetadata LoadData()
    {
        string path = Path.Combine(Application.persistentDataPath, "PlayerMetadata.json");
        PlayerMetadata data = null;

        if (File.Exists(path))
        {
            // Read the JSON string from the file
            string json = File.ReadAllText(path);

            // Convert the JSON string back to a C# object
            data = JsonUtility.FromJson<PlayerMetadata>(json);

            Debug.Log("Data loaded from: " + path);
        }
        else
        {
            // Return a new instance if no save file exists
            Debug.LogWarning("Save file not found in " + path);
            data = new PlayerMetadata(); 
        }

        return data;
    }

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject); // Persist across scenes
            m_playerData = LoadData();

            //trying to remove start
            /*
            if (m_playerData.playerName != "" && SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(0))
            {
                TMPInputUsername_Placeholder.text = "Saved as " + m_playerData.playerName;
            }
            */
            //end
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetUsername(string input)
    {
        m_playerData.playerName = input;
        SaveData();
        
        //Trying to remove start
        /*
        if (m_playerData.playerName == TMPInputUsername.text)
        {
            inputField.text = "";
            TMPInputUsername_Placeholder.text = "Saved as " + m_playerData.playerName;
        }
        else
        {
            Debug.Log("Failed to save username");
        }
        */
        //end
    }

    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex, LoadSceneMode.Single);
    }
}
