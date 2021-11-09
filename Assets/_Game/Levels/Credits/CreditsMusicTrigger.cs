using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioSystem;

public class CreditsMusicTrigger : MonoBehaviour
{
    [SerializeField] MusicEvent creditsMusic = null;
    AudioSource audioSource = null;

    void Start()
    {
        audioSource = MusicManager.Instance.GetComponentInChildren<AudioSource>();
        if (creditsMusic != null && !audioSource.isPlaying)
        {
            creditsMusic.Play();
        }
    }
}
