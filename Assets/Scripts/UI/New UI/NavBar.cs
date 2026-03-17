using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using UnityEngine.UI;

public class NavBar : MonoBehaviour
{
    [SerializeField] private NavBarScreen currentState;

    public Button companion;
    public Button raid;
    public Button shop;

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
                companion.image.sprite = companionSelected;
                raid.image.sprite = raidUnselected;
                shop.image.sprite = shopUnselected;

                companion.interactable = false;
                raid.interactable = true;
                shop.interactable = true;

                break;  

            case NavBarScreen.Raid:
                raid.image.sprite = raidSelected;
                companion.image.sprite = companionUnselected;
                shop.image.sprite = shopUnselected;

                companion.interactable = true;
                raid.interactable = false;
                shop.interactable = true;

                break;
            case NavBarScreen.Shop:
                shop.image.sprite = shopSelected;
                companion.image.sprite = companionUnselected;
                raid.image.sprite = raidUnselected;

                companion.interactable = true;
                raid.interactable = true;
                shop.interactable = false;

                break;
        }
    }

    public async void LoadScene(string sceneName)
    {
        if (NetworkManager.Singleton != null)
        {
            Destroy(NetworkManager.Singleton.gameObject);
        }

        if (FindFirstObjectByType<LobbyDetails>() != null)
        {
            Destroy(FindFirstObjectByType<LobbyDetails>().gameObject);
        }

        await SceneManager.LoadSceneAsync(sceneName);
    }
}
