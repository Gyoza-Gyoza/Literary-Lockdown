using System.Collections;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    private GameObject localPlayer;

    public GameObject TowerSpawner;
    public GameObject TowerSpawnerClosed;
    public GameObject TowerControlPanel;

    public GameObject seletedTower;

    [Header("Modal Window")]
    public GameObject ModalWindow;
    public TextMeshProUGUI ModalTitle;
    public TextMeshProUGUI ModalContent;


    [Header("UI")]
    public GameObject playerReadyUI;

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

        // Check if client has spawned in
        localPlayer = WaitForPlayerSpawn().Result;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize network Objects
        if (NetworkManager.Singleton.IsHost)
        {
            // Initialize network Objects
            foreach (GameObject obj in GameplayInitializer.Instance.initPrefabs)
            {
                NetworkObject networkObject = Instantiate(obj).GetComponent<NetworkObject>();
                networkObject.Spawn();
            }
        }


        TowerSpawner.SetActive(true);
        playerReadyUI.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (TowerControlPanel.activeSelf && seletedTower != null)
        {
            Vector3 targetPosition = Camera.main.WorldToScreenPoint(seletedTower.transform.position);
            TowerControlPanel.transform.position = targetPosition;
        }
    }

    // Change the return type of WaitForPlayerSpawn from GameObject to Task<GameObject>
    public async Task<GameObject> WaitForPlayerSpawn()
    {
        while (NetworkManager.Singleton.LocalClient == null)
        {
            await Task.Yield();
        }
        
        return NetworkManager.Singleton.LocalClient.PlayerObject.gameObject;
    }

    public void ShowModalWindow(string title, string message)
    {
        ModalTitle.text = title;
        ModalContent.text = message;

        StartCoroutine(DisplayModalWindow(3f));
    }

    public IEnumerator DisplayModalWindow(float displayDuration)
    {
        float count = 0f;
        float lerpDuration = 0.5f;

        while (count <= lerpDuration)
        {
            count += Time.fixedDeltaTime;

            ModalWindow.transform.position = new Vector2(transform.position.x, Mathf.Lerp(-111, 110, count / lerpDuration));

            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

        yield return new WaitForSeconds(displayDuration);

        count = 0f;

        while (count <= lerpDuration)
        {
            count += Time.fixedDeltaTime;

            ModalWindow.transform.position = new Vector2(transform.position.x, Mathf.Lerp(110, -111, count / lerpDuration));

            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

        yield break;
    }

    public void ShowPlayerReadyUI()
    {
        playerReadyUI.SetActive(true);
    }
}
