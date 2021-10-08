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
            SetCheckpoint(savedCkpt, _checkpoints[savedCkpt - 1].transform);
    }

    private void Start()
    {
        if (_spawnPoint != null) return;
        
        // If someone forgets to add a spawnpoint in the level, default to the player object's transform
        GameObject go = new GameObject();
        Transform tf = GameObject.FindGameObjectWithTag("Player").transform;
        go.transform.position = tf.position;
        go.transform.rotation = tf.rotation;
        RespawnPoint = go.transform;
        
        Debug.Log("SpawnPoint not set. Defaulting to player's position.");
    }

    private void SetCheckpoint(int ckptNumber, Transform spawn)
    {
        if (ckptNumber <= CurrentCheckpoint) return;
        
        RespawnPoint = spawn;
        CurrentCheckpoint = ckptNumber;
        
        PlayerPrefs.SetInt("Checkpoint", ckptNumber);
        PlayerPrefs.SetInt("Level", _currentLevel);
        PlayerPrefs.Save();
        
        Debug.Log("Checkpoint " + ckptNumber + " reached; Respawn point set to " + spawn.position);
    }
}
