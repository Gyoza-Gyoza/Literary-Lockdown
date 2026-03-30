using NUnit.Framework;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    private Tower m_localPlayer;
    private NetworkManager m_networkManager;
    private RectTransform rectTransform;

    public GameObject closeUI;

    public static CharacterSelectUI instance;

    public float openUIy = 160f;
    public float closeUIy = -201f;
    public float rate = 1f;

    private bool opened = true;

    public TMP_Text leftTabText;
    public TMP_Text towersSpawnedCount;
    public TMP_Text maxTowerCount;
    public Image rightTabImage;
    public Sprite maxTowerTabSprite;
    public Sprite notMaxTowerTabSprite;


    public Sprite rapuUnspawned;
    public Sprite rapuSpawned;
    public Sprite wolfUnspawned;
    public Sprite wolfSpawned;
    public Sprite frogUnspawned;
    public Sprite frogSpawned;


    public Button rapButton;
    public Button wolfButton;
    public Button frogButton;

    public void Awake()
    {
        rectTransform = this.GetComponent<RectTransform>();

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        maxTowerCount.text = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerClientController>().maxTowers.ToString();
    }

    public void Update()
    {
        if (ObjectivesManager.Instance.isGameStart() && opened)
        {
            HideUITotally();
        }
    }

    public void SpawningAvailable()
    {
        rapButton.image.sprite = rapuUnspawned;
        rapButton.interactable = true;
        wolfButton.image.sprite = wolfUnspawned;
        wolfButton.interactable = true;
        frogButton.image.sprite = frogUnspawned;
        frogButton.interactable = true;


        rightTabImage.sprite = notMaxTowerTabSprite;

        leftTabText.text = "Place Companion";
    }

    public void SpawningUnavailable()
    {
        rapButton.image.sprite = rapuSpawned;
        rapButton.interactable = false;
        wolfButton.image.sprite = wolfSpawned;
        wolfButton.interactable = false;
        frogButton.image.sprite = frogSpawned;
        frogButton.interactable = false;

        rightTabImage.sprite = maxTowerTabSprite;

        leftTabText.text = "Max Companion Placed";
    }

    public void TrySpawnTower(int towerIndex)
    {
        if (NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerClientController>().TrySpawnTower(towerIndex))
        {
            towersSpawnedCount.text = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerClientController>().currentTowers.Value.ToString();

            if (NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerClientController>().currentTowers.Value >= NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerClientController>().maxTowers)
            {
                SpawningUnavailable();
            }

            //ToggleUI();
            //ToggleButton(towerIndex, true);
        }
    }

    public void ToggleUI()
    {
        if (opened)
        {
            StartCoroutine(lerpCoroutine(openUIy, closeUIy));
            closeUI.SetActive(true);
            opened = false;
        }
        else
        {
            StartCoroutine(lerpCoroutine(closeUIy, openUIy));
            closeUI.SetActive(false);
            opened = true;
        }
    }

    public void HideUITotally()
    {

            StartCoroutine(lerpCoroutine(openUIy, closeUIy));
            closeUI.SetActive(false);
            opened = false;

    }

    IEnumerator lerpCoroutine(float start, float end)
    {
        float count = 0f;

        while (count <= rate)
        {
            count += Time.fixedDeltaTime;

            rectTransform.position = new Vector2(rectTransform.position.x,  Mathf.Lerp(start, end, count / rate));

            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

        yield break;
    }
}
