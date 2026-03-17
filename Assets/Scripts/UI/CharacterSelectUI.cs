using NUnit.Framework;
using System.Collections;
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

    public Sprite rapuUnspawned;
    public Sprite rapuSpawned;
    public Sprite wolfUnspawned;
    public Sprite wolfSpawned;

    public Image rapButton;
    public Image wolfButton;

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
        //closeUI.SetActive(false);
    }

    public void Update()
    {
        if (ObjectivesManager.Instance.isGameStart() && opened)
        {
            HideUITotally();
        }
    }

    public void ToggleButton(int towerIndex, bool spawned)
    {
        switch (towerIndex)
        {
            case 0:
                if (spawned)
                {
                    rapButton.sprite = rapuSpawned;
                }
                else
                {
                    rapButton.sprite = rapuUnspawned;
                }
                    break;
            case 1:
                if (spawned)
                {
                    wolfButton.sprite = wolfSpawned;
                }
                else
                {
                    wolfButton.sprite = wolfUnspawned;
                }
                break;
            default:
                break;
        }
    }

    public void TrySpawnTower(int towerIndex)
    {
        if (NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerClientController>().TrySpawnTower(towerIndex))
        {
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
