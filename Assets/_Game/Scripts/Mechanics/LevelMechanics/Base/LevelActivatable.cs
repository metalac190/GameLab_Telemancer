using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class LevelActivatable : MonoBehaviour
{
    [Header("LevelActivatable")]
    [SerializeField] private bool _isActiveOnStartup = false;
    private bool _isCurentlyActive;
    public bool IsCurrentlyActive { get => _isCurentlyActive; }

    private enum logicalOperators {AND, OR };
    [SerializeField] private logicalOperators _logicOperator = logicalOperators.AND;
    private Dictionary<int, bool> _switches = new Dictionary<int, bool>();
    /*
     * I feel like there must be a better way of doing this, but this was the most concise I could think of.
     * This dictionay system allows for an activatable to have some unknown number of pressure plates (or other kind of switch if we make more)
     * working on it so that you can require multiple switches to be on to activate something. (for example, having 4 pressure plates to open 1 door).
     * There may be solutions that are more effiecient in runtime, but this system makes it very easy for the level designers, all they have to
     * do is plug the activatable into the pressure plate and it just works.
     * 
     * use AND for "all switches must be active", use OR for "at least one switch must be active"
     */


    private void Awake()
    {
        _isCurentlyActive = _isActiveOnStartup;
        if (_isActiveOnStartup)
            OnActivate();
        else
            OnDeactivate();
    }

    private void OnEnable()
    {
        UIEvents.current.OnPlayerRespawn += OnPlayerRespawn;
    }

    private void OnDisable()
    {
        if (UIEvents.current != null)
            UIEvents.current.OnPlayerRespawn -= OnPlayerRespawn;
    }

    // any pressure plate or switch should run this on start so the activatable is aware the switch exists
    // id needs to be some unique identifier, I've been using gameobject.GetInstanceID()
    public void AddToSwitches(int id)
    {
        _switches.Add(id, _isActiveOnStartup);
    }

    // id needs to be some unique identifier, I've been using gameobject.GetInstanceID()
    public void Toggle(int id)
    {
        _switches[id] = !_switches[id];
        bool wasActive = _isCurentlyActive;
        
        if(_logicOperator == logicalOperators.AND)
        {
            bool bitChecker = true;
            foreach(int key in _switches.Keys)
            {
                bitChecker = (bitChecker && _switches[key]); // if any key is false, bitChecker will be false
            }
            _isCurentlyActive = bitChecker;
        }
        else if(_logicOperator == logicalOperators.OR)
        {
            bool bitChecker = false;
            foreach (int key in _switches.Keys)
            {
                bitChecker = (bitChecker || _switches[key]); // if any key is true, bitChecker will be true
            }
            _isCurentlyActive = bitChecker;
        }

        if (wasActive != _isCurentlyActive)
        {
            if (_isCurentlyActive)
                OnActivate();
            else
                OnDeactivate();
        }

        //Debug.Log("Switch " + id + " = " + _switches[id]);
    }

    protected abstract void OnActivate();
    protected abstract void OnDeactivate();
    protected abstract void OnReset();


    private void OnPlayerRespawn()
    {
        _isCurentlyActive = _isActiveOnStartup;
        OnReset();
    }
}
