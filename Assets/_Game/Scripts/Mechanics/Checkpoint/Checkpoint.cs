using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Light _light;
    [SerializeField] private GameObject _respawnPoint;
    public Transform RespawnPoint => _respawnPoint.transform;

    public event Action<Transform> OnCheckpointReached;

    private void Awake()
    {
        _light.enabled = false;
    }

    private void Start()
    {
        UIEvents.current.OnRestartLevel += () => _light.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            OnCheckpointReached?.Invoke(RespawnPoint);
        }
    }

    public void EnableLight()
    {
        _light.enabled = true;
    }
}
