using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioSystem;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class AudioTester : MonoBehaviour
{
    [SerializeField] MusicEvent songA;
    [SerializeField] MusicEvent songB;
    [SerializeField] MusicEvent songC;

    [SerializeField] int decreaseMusicLayerTransitionTime = 0;
    [SerializeField] int increaseMusicLayerTransitionTime = 0;
    [SerializeField] int setMusicLayerNumber = 0;
    [SerializeField] int setMusicLayerTransitionTime = 0;

    private void Update()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            songA.Play();
        }
        if (Keyboard.current.wKey.wasPressedThisFrame)
        {
            songB.Play();
        }
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            songC.Play();
        }
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            MusicManager.Instance.DecreaseLayerIndex(decreaseMusicLayerTransitionTime);
        }
        if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            MusicManager.Instance.IncreaseLayerIndex(increaseMusicLayerTransitionTime);
        }
        if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            MusicManager.Instance.StopMusic();
        }
        if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            MusicManager.Instance.SetLayerIndex(setMusicLayerNumber,
                setMusicLayerTransitionTime);
        }
    }
}
