using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioSystem;

public class AudioTester : MonoBehaviour
{
    [SerializeField] MusicEvent songA;
    [SerializeField] MusicEvent songB;
    [SerializeField] MusicEvent songC;

    [SerializeField] SFXOneShot soundA;
    [SerializeField] SFXLoop soundB;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            songA.Play();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            songB.Play();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            songC.Play();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            soundA.PlayOneShot(transform.position);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            soundB.Play(transform.position);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MusicManager.Instance.DecreaseLayerIndex(5);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MusicManager.Instance.IncreaseLayerIndex(5);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MusicManager.Instance.StopMusic(1);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            MusicManager.Instance.SetLayerIndex(3, 5f);
        }
    }
}
