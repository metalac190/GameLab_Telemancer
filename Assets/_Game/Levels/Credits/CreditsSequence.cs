using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditsSequence : MonoBehaviour
{
    [SerializeField] private GameObject _titleCard, _designCard, _artCard, _soundCard, _progCard;
    [SerializeField] private Image _fadeImage;
    [SerializeField] private float _initialPause = 3;
    [SerializeField] private float _cardDuration = 10;
    [SerializeField] private float _fadeDuration = 0.3f;
    
    private void Start()
    {
        _titleCard.SetActive(true);
        _fadeImage.gameObject.SetActive(true);
        _fadeImage.color = new Color(0.8396f, 0.8396f, 0.8396f, 0f);
        StartCoroutine(Cutscene());
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            ExitToMainMenu();
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
