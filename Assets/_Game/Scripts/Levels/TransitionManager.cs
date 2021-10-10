using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager tm;
    [SerializeField] Image fade;
    float fadeDuration = 1;
    string chapterNumber, title = "";
    Color transparent = new Color(0, 0, 0, 0);
    Color opaque = new Color(0, 0, 0, 1);

    private void Awake()
    {
        if (tm == null)
        {
            tm = this;
            DontDestroyOnLoad(gameObject);
        } 
        else { Destroy(gameObject); }
    }

    public void ChangeLevel(int lvl)
    {
        // PlayerPrefs.SetInt("CurrentLevel", lvl);
        // PlayerPrefs.Save();
        // SceneManager.LoadScene(1);
        StartCoroutine(LoadLevel(lvl));
    }

    private IEnumerator LoadLevel(int lvl)
    {
        StartCoroutine(FadeToBlack());
        yield return new WaitForSeconds(fadeDuration);
        AsyncOperation loadingLevel = SceneManager.LoadSceneAsync(lvl);
        while (!loadingLevel.isDone)
        {
            yield return null;
        }
        StartCoroutine(FadeToTransparent());
        yield return new WaitForSeconds(fadeDuration);

        switch (lvl)
        {
            case 2:
                chapterNumber = "CHAPTER I";
                title = "A New Sandbox";
                break;

            case 3:
                chapterNumber = "CHAPTER II";
                title = "The Sand Boxes Back";
                break;

            default:
                chapterNumber = "";
                title = "";
                break;
        }
        // UIEvents.current.NotifyChapter(chapterNumber, title);
    }

    private IEnumerator FadeToTransparent()
    {
        float time = 0;

        while (time < fadeDuration)
        {
            fade.color = Color.Lerp(opaque, transparent, time / fadeDuration);
            time += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator FadeToBlack()
    {
        float time = 0;

        while (time < fadeDuration)
        {
            fade.color = Color.Lerp(transparent, opaque, time / fadeDuration);
            time += Time.deltaTime;
            yield return null;
        }
    }
}
