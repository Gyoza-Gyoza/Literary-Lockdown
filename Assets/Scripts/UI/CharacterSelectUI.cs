using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class CharacterSelectUI : MonoBehaviour
{
    private Tower m_localPlayer;
    private NetworkManager m_networkManager;
    private RectTransform rectTransform;

    public float openUIy = 160f;
    public float closeUIy = -201f;
    public float rate = 1f;

    private bool opened = true;

    public void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void TrySpawnTower(int towerIndex)
    {
        if ( NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerClientController>().TrySpawnTower(towerIndex) && opened)
        {
            //close the UI
            StartCoroutine(lerpCoroutine(openUIy, closeUIy));
            opened = false;
        }
        else { 
         
            //display an error ig
            
        }
    }

    public void ToggleUI()
    {
        if (opened)
        {
            StartCoroutine(lerpCoroutine(openUIy, closeUIy));
            opened = false;
        }
        else
        {
            StartCoroutine(lerpCoroutine(closeUIy, openUIy));
            opened = true;
        }
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
