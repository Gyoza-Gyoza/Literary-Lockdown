using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RewardScreen : MonoBehaviour
{
    public TextMeshProUGUI booksKilledText;
    public TextMeshProUGUI pagesEarnedText;
    private CanvasGroup canvasGroup;
    public float fadeInTiming = 1f;
    public float fadeOutTiming = 1f;

    public void SetBooksKilled(int input)
    {
        booksKilledText.text = input.ToString();
    }

    public void SetPagesEarned(int input)
    {
        pagesEarnedText.text = input.ToString();
    }


    public void ExitToMain()
    {
        Debug.Log("Exit to main called");
        SceneManager.LoadScene("Main");
    }

    #region ############# Fades ##################
    public void FadeIn()
    {
        StartCoroutine(FadeInCorutine(fadeInTiming));
    }
    IEnumerator FadeInCorutine(float timing)
    {
        float count = 0f;
        while (count <= timing)
        {
            count += Time.deltaTime;

            canvasGroup.alpha = count / timing;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        canvasGroup.alpha = 1f;
        yield break;
    }

    public void FadeOut()
    {
        StartCoroutine(FadeIOutorutine(fadeOutTiming));
    }


    IEnumerator FadeIOutorutine(float timing)
    {
        float count = 0f;
        while (count <= timing)
        {
            count += Time.deltaTime;

            canvasGroup.alpha = 1f -  (count / timing);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        canvasGroup.alpha = 0f;
        yield break;
    }
    #endregion
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.canvasGroup = this.GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        #region Debugging / Testing Inputs
        //if (Input.GetKeyDown(KeyCode.O))
        //{
        //    this.FadeOut();
        //}

        //if (Input.GetKeyDown(KeyCode.I))
        //{
        //    this.FadeIn();
        //}
        #endregion
    }
}
