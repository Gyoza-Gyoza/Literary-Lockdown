using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UpdateUsername : MonoBehaviour
{
    private TextMeshProUGUI text;
    //public TextMeshProUGUI textPro;

    public TMP_InputField inputField;
    //public TextMeshProUGUI TMPInputUsername;
    public TextMeshProUGUI TMPInputUsername_Placeholder;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        text.text = SaveLoadManager.PlayerData.playerName.ToString();

        if (SaveLoadManager.PlayerData.playerName != "" /*&& SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(0)*/)
        {
            TMPInputUsername_Placeholder.text = "Saved as " + SaveLoadManager.PlayerData.playerName;
        }
    }

    public void SetUserName()
    {
        SaveLoadManager.Instance.SetUsername(inputField.text);

        if (SaveLoadManager.PlayerData.playerName == inputField.text)
        {
            inputField.text = "";
            TMPInputUsername_Placeholder.text = "Saved as " + SaveLoadManager.PlayerData.playerName;
        }
        else
        {
            Debug.Log("Failed to save username");
        }
    }
}
