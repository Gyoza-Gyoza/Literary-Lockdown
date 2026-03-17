using UnityEngine;

public class MainMenuUIManager:MonoBehaviour
{
    public void LoadScene(int index) => SaveLoadManager.Instance.LoadScene(index);
    public void LoadCompanionScene()
    {
        SaveLoadManager.Instance.LoadScene(2);
    }

    public void LoadShopScene()
    {
        SaveLoadManager.Instance.LoadScene(3);
    }

    public void LoadPlayScene()
    {
        SaveLoadManager.Instance.LoadScene(1);
    }
}
