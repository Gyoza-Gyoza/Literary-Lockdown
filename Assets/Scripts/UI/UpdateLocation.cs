using System;
using TMPro;
using UnityEngine;

public class UpdateLocation : MonoBehaviour
{
    private void Start()
    {
        GetComponent<TextMeshProUGUI>().text = LocationManager.Instance.LibraryBranch;
    }
}
