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

    // Add code after build index updated to check which level so 
    // that you can potentially use a crossfade through Play() instead of Stop()
    void OnContinue()
    {
        mainMenuMusic.Stop();
    }

    // Consider using play once build index updated as well
    void OnNewGame()
    {
        mainMenuMusic.Stop();
    }
}
