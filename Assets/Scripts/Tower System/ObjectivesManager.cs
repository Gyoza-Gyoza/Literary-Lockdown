using UnityEngine;
using Unity.Netcode;
using TMPro;

public class ObjectivesManager : NetworkBehaviour
{
    [SerializeField] private GameObject rewardScreen;
    [SerializeField] private TextMeshProUGUI booksRewardsText, pagesRewardsText; 

    public TextMeshProUGUI timeText;
    private NetworkVariable<float> remainingTime = new NetworkVariable<float>(900); // 15 minutes in seconds
    [SerializeField]
    private NetworkVariable<bool> startGame = new NetworkVariable<bool>(false);
    private NetworkVariable<bool> gameEnded = new NetworkVariable<bool>(false);
    private int pageAmount; 

    public NetworkVariable<int> booksCaptured = new NetworkVariable<int>(0);
    public TextMeshProUGUI booksCapturedText;

    public static ObjectivesManager Instance;
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += () =>
        {
            // Only server sets the NetworkVariable; clients will enable UI via OnValueChanged
            //startGame.Value = true;
        };
    }

    public override void OnNetworkSpawn()
    {
        // Subscribe only on clients (host counts as client too)
        if (IsClient)
        {
            remainingTime.OnValueChanged += OnRemainingTimeChanged;
            booksCaptured.OnValueChanged += OnBooksCapturedChanged;
            startGame.OnValueChanged += OnStartGameChanged;
            gameEnded.OnValueChanged += OnGameEndedChanged;

            // Initialize UI from current networked values so late-joining clients see current state immediately
            ApplyAllNetworkValuesToUI();
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            remainingTime.OnValueChanged -= OnRemainingTimeChanged;
            booksCaptured.OnValueChanged -= OnBooksCapturedChanged;
            startGame.OnValueChanged -= OnStartGameChanged;
            gameEnded.OnValueChanged -= OnGameEndedChanged;
        }
    }

    void Update()
    {
        if (!IsServer) return;

        if (startGame.Value == true)
        {
            remainingTime.Value -= Time.deltaTime;

            if (remainingTime.Value <= 0 || Input.GetKeyDown(KeyCode.P))
            {
                remainingTime.Value = 0;
                startGame.Value = false;
                gameEnded.Value = true;
            }
        }
        else
        {
            foreach(NetworkClient playerClient in NetworkManager.ConnectedClientsList)
            {
                PlayerClientController cilent = playerClient.PlayerObject.GetComponent<PlayerClientController>();
                if (cilent.playerReady.Value == false)
                {
                    return;
                }
            }

            startGame.Value = true;
        }
    }

    public void PrematureEndGame()
    {
        if (IsServer)
        {
            remainingTime.Value = 0;
            startGame.Value = false;
            gameEnded.Value = true;
        }

    }

    public void CaptureBooks()
    {
        if (!IsServer) return;
        booksCaptured.Value++;
    }

    private void EndGame()
    {
        rewardScreen.SetActive(true);
        booksRewardsText.text = $"{booksCaptured.Value}";
        pageAmount = (int)(booksCaptured.Value * Random.Range(1.5f, 2.3f));
        pagesRewardsText.text = $"{pageAmount}";
        SaveLoadManager.PlayerData.pagesHeld += pageAmount;
        SaveLoadManager.SaveData();
    }

    private void OnRemainingTimeChanged(float oldValue, float newValue)
    {
        UpdateTimeText(newValue);
    }

    private void OnBooksCapturedChanged(int oldValue, int newValue)
    {
        if (booksCapturedText != null)
            booksCapturedText.text = $"{newValue}";
    }

    private void OnStartGameChanged(bool oldValue, bool newValue)
    {
        var timeParent = timeText.transform.parent.gameObject;
        var booksParent = booksCapturedText.transform.parent.gameObject;
        if (timeParent != null) timeParent.SetActive(newValue);
        if (booksParent != null) booksParent.SetActive(newValue);
        Debug.Log($"Start game changed");
    }

    private void OnGameEndedChanged(bool oldValue, bool newValue)
    {
        if (newValue)
        {
            EndGame();
        }
        else
        {
            if (rewardScreen != null) rewardScreen.SetActive(false);
        }
    }

    private void UpdateTimeText(float secondsTotal)
    {
        int minutes = (int)(secondsTotal / 60);
        int seconds = (int)(secondsTotal % 60);
        if (timeText != null)
            timeText.text = $"{minutes}:{seconds:00}";
    }

    private void ApplyAllNetworkValuesToUI()
    {
        // Called on client when they spawn to initialize UI
        if (timeText != null)
            UpdateTimeText(remainingTime.Value);

        if (booksCapturedText != null)
            booksCapturedText.text = $"{booksCaptured.Value}";

        var timeParent = timeText.transform.parent.gameObject;
        var booksParent = booksCapturedText.transform.parent.gameObject;
        if (timeParent != null) timeParent.SetActive(startGame.Value);
        if (booksParent != null) booksParent.SetActive(startGame.Value);

        if (gameEnded.Value)
        {
            EndGame();
        }
        else if (rewardScreen != null)
        {
            rewardScreen.SetActive(false);
        }
    }

    public bool isGameStart()
    {
        return startGame.Value;
    }
}
