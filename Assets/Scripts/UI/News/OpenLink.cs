using UnityEngine;

public class OpenLink : MonoBehaviour
{

    [SerializeField] private string url;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenURL()
    {
        Application.OpenURL(url);
    }
}
