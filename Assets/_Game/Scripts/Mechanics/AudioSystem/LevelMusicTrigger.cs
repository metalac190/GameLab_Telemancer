using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelMusicTrigger : MonoBehaviour
{
    public UnityEvent OnTriggerMusic;
    public UnityEvent OnStopMusic;
    [SerializeField] Button deathButton;
    [SerializeField] Button restartButton;

    private void Start()
    {
        deathButton.onClick.AddListener(PlayerDeath);
        restartButton.onClick.AddListener(LevelRestart);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            OnTriggerMusic?.Invoke();
            GetComponent<Collider>().enabled = false;
        }
    }

    void PlayerDeath()
    {
        OnStopMusic?.Invoke();
    }

    void LevelRestart()
    {
        OnStopMusic?.Invoke();
    }
}
