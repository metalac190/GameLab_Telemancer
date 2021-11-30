using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;

public class StatsMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _fpsValue = null;
    [SerializeField] private TextMeshProUGUI _speedometerValue = null;
    [SerializeField] private TextMeshProUGUI _timerValue = null, _timerMillisecondsValue = null;
    [SerializeField] private TextMeshProUGUI _positionValue = null;
    [SerializeField] private TextMeshProUGUI _boltCounterValue = null;
    private CharacterController _player;

    private int _framesPerSecond;
    private float _playerSpeed;
    private Vector3 _playerVelocity;
    private float _currentTime;
    private Vector3 _playerPosition;
    private int _boltsFired;


    private int frameCount = 0;
    private double dt = 0.0;
    private double fps = 0.0;
    private double updateRate = 4.0;
    

    private bool _timerRunning = false;
    private bool _playerMoved = false;
    private bool _gamePaused = false;

    private bool _hasMovementAchievement;

    private void Start()
    {
        // add listeners
        //UIEvents.current.OnCastBolt += UpdatePlayerMoved;
        UIEvents.current.OnRestartLevel += ResetTimer;
        UIEvents.current.OnAcquireWarpScroll += StopTimer;
        UIEvents.current.OnAcquireResidueScroll += StopTimer;
        UIEvents.current.OnAcquireGameEndScroll += StopTimer;
        UIEvents.current.OnPauseGame += b => _gamePaused = b;

        UIEvents.current.OnShowFpsCounter += ShowFPS;
        UIEvents.current.OnShowSpeedometer += ShowSpeedometer;
        UIEvents.current.OnShowTimer += ShowTimer;
        
        // reset colors
        _fpsValue.color = Color.white;
        _timerValue.color = Color.white;
        _timerMillisecondsValue.color = Color.white;
        _speedometerValue.color = Color.white;
        
        // find player object
        _player = FindObjectOfType<CharacterController>();
        if (_player == null)
            _positionValue.text = "[Player Not Found]";
        
        // check if the player already has the movement achievement
        _hasMovementAchievement = AchievementManager.current.isUnlocked(
            AchievementManager.Achievements.Reach14Speed);
    }

    private void Awake()
    {
        // call GetFPS once per second
        //InvokeRepeating(nameof(GetFPS), 1, 1); 
        
        // update elements
        DisplayTimer();
        //UpdateBoltCounter(false);
    }

    private void Update()
    {
        GetFPS();
        
        // update timer
        if (_timerRunning || _gamePaused)
        {
            _currentTime += Time.deltaTime;
            DisplayTimer();
        }

        // update speedometer
        _playerVelocity = _player.velocity;
        _playerVelocity.y = 0;
        _speedometerValue.text = _playerVelocity.magnitude.ToString("F2") + "<size=50%> m/s";
        if (!_hasMovementAchievement && _playerVelocity.magnitude >= 14)
            AchievementManager.current.unlockAchievement(AchievementManager.Achievements.Reach14Speed);
        
        /*
        // update position
        _playerPosition = _player.transform.position;
        _positionValue.text = "X:" + _playerPosition.x.ToString("F3") 
                                   + "\nY:" + _playerPosition.y.ToString("F3") 
                                   + "\nZ:" + _playerPosition.z.ToString("F3");
        */
        
        // Start the timer if the player has pressed any button
        if (!_playerMoved) 
        {
            InputSystem.onAnyButtonPress.CallOnce(ctrl =>
            {
                _playerMoved = true;
                _timerRunning = true;
                //Debug.Log($"Button {ctrl} was pressed");
            });
        }

    }

    public void ShowSpeedometer(bool isShown)
    {
        _speedometerValue.gameObject.GetComponentInParent<Canvas>().enabled = isShown;
    }

    public void ShowTimer(bool isShown)
    {
        _timerValue.gameObject.GetComponentInParent<Canvas>().enabled = isShown;
    }

    public void ShowFPS(bool isShown)
    {
        _fpsValue.gameObject.GetComponentInParent<Canvas>().enabled = isShown;
    }

    // TODO: Fixme, currently called by Reset Level button. Should reset on reload level but not on respawn
    public void ResetTimer()
    {
        _currentTime = 0;
        _timerRunning = false;
        _playerMoved = false;
    }

    // Stop the speedrun timer and check for speedrun achievements
    private void StopTimer()
    {
        _timerRunning = false;
        AchievementManager.current.CheckSpeedrunTime(_currentTime);
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
        _timerValue.text = $"<mspace=0.6em>{min:00}</mspace>:<mspace=0.6em>{sec:00}</mspace>";
        _timerMillisecondsValue.text = $".<mspace=0.6em>{ms:000}</mspace>";
    }

    private void GetFPS()
    {
        frameCount++;
        dt += Time.deltaTime;
        if (dt > 1.0/updateRate)
        {
            fps = frameCount / dt ;
            frameCount = 0;
            dt -= 1.0/updateRate;
            
            _fpsValue.text = Math.Floor(fps) + " <size=70%>FPS";
            
            // This could be optimized
            if (fps < 60)
                _fpsValue.color = Color.red;
            else if (fps >= 120)
                _fpsValue.color = Color.green;
            else
                _fpsValue.color = Color.white;
        }
        
    }
}
