using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _fpsValue;
    [SerializeField] private TextMeshProUGUI _speedometerValue;
    [SerializeField] private TextMeshProUGUI _timerValue;
    [SerializeField] private TextMeshProUGUI _positionValue;
    [SerializeField] private TextMeshProUGUI _boltCounterValue;
    private CharacterController _player;

    private int _framesPerSecond;
    private float _playerSpeed;
    private float _currentTime;
    private Vector3 _playerPosition;
    private int _boltsFired;

    private bool _timerRunning = true;

    private void Start()
    {
        // add listeners
        UIEvents.current.OnCastBolt += UpdateBoltCounter;
        
        // reset colors
        _fpsValue.color = Color.white;
        _timerValue.color = Color.white;
        _speedometerValue.color = Color.white;
        
        // find player object
        _player = FindObjectOfType<CharacterController>();
        if (_player == null)
            _positionValue.text = "[Player Not Found]";
    }

    private void Awake()
    {
        // call GetFPS once per second
        InvokeRepeating(nameof(GetFPS), 1, 1); 
        
        // update elements
        DisplayTimer();
        UpdateBoltCounter(false);
    }

    private void Update()
    {
        // update timer
        if (_timerRunning)
        {
            _currentTime += Time.deltaTime;
            DisplayTimer();
        }
        
        // update speedometer
        _speedometerValue.text = _player.velocity.magnitude.ToString("F2") + " ups";
        
        // update position
        _playerPosition = _player.transform.position;
        _positionValue.text = "X:" + _playerPosition.x.ToString("F3") 
                                   + "  Y:" + _playerPosition.y.ToString("F3") 
                                   + "  Z:" + _playerPosition.z.ToString("F3");

    }

    // TODO: Fixme, currently called by Reset Level button. Should reset on reload level but not on respawn
    public void ResetTimer()
    {
        _currentTime = 0;
    }

    private void UpdateBoltCounter(bool boltSucceeded)
    {
        if (boltSucceeded)
            _boltsFired++;

        _boltCounterValue.text = "Bolts fired: " + _boltsFired;
    }

    private void DisplayTimer()
    {
        float min = Mathf.FloorToInt(_currentTime / 60);
        float sec = Mathf.FloorToInt(_currentTime % 60);
        float ms = (_currentTime % 1) * 1000;
        _timerValue.text = $"{min:00}:{sec:00}:{ms:000}";
    }

    private void GetFPS()
    {
        _framesPerSecond = (int)(1f / Time.unscaledDeltaTime);
        _fpsValue.text = _framesPerSecond + "";
        
        // This could be optimized
        if (_framesPerSecond < 60)
            _fpsValue.color = Color.red;
        else if (_framesPerSecond >= 120)
            _fpsValue.color = Color.green;
        else
            _fpsValue.color = Color.white;
    }
}
