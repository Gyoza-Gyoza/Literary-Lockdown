using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using UnityEngine.UI;

public class NavBar : MonoBehaviour
{
    [SerializeField] private NavBarScreen currentState;

    public Image companion;
    public Image raid;
    public Image shop;

    public Button companionButton;

    

    public Sprite companionSelected;
    public Sprite companionUnselected;

    public Sprite raidSelected;
    public Sprite raidUnselected;

    public Sprite shopSelected;
    public Sprite shopUnselected;
    public enum NavBarScreen {Companion, Raid, Shop}

    private void Start()
    {
        //companionButton.imag
        switch (currentState) 
        {
            case NavBarScreen.Companion:
                companion.sprite = companionSelected;
                raid.sprite = raidUnselected;
                shop.sprite = shopUnselected;
                break;  

            case NavBarScreen.Raid:
                raid.sprite = raidSelected;
                companion.sprite = companionUnselected;
                shop.sprite = shopUnselected;
                break;
            case NavBarScreen.Shop:
                shop.sprite = shopSelected;
                companion.sprite = companionUnselected;
                raid.sprite = raidUnselected;
                break;
        }
    }

    public async void LoadScene(string sceneName)
    {
        await SceneManager.LoadSceneAsync(sceneName);
    }
}
