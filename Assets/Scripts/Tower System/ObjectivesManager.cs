using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.UIElements;

public class ObjectivesManager : NetworkBehaviour
{
    public NetworkVariable<float> remainingTime = new NetworkVariable<float>(300); // 5 minutes in seconds
    [SerializeField]
    private NetworkVariable<bool> startGame = new NetworkVariable<bool>(false);
    public bool isGameStart() { return startGame.Value;}
    private NetworkVariable<bool> gameEnded = new NetworkVariable<bool>(false);
    private int pageAmount; 

    public NetworkVariable<int> booksCaptured = new NetworkVariable<int>(0);

    public NetworkVariable<int> playersInLobby = new NetworkVariable<int>(0);
    public NetworkVariable<int> playersReadyInLobby = new NetworkVariable<int>(0);

    public NetworkVariable<int> difficulty = new NetworkVariable<int>(0);
    public NetworkVariable<int> timerValue = new NetworkVariable<int>(0);

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

            // Yes
            difficulty.OnValueChanged += (oldValue, newValue) =>
            {
                ObjectiveUIController.Instance.difficultyDropdown.value = newValue;
            };

            ObjectiveUIController.Instance.difficultyDropdown.onValueChanged.AddListener((int value) => { OnDifficultyChanged(value); });

            timerValue.OnValueChanged += (oldValue, newValue) =>
            {
                ObjectiveUIController.Instance.timerDropdown.value = newValue;
            };

            ObjectiveUIController.Instance.timerDropdown.onValueChanged.AddListener((int value) => { OnTimerDurationChange(value); });

            // Initialize UI from current networked values so late-joining clients see current state immediately
            ApplyAllNetworkValuesToUI();
        }
    }

    public void OnDifficultyChanged(int value)
    {
        if (IsHost)
        {
            difficulty.Value = value;
        }
        else
        {
            Debug.LogWarning("Only host can change difficulty");
            UIManager.Instance.ShowModalWindow("Permission Denied", "Only the host can change the difficulty setting.");
            ObjectiveUIController.Instance.difficultyDropdown.value = difficulty.Value;
        }
    }

    public void OnTimerDurationChange(int value)
    {
        if (IsHost)
        {
            switch (value)
            {
                case 0:
                    remainingTime.Value = 300; // 5 minutes
                    break;
                case 1:
                    remainingTime.Value = 600; // 10 minutes
                    break;
                case 2:
                    remainingTime.Value = 900; // 15 minutes
                    break;
                case 3:
                    remainingTime.Value = 1800; // 30 minutes
                    break;
                case 4:
                    remainingTime.Value = 2700; // 45 minutes
                    break;
                case 5:
                    remainingTime.Value = 3600; // 60 minutes
                    break;
                default:
                    Debug.LogError("Invalid timer duration value");
                    UIManager.Instance.ShowModalWindow("Error", "Invalid timer duration value.");
                    break;
            }

            // Update the NetworkVariable to sync with clients
            timerValue.Value = value; 
        }
        else
        {
            Debug.LogWarning("Only host can change timer duration");
            UIManager.Instance.ShowModalWindow("Permission Denied", "Only the host can change the timer duration.");
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
        if (IsServer)
        {
            playersInLobby.Value = NetworkManager.ConnectedClientsList.Count;

            playersReadyInLobby.Value = 0;
            foreach (NetworkClient playerClient in NetworkManager.ConnectedClientsList)
            {
                PlayerClientController cilent = playerClient.PlayerObject.GetComponent<PlayerClientController>();
                if (cilent.playerReady.Value == false)
                {
                    continue;
                }

                playersReadyInLobby.Value++;
            }

            // Server Logic for game start
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
                // Server Logic to check if all players are ready to start the game
                foreach (NetworkClient playerClient in NetworkManager.ConnectedClientsList)
                {
                    PlayerClientController cilent = playerClient.PlayerObject.GetComponent<PlayerClientController>();
                    if (cilent.playerReady.Value == false)
                    {
                        return;
                    }
                }

                // Check pass, start game
                startGame.Value = true;

                // Send command to enable game UI for all clients
                InitializeGameStartRpc();
            }
        }
    }

    [Rpc(SendTo.Everyone)]
    public void InitializeGameStartRpc()
    {
        // Hide Character Select UI
        FindFirstObjectByType<CharacterSelectUI>().gameObject.SetActive(false);

        // Show Objective UI
        var timeParent = ObjectiveUIController.Instance.timeText.transform.parent.gameObject.transform.parent.gameObject;
        var booksParent = ObjectiveUIController.Instance.booksCapturedText.transform.parent.gameObject;
        if (timeParent != null) timeParent.SetActive(true);
        if (booksParent != null) booksParent.SetActive(true);

        TowerManager.Instance.HideRangeOfTowers();

        // Disable tower control panel UI
        UIManager.Instance.TowerControlPanel.SetActive(false);
        UIManager.Instance.TowerSpawner.SetActive(false);
        UIManager.Instance.TowerSpawnerClosed.SetActive(false);
        UIManager.Instance.seletedTower = null;
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

    private void OnRemainingTimeChanged(float oldValue, float newValue)
    {
        ObjectiveUIController.Instance.UpdateTimeText(newValue);
    }

    private void OnBooksCapturedChanged(int oldValue, int newValue)
    {
        if (ObjectiveUIController.Instance.booksCapturedText != null)
            ObjectiveUIController.Instance.booksCapturedText.text = $"{newValue}";
    }

    private void OnStartGameChanged(bool oldValue, bool newValue)
    {
        Debug.Log($"Start game changed");
    }

    private void OnGameEndedChanged(bool oldValue, bool newValue)
    {
        if (newValue)
        {
            ObjectiveUIController.Instance.EndGame();
        }
        else
        {
            if (ObjectiveUIController.Instance.rewardScreen != null) ObjectiveUIController.Instance.rewardScreen.SetActive(false);
        }
    }

    private void ApplyAllNetworkValuesToUI()
    {
        if (gameEnded.Value)
        {
            ObjectiveUIController.Instance.EndGame();
        }
        else if (ObjectiveUIController.Instance.rewardScreen != null)
        {
            ObjectiveUIController.Instance.rewardScreen.SetActive(false);
        }
    }
}
