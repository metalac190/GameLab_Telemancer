using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditsSequence : MonoBehaviour
{
    [SerializeField] private GameObject _titleCard, _designCard, _artCard, _soundCard, _progCard;
    [SerializeField] private Image _fadeImage;
    [SerializeField] private TMP_Text _speedrunResults;
    [SerializeField] private float _initialPause = 3;
    [SerializeField] private float _cardDuration = 10;
    [SerializeField] private float _fadeDuration = 0.3f;
    
    private void Start()
    {
        _titleCard.SetActive(true);
        _fadeImage.gameObject.SetActive(true);
        _fadeImage.color = new Color(0.8396f, 0.8396f, 0.8396f, 0f);
        _speedrunResults.enabled = PlayerPrefs.GetFloat("SpeedrunTimer", 0) == 1;
        GetTimerResults();
        
        StartCoroutine(Cutscene());
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            ExitToMainMenu();
    }

    private void GetTimerResults()
    {
        float lvl1 = PlayerPrefs.GetFloat("Level1Time", 0);
        float lvl2 = PlayerPrefs.GetFloat("Level2Time", 0);
        float lvl3 = PlayerPrefs.GetFloat("Level3Time", 0);
        float total = lvl1 + lvl2 + lvl3;
        string results = "";

        float min = Mathf.FloorToInt(lvl1 / 60);
        float sec = Mathf.FloorToInt(lvl1 % 60);
        float ms = (lvl1 % 1) * 1000;
        results += "Level 1 Time: ";
        results += lvl1 == 0 ? "--:--.---" : $"{min:00}:{sec:00}.{ms:000}";
        
        min = Mathf.FloorToInt(lvl2 / 60);
        sec = Mathf.FloorToInt(lvl2 % 60);
        ms = (lvl2 % 1) * 1000;
        results += "  |  Level 2 Time: ";
        results += lvl2 == 0 ? "--:--.---" : $"{min:00}:{sec:00}.{ms:000}";
        
        min = Mathf.FloorToInt(lvl3 / 60);
        sec = Mathf.FloorToInt(lvl3 % 60);
        ms = (lvl3 % 1) * 1000;
        results += "  |  Level 3 Time: ";
        results += lvl3 == 0 ? "--:--.---" : $"{min:00}:{sec:00}.{ms:000}";
        
        min = Mathf.FloorToInt(total / 60);
        sec = Mathf.FloorToInt(total % 60);
        ms = (total % 1) * 1000;
        results += "  |  Final Time: ";
        results += (lvl1 == 0 || lvl2 == 0 || lvl3 == 0) ? "incomplete" : $"{min:00}m {sec:00}s {ms:000}ms";

        _speedrunResults.text = results;
    }

    private IEnumerator Cutscene()
    {
        yield return new WaitForSecondsRealtime(_initialPause);

        float time = 0;
        while (time < _fadeDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, time / _fadeDuration);
            _fadeImage.color = new Color(0.8396f, 0.8396f, 0.8396f, alpha);
            
            time += Time.deltaTime;
            yield return null;
        }
        _fadeImage.color = new Color(0.8396f, 0.8396f, 0.8396f, 1f);
        
        // hide title card
        _titleCard.SetActive(false);
        // show design
        _designCard.SetActive(true);
        
        time = 0;
        while (time < _fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, time / _fadeDuration);
            _fadeImage.color = new Color(0.8396f, 0.8396f, 0.8396f, alpha);
            
            time += Time.deltaTime;
            yield return null;
        }
        _fadeImage.color = new Color(0.8396f, 0.8396f, 0.8396f, 0f);
        
        // pause for delay
        yield return new WaitForSecondsRealtime(_cardDuration);
        
        time = 0;
        while (time < _fadeDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, time / _fadeDuration);
            _fadeImage.color = new Color(0.8396f, 0.8396f, 0.8396f, alpha);
            
            time += Time.deltaTime;
            yield return null;
        }
        _fadeImage.color = new Color(0.8396f, 0.8396f, 0.8396f, 1f);
        
        // hide design
        _designCard.SetActive(false);
        // show art
        _artCard.SetActive(true);
        
        time = 0;
        while (time < _fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, time / _fadeDuration);
            _fadeImage.color = new Color(0.8396f, 0.8396f, 0.8396f, alpha);
            
            time += Time.deltaTime;
            yield return null;
        }
        _fadeImage.color = new Color(0.8396f, 0.8396f, 0.8396f, 0f);
        
        // pause for delay
        yield return new WaitForSecondsRealtime(_cardDuration);
        
        time = 0;
        while (time < _fadeDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, time / _fadeDuration);
            _fadeImage.color = new Color(0.8396f, 0.8396f, 0.8396f, alpha);
            
            time += Time.deltaTime;
            yield return null;
        }
        _fadeImage.color = new Color(0.8396f, 0.8396f, 0.8396f, 1f);
        
        // hide art
        _artCard.SetActive(false);
        // show product/sound
        _soundCard.SetActive(true);
        
        time = 0;
        while (time < _fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, time / _fadeDuration);
            _fadeImage.color = new Color(0.8396f, 0.8396f, 0.8396f, alpha);
            
            time += Time.deltaTime;
            yield return null;
        }
        _fadeImage.color = new Color(0.8396f, 0.8396f, 0.8396f, 0f);
        
        // pause for delay
        yield return new WaitForSecondsRealtime(_cardDuration);
        
        time = 0;
        while (time < _fadeDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, time / _fadeDuration);
            _fadeImage.color = new Color(0.8396f, 0.8396f, 0.8396f, alpha);
            
            time += Time.deltaTime;
            yield return null;
        }
        _fadeImage.color = new Color(0.8396f, 0.8396f, 0.8396f, 1f);
        
        // hide sound
        _soundCard.SetActive(false);
        // show programming
        _progCard.SetActive(true);
        
        time = 0;
        while (time < _fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, time / _fadeDuration);
            _fadeImage.color = new Color(0.8396f, 0.8396f, 0.8396f, alpha);
            
            time += Time.deltaTime;
            yield return null;
        }
        _fadeImage.color = new Color(0.8396f, 0.8396f, 0.8396f, 0f);
        
        // pause for delay
        yield return new WaitForSecondsRealtime(_cardDuration);
        
        time = 0;
        while (time < _fadeDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, time / _fadeDuration);
            _fadeImage.color = new Color(0.8396f, 0.8396f, 0.8396f, alpha);
            
            time += Time.deltaTime;
            yield return null;
        }
        _fadeImage.color = new Color(0.8396f, 0.8396f, 0.8396f, 1f);

        ExitToMainMenu();
    }

    private void ExitToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
