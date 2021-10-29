﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelMusicTrigger : MonoBehaviour
{
    public UnityEvent OnTriggerMusic;
    public UnityEvent OnStopMusic;

    private void Awake()
    {
        UIEvents.current.OnQuitToMenu += Test;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            OnTriggerMusic?.Invoke();
            GetComponent<Collider>().enabled = false;
        }
    }

    void Test()
    {
        Time.timeScale = 1f;
        OnStopMusic?.Invoke();
    }
}
