using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button _continueButton = null;
    [SerializeField] private GameObject _submenu = null;
    private bool hasSave;
    [SerializeField] Image fade = null;
    [SerializeField] float fadeDuration = 3;
    Color transparent = new Color(0, 0, 0, 0);
    Color opaque = new Color(0, 0, 0, 1);

    public void Awake()
    {
        // If no existing save, then hide the continue button
        // ContinueButton.SetEffectActive(false);

        /*
        int savedLevel = PlayerPrefs.GetInt("Level");
        int savedCkpt = PlayerPrefs.GetInt("Checkpoint");

        hasSave = (savedLevel != 0 && savedCkpt != 0);
        _continueButton.interactable = hasSave;
        _continueButton.GetComponentInChildren<TextMeshProUGUI>().alpha = hasSave ? 1f : 0.4f;
        */
        StartCoroutine(FadeToTransparent());
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame && _submenu.activeSelf)
            _submenu.SetActive(false);
        
        if (Keyboard.current.f5Key.wasPressedThisFrame) {
            PlayerPrefs.DeleteKey("Level");
            PlayerPrefs.DeleteKey("Checkpoint");
            PlayerPrefs.Save();
            SceneManager.LoadScene(0);
        } else if (Keyboard.current.f6Key.wasPressedThisFrame) {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            SceneManager.LoadScene(0);
        }
    }

    public void NewGame()
    {
        // OnReset all save data to default

        // Load first scene 
        //SceneManager.LoadScene(1);

        // alternate method using loading screen
        PlayerPrefs.DeleteKey("Level1Time");
        PlayerPrefs.DeleteKey("Level2Time");
        PlayerPrefs.DeleteKey("Level3Time");
        PlayerPrefs.SetInt("CurrentLevel", 2);
        PlayerPrefs.SetInt("Checkpoint", 1);
        PlayerPrefs.SetInt("Level", 2);
        PlayerPrefs.Save();
        SceneManager.LoadScene(1);
    }

    public void ContinueGame()
    {
        // Load scene from save data
        if (hasSave)
            SceneManager.LoadScene(PlayerPrefs.GetInt("Level"));
    }

    public void LoadCredits()
    {
        // Load credits scene
        SceneManager.LoadScene(5);
    }

    public void OpenOptionsMenu()
    {
        // Fire OpenOptionsMenu event
        UIEvents.current.OpenOptionsMenu();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private IEnumerator FadeToTransparent()
    {
        float time = 0;
        fade.color = opaque;
        yield return new WaitForEndOfFrame();

        while (time < fadeDuration)
        {
            fade.color = Color.Lerp(opaque, transparent, time / fadeDuration);
            time += Time.deltaTime;
            yield return null;
        }
    }
}