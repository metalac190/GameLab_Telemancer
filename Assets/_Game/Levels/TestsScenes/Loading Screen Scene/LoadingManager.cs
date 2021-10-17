using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] private Image _progressBar;
    [SerializeField] private GameObject[] _peomTxt;
    [SerializeField] private TextMeshProUGUI _helperText;
    
    private void Start()
    {
        _progressBar.fillAmount = 0f;
        int lvl = PlayerPrefs.GetInt("CurrentLevel", 2);

        for (int i = 0; i < _peomTxt.Length; i++)
        {
            // set the poem active for the level we're loading
            // (we need to offset the lvl value by one)
            _peomTxt[i].SetActive(i == lvl - 1);
        }
        
        StartCoroutine(LoadLevel(lvl));
    }

    private IEnumerator LoadLevel(int lvl)
    {
        AsyncOperation loadingLevel = SceneManager.LoadSceneAsync(lvl);
        loadingLevel.allowSceneActivation = false; // disable automatic scene activation

        while (!loadingLevel.isDone)
        {
            float progress = Mathf.Clamp01(loadingLevel.progress / .9f);
            //_progressBar.fillAmount = progress;
            yield return new WaitForEndOfFrame();
        }

        float hold = 0, time = 0, switchPoint = 0;
        float holdDuration = 2;
        bool prevStatus = false;
        
        while (hold < 1)
        {
            bool skipPressed = Keyboard.current.eKey.wasPressedThisFrame;

            if (prevStatus != skipPressed)
            {
                time = 0;
                switchPoint = hold;
            }
            
            // Hack fraud way of waiting for player input
            if (skipPressed)
            {
                hold = Mathf.Lerp(0, 1, time / holdDuration);
            }
            else
            {
                hold = Mathf.Lerp(switchPoint, 1, time / holdDuration / 3);
            }
            
            prevStatus = skipPressed;
            time += Time.deltaTime;

            _progressBar.fillAmount = hold;
            
            yield return null;
        }
        
        loadingLevel.allowSceneActivation = true; // begin scene activation
        yield return loadingLevel;
    }
}
