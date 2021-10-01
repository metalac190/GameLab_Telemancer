using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : LevelActivatable
{
    [Header("Door")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private Vector3 _openPosition = new Vector3();
    [SerializeField] private Vector3 _closedPosition = new Vector3();

    protected override void OnActivate()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnDeactivate()
    {
        throw new System.NotImplementedException();
    }
}
