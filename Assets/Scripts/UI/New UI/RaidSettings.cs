using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RaidSettings : MonoBehaviour
{
    public static RaidSettings instance;
    public GameObject lobbyDetailsGO;
    private LobbyDetails lobbyDetails;

    [Header("Difficulty")]
    public TextMeshProUGUI diffcultyTxt;
    private List<string> difficultyList = new List<string>{"EASY", "MEDIUM", "HARD"};
    private int difficultyIndex = 0;

    [Header("Timer")]
    public TextMeshProUGUI timerTxt;
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
            return;
        }

        difficultyIndex++;
        UpdateDifficulty();
    }

    public void DecreaseDifficulty()
    {
        if (difficultyIndex == 0)
        {
            Debug.Log("Reached min difficulty!");
            return;
        }

        difficultyIndex--;
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

        timerIndex++;
        UpdateTimer();
    }

    public void DecreaseTimer()
    {
        if (timerIndex == 0)
        {
            Debug.Log("Reached min time allowed!");
            return;
        }

        timerIndex--;
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
