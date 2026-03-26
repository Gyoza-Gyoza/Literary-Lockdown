using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PopUpManager: MonoBehaviour
{
    private bool displaying = false;
    private bool cooling = false;
    private float count = 0;
    private float maxDisplayTime = 120f;
    private float maxCoolingTime = 20f;

    private List<PopUpWindow> popUpWindows = new List<PopUpWindow>();

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
        //else if (cooling) 
        //{

        //}
    }

    public void StartCooling()
    {

    }


    IEnumerator Cooling()
    {

        cooling = true;

        count += Time.deltaTime;

        while (count <= maxCoolingTime) 
        {

        }

        yield break;
    }

}
