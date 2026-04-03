using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PopUpManager: MonoBehaviour
{
    public bool displaying = false;


    private bool cooling = false;
    private float count = 0;
    private float maxDisplayTime = 120f;
    private float maxCoolingTime = 20f;

    private List<PopUpWindow> popUpWindows = new List<PopUpWindow>();


    public static PopUpManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(this);
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        popUpWindows = this.GetComponentsInChildren<PopUpWindow>(true).ToList<PopUpWindow>();
    }

    private void Update()
    {
        //count += Time.deltaTime;

        //if (displaying)
        //{

        //}
        //else
        if (!displaying && !cooling) 
        {
            StartCooling();
        }
    }

    public void StartCooling()
    {
        StartCoroutine(Cooling());
    }


    IEnumerator Cooling()
    {

        cooling = true;


        while (count <= maxCoolingTime) 
        {
            count += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        popUpWindows[Random.Range(0, popUpWindows.Count)].OpenPopUp();
        //displaying = true;

        yield break;
    }


}
