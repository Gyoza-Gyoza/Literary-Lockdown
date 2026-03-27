using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PopUpWindow: MonoBehaviour
{
    public Button CloseButton;

    public void OpenPopUp()
    {
        this.gameObject.SetActive(true);
        PopUpManager.Instance.displaying = true;
        StartCoroutine(WaitBeforeShowingClose());
    }

    public void ClosePopUp()
    {
        this.gameObject.SetActive(false);
        CloseButton.gameObject.SetActive(false);
        PopUpManager.Instance.displaying = false;
    }

    IEnumerator WaitBeforeShowingClose()
    {

        float count = 30f;
        while (count > 0)
        {
            count -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        //CloseButton.gameObject.SetActive(true);

        yield break;
    }
}
