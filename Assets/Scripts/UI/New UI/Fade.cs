using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Fade : MonoBehaviour
{
    static Fade instance;

    CanvasGroup canvasGroup;
    public float fadeOut = .25f;
    public float fadeIn = .125f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else Destroy(gameObject);

        canvasGroup = GetComponent<CanvasGroup>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float count = fadeIn;

        while (count >= 0)
        {
            count -= Time.deltaTime;
            canvasGroup.alpha = count / fadeIn;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        canvasGroup.alpha = 0;

        yield break;
    }


    public void StartFadeOut(string scenename, bool isAsync) { StartCoroutine(FadeOut(scenename, isAsync)); }

    IEnumerator FadeOut(string scenename, bool isAsync)
    {
        float count = 0;

        while (count <= fadeOut)
        {
            count += Time.deltaTime;
            canvasGroup.alpha = count / fadeOut;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        canvasGroup.alpha = 1;

        if (isAsync)
        {
            SceneManager.LoadSceneAsync(scenename);
        }
        else
        {
            SceneManager.LoadScene(scenename);
        }

        yield break;
    }
}
