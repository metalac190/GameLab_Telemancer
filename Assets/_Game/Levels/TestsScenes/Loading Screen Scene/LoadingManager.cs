using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] private Image _progressBar;
    
    private void Start()
    {
        int lvl = PlayerPrefs.GetInt("CurrentLevel", 2);
        StartCoroutine(LoadLevel(lvl));
    }

    private IEnumerator LoadLevel(int lvl)
    {
        AsyncOperation loadingLevel = SceneManager.LoadSceneAsync(lvl);

        while (loadingLevel.progress < 1)
        {
            _progressBar.fillAmount = loadingLevel.progress;
            yield return new WaitForEndOfFrame();
        }
    }
}
