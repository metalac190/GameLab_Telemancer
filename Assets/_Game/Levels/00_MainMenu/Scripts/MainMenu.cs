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
    [SerializeField] private Button _continueButton;
    private bool hasSave;

    public void Awake()
    {
        // If no existing save, then hide the continue button
        // ContinueButton.SetActive(false);

        int savedLevel = PlayerPrefs.GetInt("Level");
        int savedCkpt = PlayerPrefs.GetInt("Checkpoint");

        hasSave = (savedLevel != 0 && savedCkpt != 0);
        _continueButton.interactable = hasSave;
        _continueButton.GetComponentInChildren<TextMeshProUGUI>().alpha = hasSave ? 1f : 0.4f;
    }

    private void Update()
    {
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
}