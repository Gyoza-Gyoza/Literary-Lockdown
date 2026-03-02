using TMPro;
using UnityEngine;

public class StartScreenManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pagesHeld;

    private void Start()
    {
        pagesHeld.text = ObjectivesManager.Instance.pageAmount.Value.ToString();
    }
}
