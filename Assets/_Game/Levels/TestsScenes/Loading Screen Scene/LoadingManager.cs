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
    
    [SerializeField] private float _textFadeIn = 1, _holdDuration = 2;
    [SerializeField] private GameObject _progressBarContainer = null;
    [SerializeField] private Image _progressBar = null;
    [SerializeField] private TextMeshProUGUI _helperText = null;
    [SerializeField] private GameObject[] _peomTxt = {};
    
    private void Start()
    {
        _progressBar.fillAmount = 0f;
        _progressBarContainer.SetActive(false);
        _helperText.text = "Loading...";
        _helperText.alpha = 0f;
        int lvl = PlayerPrefs.GetInt("CurrentLevel", 2);

        for (int i = 0; i < _peomTxt.Length; i++)
        {
            // set the poem active for the level we're loading
            // (we need to offset the lvl value by two)
            _peomTxt[i].SetActive(i == lvl - 2);
        }
        
        StartCoroutine(LoadLevel(lvl));
    }

    private IEnumerator LoadLevel(int lvl)
    {
        AsyncOperation loadingLevel = SceneManager.LoadSceneAsync(lvl);
        loadingLevel.allowSceneActivation = false; // disable automatic scene activation
        
        bool prevStatus = false;
        
        while (loadingLevel.progress < 0.9f)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame && !prevStatus)
            {
                StartCoroutine(FadeInHelperText());
                prevStatus = true;
            }

            float progress = Mathf.Clamp01(loadingLevel.progress / .9f);
            _helperText.text = "Loading... " + (int)(progress * 100) + "%";
            //_progressBar.fillAmount = progress;
            yield return new WaitForEndOfFrame();
        }
        
        // Fade in text if it wasn't already
        if (!prevStatus)
            StartCoroutine(FadeInHelperText());
        
        prevStatus = false; // reset prevStatus
        _helperText.text = "Hold [E] to Continue"; // Set helper text to skip prompt
        _progressBarContainer.SetActive(true); // show progressbar

        // Init new vars
        float hold = 0, time = 0, switchPoint = 0;
        float holdDuration = _holdDuration;
        
        // Wait for hold to skip
        while (hold < 1)
        {
            // Hack fraud way of waiting for player input
            bool skipPressed = Keyboard.current.eKey.isPressed;

            if (prevStatus != skipPressed)
            {
                time = 0;
                switchPoint = hold;
            }

            // Adjust time
            var t = time / holdDuration;
            t = 1f - Mathf.Cos(t * Mathf.PI * 0.5f); // ease in
            //t = t*t * (3f - 2f*t); // smooth step
            
            // Lerp to either 1 or 0 based on if skip is held
            hold = skipPressed ? Mathf.Lerp(switchPoint, 1, t) : Mathf.Lerp(switchPoint, 0, time / holdDuration * 2);
            
            prevStatus = skipPressed;
            time += Time.deltaTime;

            _progressBar.fillAmount = hold;
            
            yield return null;
        }
        
        loadingLevel.allowSceneActivation = true; // begin scene activation
        yield return loadingLevel;
    }

    private IEnumerator FadeInHelperText()
    {
        float time = 0;
        while (time < _textFadeIn)
        {
            _helperText.alpha = Mathf.Lerp(0, 1, time / _textFadeIn);
            time += Time.deltaTime;
            yield return null;
        }
        _helperText.alpha = 1;
    }
}
