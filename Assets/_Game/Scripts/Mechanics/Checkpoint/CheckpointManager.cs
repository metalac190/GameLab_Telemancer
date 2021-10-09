using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager current;
    
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Checkpoint[] _checkpoints; // ordered list of checkpoints
    
    [HideInInspector] public int CurrentCheckpoint;
    [HideInInspector] public Transform RespawnPoint;

    private int _currentLevel;

    private void Awake()
    {
        current = this;
        
        _currentLevel = SceneManager.GetActiveScene().buildIndex;
        
        RespawnPoint = _spawnPoint;
    }

    private void Start()
    {
        // listen for UIEvents
        UIEvents.current.OnRestartLevel += RestartLevel;
        
        // find all the checkpoints and start listening
        for (var i = 0; i < _checkpoints.Length; i++)
        {
            var ckptNumber = i + 1;
            _checkpoints[i].OnCheckpointReached += tf => SetCheckpoint(ckptNumber, tf);
        }
        
        // load saved progress
        int savedLevel = PlayerPrefs.GetInt("Level");
        int savedCkpt = PlayerPrefs.GetInt("Checkpoint");
        if (savedLevel == _currentLevel && _checkpoints[savedCkpt - 1] != null)
        {
            Transform rp = _checkpoints[savedCkpt - 1].RespawnPoint;
            SetCheckpoint(savedCkpt, rp);
            FindObjectOfType<PlayerController>().TeleportToPosition(rp.position); // yes, I know it's suboptimal
        }

        // If someone forgets to add a spawnpoint in the level, default to the player object's transform
        if (_spawnPoint != null) return;
        
        GameObject go = new GameObject();
        Transform temp_tf = GameObject.FindGameObjectWithTag("Player").transform;
        go.transform.position = temp_tf.position;
        go.transform.rotation = temp_tf.rotation;
        _spawnPoint = go.transform; // This is so we can reference spawnPoint in RestartLevel()
        RespawnPoint = _spawnPoint;
        
        Debug.Log("SpawnPoint not set. Defaulting to player's position.");
    }

    private void SetCheckpoint(int ckptNumber, Transform spawn)
    {
        // don't re-activate previous or current checkpoint
        if (ckptNumber <= CurrentCheckpoint) return;
        
        RespawnPoint = spawn;
        CurrentCheckpoint = ckptNumber;
        
        _checkpoints[ckptNumber - 1]?.EnableLight(); // turn on light
        
        // set player prefs
        PlayerPrefs.SetInt("Checkpoint", ckptNumber);
        PlayerPrefs.SetInt("Level", _currentLevel);
        PlayerPrefs.Save();
        
        Debug.Log("Checkpoint " + ckptNumber + " reached; Respawn point set to " + spawn.position);
    }

    private void RestartLevel()
    {
        CurrentCheckpoint = 0;
        RespawnPoint = _spawnPoint;
        
        PlayerPrefs.DeleteKey("Checkpoint");
        PlayerPrefs.DeleteKey("Level");
        PlayerPrefs.Save();
        
        PlayerController pc = FindObjectOfType<PlayerController>();
        pc.TeleportToPosition(RespawnPoint.position);
        pc.GetComponentInParent<Transform>().rotation = RespawnPoint.rotation;
        
        UIEvents.current.PauseGame(false);
    }
}
