using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Light _light;
    [SerializeField] private GameObject _respawnPoint;

    public event Action<Transform> OnCheckpointReached;

    private void Start()
    {
        _light.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            _light.enabled = true;
            OnCheckpointReached?.Invoke(_respawnPoint.transform);
        }
    }
}
