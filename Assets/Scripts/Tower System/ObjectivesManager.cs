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
    public NetworkVariable<bool> gameEnded = new NetworkVariable<bool>(false);
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

        if (IsHost)
        {
            // Try find the GameObject with the timer and difficulty settings
            var (raidDifficulty, raidTime) = FindFirstObjectByType<LobbyDetails>().GetRaidDetails();

            // Assign the extracted values to the NetworkVariables
            difficulty.Value = raidDifficulty;
            remainingTime.Value = raidTime;
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

                ApplyAllNetworkValuesToUI();
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
        //var timeParent = ObjectiveUIController.Instance.timeText.transform.parent.gameObject.transform.parent.gameObject;
        //var booksParent = ObjectiveUIController.Instance.booksCapturedText.transform.parent.gameObject;
        //if (timeParent != null) timeParent.SetActive(true);
        //if (booksParent != null) booksParent.SetActive(true);

        TowerManager.Instance.HideRangeOfTowers();
        ObjectiveUIController.Instance.raidingProgressScreen.SetActive(true);

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

    private void OnRemainingTimeChanged(float oldValue, float newValue)
    {
        ObjectiveUIController.Instance.UpdateTimeText(newValue);
    }

    public void CaptureBooks()
    {
        if (!IsServer) return;
        booksCaptured.Value++;
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
            gameEnded.Value = false;
            startGame.Value = false;

            Debug.Log("GAME END VALUE IS TRUE");

            ObjectiveUIController.Instance.EndGame();
            // GetComponent<NetworkObject>().Despawn();
            // Destroy(gameObject);

            Debug.Log("This shouldnt exist");
        }
        else if (ObjectiveUIController.Instance.rewardScreen != null)
        {
            ObjectiveUIController.Instance.rewardScreen.SetActive(false);
        }
    }
}
