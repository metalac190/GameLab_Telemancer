using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RestartLevelPrompt : MonoBehaviour
{
    [SerializeField] private Button _yesBtn, _noBtn = null;

    private void OnEnable()
    {
        _noBtn.Select();
    }
}