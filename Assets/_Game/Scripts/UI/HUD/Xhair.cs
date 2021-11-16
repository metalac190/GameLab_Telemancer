using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Handle crosshair logic and animations(?)
/// Might be replaced by a more complete UI script in the near future.
/// </summary>
public class Xhair : MonoBehaviour
{
    [SerializeField] private float _watchedOpacity = 0.19f;
    [SerializeField] private Image _xhair;

    private void Awake()
    {
        
    }

    private void Start()
    {
        UIEvents.current.OnPlayerWatched += UpdateXhairOpacity;
    }

    private void UpdateXhairOpacity(bool isWatched)
    {
        
    }
}
