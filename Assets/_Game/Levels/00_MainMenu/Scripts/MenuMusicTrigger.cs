using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioSystem;

public class MenuMusicTrigger : MonoBehaviour
{
    [SerializeField] MusicEvent mainMenuMusic = null;

    // Start is called before the first frame update
    void Start()
    {
        if (mainMenuMusic != null)
        {
            mainMenuMusic.Play();
        }
    }
}
