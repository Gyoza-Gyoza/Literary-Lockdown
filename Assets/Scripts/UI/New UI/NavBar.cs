using UnityEngine;
using UnityEngine.UI;

public class NavBar : MonoBehaviour
{
    [SerializeField] private NavBarScreen currentState;

    public Image companion;
    public Image raid;
    public Image shop;


    public Sprite companionSelected;
    public Sprite companionUnselected;

    public Sprite raidSelected;
    public Sprite raidUnselected;

    public Sprite shopSelected;
    public Sprite shopUnselected;
    public enum NavBarScreen {Companion, Raid, Shop}

    private void Start()
    {
        switch (currentState) 
        {
            case NavBarScreen.Companion:

                break;  

            case NavBarScreen.Raid:

                break;
            case NavBarScreen.Shop:

                break;
        }
    }
}
