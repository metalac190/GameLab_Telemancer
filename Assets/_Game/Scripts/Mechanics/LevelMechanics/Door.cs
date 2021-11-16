using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioSystem;

public class Door : LevelActivatable
{
    [Header("Door")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _openY = -5.1f;
    [SerializeField] private float _closedY = 0;
    [SerializeField] private GameObject _door = null;

    [Header("Audio")]
    [SerializeField] private SFXOneShot _openDoorSound = null;
    [SerializeField] private SFXOneShot _closeDoorSound = null;

    protected override void OnActivate()
    {
        if (_door.transform.localPosition.y != _openY)
        {
            StartCoroutine(MoveDoor(_openY, true));
            _openDoorSound?.PlayOneShot(transform.position);
        }
    }

    protected override void OnDeactivate()
    {
        if (_door.transform.localPosition.y != _closedY)
        {
            StartCoroutine(MoveDoor(_closedY, false));
            _closeDoorSound?.PlayOneShot(transform.position);
        }
    }

    protected override void OnReset()
    {
        if(!IsCurrentlyActive)
            _door.transform.localPosition = new Vector3(_door.transform.localPosition.x, _closedY, _door.transform.localPosition.z);
        else
            _door.transform.localPosition = new Vector3(_door.transform.localPosition.x, _openY, _door.transform.localPosition.z);
    }

    IEnumerator MoveDoor(float target, bool opening)
    {
        Debug.Log("Opening Door");
        while(opening == IsCurrentlyActive && _door.transform.localPosition.y != target)
        {
            float newPos = Mathf.Lerp(_door.transform.localPosition.y, target, Time.fixedDeltaTime * _moveSpeed);
            _door.transform.localPosition = new Vector3(_door.transform.localPosition.x, newPos, _door.transform.localPosition.z);
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("Door open");
    }
}
