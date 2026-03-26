using Unity.VisualScripting;
using UnityEngine;

public class PopUpWindow: MonoBehaviour
{
    public void OpenPopUp()
    {
        this.gameObject.SetActive(true);
    }

    public void ClosePopUp()
    {
        this.gameObject.SetActive(false);
    }
    
}
