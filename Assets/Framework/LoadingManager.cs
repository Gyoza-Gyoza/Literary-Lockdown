using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI loadingText; 
    private List<object> loadingList = new();
    private List<object> completedList = new();

    public static LoadingManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }

    public void StartLoading(object objectToLoad)
    {
        loadingList.Add(objectToLoad);
        UpdateText();
    }
    public void FinishLoading(object objectToLoad)
    {
        loadingList.Remove(objectToLoad);
        completedList.Add(objectToLoad);
        UpdateText();
    }
    private void UpdateText()
    {
        loadingText.text = "";

        foreach (object obj in loadingList)
        {
            loadingText.text += obj.GetType().Name + " is loading\n";
        }
        foreach (object obj in completedList)
        {
            loadingText.text += obj.GetType().Name + " completed loading\n";
        }
    }
}
