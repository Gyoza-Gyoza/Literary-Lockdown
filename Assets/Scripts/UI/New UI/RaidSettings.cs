using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RaidSettings : MonoBehaviour
{
    public static RaidSettings instance;
    public GameObject lobbyDetailsGO;
    private LobbyDetails lobbyDetails;


    [Header("Difficulty")]
    public TextMeshProUGUI diffcultyTxt;
    public Button decreaseDiff_Button;
    public Button increaseDiff_Button;
    public Sprite decreaseDiffActive_Sprite;
    public Sprite decreaseDiffInactive_Sprite;
    public Sprite increaseDiffActive_Sprite;
    public Sprite increaseDiffInactive_Sprite;
    private List<string> difficultyList = new List<string>{"EASY", "MEDIUM", "HARD"};
    private int difficultyIndex = 0;

    [Header("Timer")]
    public TextMeshProUGUI timerTxt;
    public Button increaseTime_Button;
    public Button decreaseTime_Button;
    public Sprite decreaseTimeActive_Sprite;
    public Sprite decreaseTimeInactive_Sprite;
    public Sprite increaseTimeActive_Sprite;
    public Sprite increaseTimeInactive_Sprite;
    private List<int> timerList = new List<int> {300, 600, 900, 1800, 2700, 3600};
    private int timerIndex = 0;


    public void Awake()
    {
        // Singleton Pattern
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    public void Start()
    {
        if (FindFirstObjectByType<LobbyDetails>() != null)
        {
            lobbyDetails = GameObject.FindFirstObjectByType<LobbyDetails>();

            // Try to extract existing settings from the lobby details script if it exists
            var (raidDifficulty, raidTime) = lobbyDetails.GetRaidDetails();
            difficultyIndex = raidDifficulty;
            timerIndex = timerList.IndexOf(raidTime);
        }

        UpdateDifficulty();
        UpdateTimer();
    }

    public void LockInSettings()
    {
        if (lobbyDetails == null)
        {
             lobbyDetails = Instantiate(lobbyDetailsGO).GetComponent<LobbyDetails>();
        }
        
        lobbyDetails.SetRaidDetails(difficultyIndex, timerList[timerIndex]);
    }

    #region Difficulty Functions

    public void IncreaseDifficulty()
    {
        if (difficultyIndex == difficultyList.Count - 1)
        {
            Debug.Log("Reached max difficulty!");
            // Insert visual feedback here
            
            return;
        }

        decreaseDiff_Button.image.sprite = decreaseDiffActive_Sprite;

        difficultyIndex++;

        if (difficultyIndex == difficultyList.Count - 1)
        {
            increaseDiff_Button.image.sprite = increaseDiffInactive_Sprite;
        }

        UpdateDifficulty();
    }

    public void DecreaseDifficulty()
    {
        if (difficultyIndex == 0)
        {
            Debug.Log("Reached min difficulty!");
            // Insert visual feedback here
            return;
        }


        increaseDiff_Button.image.sprite = increaseDiffActive_Sprite;
        difficultyIndex--;

        if (difficultyIndex == 0)
        {
            decreaseDiff_Button.image.sprite = decreaseDiffInactive_Sprite;
        }

        UpdateDifficulty();
    }

    public void UpdateDifficulty()
    {
        diffcultyTxt.text = difficultyList[difficultyIndex];
        LockInSettings();
    }

    #endregion

    #region Timer Functions

    public void IncreaseTimer()
    {
        if (timerIndex == timerList.Count - 1)
        {
            Debug.Log("Reached max time allowed!");
            return;
        }

        decreaseTime_Button.image.sprite = decreaseTimeActive_Sprite;
        timerIndex++;

        if (timerIndex == timerList.Count - 1)
        {
            increaseTime_Button.image.sprite = increaseTimeInactive_Sprite;
        }

        UpdateTimer();
    }

    public void DecreaseTimer()
    {
        if (timerIndex == 0)
        {
            Debug.Log("Reached min time allowed!");
            return;
        }

        increaseTime_Button.image.sprite = increaseTimeActive_Sprite;
        timerIndex--;

        if (timerIndex == 0)
        {
            decreaseTime_Button.image.sprite = decreaseTimeInactive_Sprite;
        }

        UpdateTimer();
    }

    public void UpdateTimer()
    {
        int minutes = (int)(timerList[timerIndex] / 60);
        int seconds = (int)(timerList[timerIndex] % 60);
        if (timerTxt != null)
            timerTxt.text = $"{minutes}:{seconds:00}";

        LockInSettings();
    }

    #endregion
}
