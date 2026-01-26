using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI loadingText; 
    private List<string> loadingList = new();
    private List<string> completedList = new();

    private delegate void FinishedLoading();
    private event FinishedLoading OnFinishedLoading;
    public static LoadingManager instance;

    private bool loadingRegistrationCompleted = false;

    private void Awake()
    {
        Bootstrap.SingletonInitialization += () =>
        {
            if (instance == null) instance = this;
            else Destroy(this);
        };
        Bootstrap.StopLoadRegistrations += () => loadingRegistrationCompleted = true;
    }

    /// <summary>
    /// IMPORTANT: Call this before Start()
    /// </summary>
    /// <param name="objectToLoadName"></param>
    /// <param name="onFinishedLoading"></param>
    public void StartLoading(string objectToLoadName, Action onFinishedLoading = null)
    {
        Bootstrap.AcceptLoadRegistrations += () =>
        {
            loadingList.Add(objectToLoadName);
            if (onFinishedLoading != null) OnFinishedLoading += new FinishedLoading(onFinishedLoading);
            UpdateText();
        };
    }
    public void FinishLoading(string objectToLoadName)
    {
        loadingList.Remove(objectToLoadName);
        completedList.Add(objectToLoadName);
        UpdateText();
        if (loadingRegistrationCompleted && loadingList.Count == 0)
        {
            OnFinishedLoading?.Invoke();
            OnFinishedLoading = null;
        }
    }
    private void UpdateText()
    {
        loadingText.text = "";

        foreach (string name in loadingList)
        {
            loadingText.text += name + " is loading\n";
        }
        foreach (string name in completedList)
        {
            loadingText.text += name + " completed loading\n";
        }
    }
}
