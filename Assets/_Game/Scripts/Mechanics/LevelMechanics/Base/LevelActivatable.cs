using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelActivatable : MonoBehaviour
{
    [Header("LevelActivatable")]
    [SerializeField] private bool _isActiveOnStartup = false;
    [SerializeField] private bool _isCurentlyActive;
    public bool IsCurrentlyActive { get => _isCurentlyActive; }

    private void Awake()
    {
        _isCurentlyActive = _isActiveOnStartup;
    }

    public void Toggle()
    {
        _isCurentlyActive = !_isCurentlyActive;
        if (_isCurentlyActive)
            OnActivate();
        else
            OnDeactivate();
    }

    protected abstract void OnActivate();
    protected abstract void OnDeactivate();
}
