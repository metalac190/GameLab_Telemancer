using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioSystem;
using UnityEngine.UI;

public class MenuMusicTrigger : MonoBehaviour
{
    [SerializeField] MusicEvent mainMenuMusic = null;
    [SerializeField] Button continueButton = null;
    [SerializeField] Button newGameButton = null;

    void Start()
    {
        continueButton.onClick.AddListener(OnContinue);
        newGameButton.onClick.AddListener(OnNewGame);

        if (mainMenuMusic != null)
        {
            mainMenuMusic.Play();
        }
    }

    void OnContinue()
    {
        mainMenuMusic.Stop();
    }

    void OnNewGame()
    {
        mainMenuMusic.Stop();
    }
}
