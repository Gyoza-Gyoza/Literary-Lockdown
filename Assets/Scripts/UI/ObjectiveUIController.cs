using UnityEngine;
using TMPro;

public class ObjectiveUIController : MonoBehaviour
{
    [SerializeField] public GameObject rewardScreen, raidingProgressScreen, confirmConcelScreen;
    [SerializeField] private TextMeshProUGUI booksRewardsText, pagesRewardsText, booksEscapedRewardsText, currntPlayers, totalPlayers;
    //[SerializeField] public TMP_Dropdown difficultyDropdown;
    //[SerializeField] public TMP_Dropdown timerDropdown;
    private int pageAmount;

    public TextMeshProUGUI booksEscapedText;
    public TextMeshProUGUI booksCapturedText;
    public TextMeshProUGUI timeText;

    public static ObjectiveUIController Instance;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject);
        }
    }
    void Update()
    {
        if (ObjectivesManager.Instance != null)
        {
            currntPlayers.text = ObjectivesManager.Instance.playersReadyInLobby.Value.ToString();
            totalPlayers.text = ObjectivesManager.Instance.playersInLobby.Value.ToString();

            UpdateTimeText(ObjectivesManager.Instance.remainingTime.Value);
        }
    }
    public void UpdateTimeText(float secondsTotal)
    {
        int minutes = (int)(secondsTotal / 60);
        int seconds = (int)(secondsTotal % 60);
        if (timeText != null)
            timeText.text = $"{minutes}:{seconds:00}";
    }
    public void EndGame()
    {
        Debug.Log("End Game Called");
        ObjectivesManager.Instance.gameEnded.Value = true;
        rewardScreen.SetActive(true);
        booksRewardsText.text = $"{ObjectivesManager.Instance.booksCaptured.Value}";
        pagesRewardsText.text = ObjectivesManager.Instance.pagesEarned.Value.ToString();
        booksEscapedRewardsText.text = ObjectivesManager.Instance.booksEscaped.Value.ToString();
        SaveLoadManager.PlayerData.pagesHeld += pageAmount;
        SaveLoadManager.SaveData();
    }

    public void PreMatureEndGame()
    {
        confirmConcelScreen.SetActive(false);
        ObjectivesManager.Instance.gameEnded.Value = true;
        rewardScreen.SetActive(true);

        booksRewardsText.text = ObjectivesManager.Instance.booksCaptured.Value.ToString();
        booksEscapedRewardsText.text = ObjectivesManager.Instance.booksEscaped.Value.ToString();
        pagesRewardsText.text = "0";
    }

    public void MightEndGame(bool input)
    {
        confirmConcelScreen.SetActive(input);
    }
}
