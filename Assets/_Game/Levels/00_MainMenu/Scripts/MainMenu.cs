using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void Awake()
    {
        // If no existing save, then hide the continue button
        // ContinueButton.SetActive(false);
    }

    public void NewGame()
    {
        // Reset all save data to default
        
        // Load first scene 
    }

    public void ContinueGame()
    {
        // Load scene from save data
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
