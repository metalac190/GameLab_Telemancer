using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{
    private bool _levelKeyHeld = false;

    private void Update()
    {
        if(Keyboard.current.lKey.wasPressedThisFrame)
        {
            _levelKeyHeld = true;
        }

        if(_levelKeyHeld)
        {
            if(Keyboard.current.lKey.wasReleasedThisFrame)
            {
                _levelKeyHeld = false;
            }

            if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                LoadLevel(1);
            }

            if (Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                LoadLevel(2);
            }

            if (Keyboard.current.digit3Key.wasPressedThisFrame)
            {
                LoadLevel(3);
            }
        }
    }

    public void LoadLevel(int lvl)
    {
        PlayerPrefs.DeleteKey("Level1Time");
        PlayerPrefs.DeleteKey("Level2Time");
        PlayerPrefs.DeleteKey("Level3Time");
        Debug.Log("Loading Level " + lvl);
        SceneManager.LoadScene(lvl + 1);
    }

    public void Restartlevel(int lvl)
    {
        PlayerPrefs.DeleteKey("Level");
        PlayerPrefs.DeleteKey("Checkpoint");
        PlayerPrefs.Save();
        LoadLevel(lvl);
    }
}