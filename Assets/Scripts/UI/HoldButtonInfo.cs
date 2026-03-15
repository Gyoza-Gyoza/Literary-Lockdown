using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoldButtonInfo : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public GameObject Hover;
    public float holdTimer = .5f;

    private float count = 0f;
    private bool counting = false;

    void Update()
    {
        if (counting)
        {
            count += Time.deltaTime;

            if (count > holdTimer) 
            {
                Hover.SetActive(true);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        counting = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        counting = false;
        Hover.SetActive(false);
        count = 0f;
    }
}
