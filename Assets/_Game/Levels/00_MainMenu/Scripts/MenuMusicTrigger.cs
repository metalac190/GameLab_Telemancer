using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioSystem;
using UnityEngine.UI;

public class MenuMusicTrigger : MonoBehaviour
{
    [SerializeField] MusicEvent mainMenuMusic = null;
    [SerializeField] Button newGameButton = null;

    void Start()
    {
        newGameButton.onClick.AddListener(OnNewGame);

        if (mainMenuMusic != null)
        {
            mainMenuMusic.Play();
        }
    }

    void OnNewGame()
    {
        mainMenuMusic.Stop();
    }
}
