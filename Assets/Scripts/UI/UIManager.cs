using System.Collections;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject TowerSpawner;
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
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
