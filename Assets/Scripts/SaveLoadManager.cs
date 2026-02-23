using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SaveLoadManager : MonoBehaviour
{
    private PlayerMetadata m_playerData;

    [Header("UI Elements")]
    public TMP_InputField inputField;
    public TextMeshProUGUI TMPInputUsername;
    public TextMeshProUGUI TMPInputUsername_Placeholder;


    public static void SaveData(PlayerMetadata data)
    {
        // Convert the C# object to a JSON string
        string json = JsonUtility.ToJson(data, true); // The 'true' pretty-prints the JSON for readability

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
        m_playerData = LoadData();

        if (m_playerData.playerName != "" && SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(0))
        {
            TMPInputUsername_Placeholder.text = "Saved as " + m_playerData.playerName;
        }
    }

    public void SetUsername()
    {
        m_playerData.playerName = TMPInputUsername.text;
        SaveData(m_playerData);
        
        if (m_playerData.playerName == TMPInputUsername.text)
        {
            inputField.text = "";
            TMPInputUsername_Placeholder.text = "Saved as " + m_playerData.playerName;
        }
    }

    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
