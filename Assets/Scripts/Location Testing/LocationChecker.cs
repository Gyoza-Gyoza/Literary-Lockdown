using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class LocationChecker : MonoBehaviour
{
    [SerializeField] private float returnDuration;
    [SerializeField] private float checkInterval = 1f;
    [SerializeField] private TextMeshProUGUI timerText; 
    private CanvasGroup canvasGroup;
    private float timer;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        if (timer <= checkInterval)
        {
            timer += Time.deltaTime;
            if (timer > checkInterval)
            {
                if (!LocationManager.Instance.isLocationValid)
                {
                    StartCoroutine(StartCountdown());
                }
                else
                {
                    timer = 0f;
                }
            }
        }
    }

    private IEnumerator StartCountdown()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        float returnTimer = returnDuration;
        while (returnTimer >= 0f)
        {
            returnTimer -= Time.deltaTime;
            timerText.text = ((int)returnTimer).ToString();
            yield return null;
        }
        NetworkHandler.Instance.LeaveSession();
    }
}
