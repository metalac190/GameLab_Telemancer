using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioSystem;

public class Door : LevelActivatable
{
    [Header("Door")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _openY = 0;
    [SerializeField] private float _closedY = 0;

    [Header("Audio")]
    [SerializeField] private SFXOneShot _openDoorSound = null;
    [SerializeField] private SFXOneShot _closeDoorSound = null;

    protected override void OnActivate()
    {
        if (transform.localPosition.y != _openY)
        {
            StartCoroutine(MoveDoor(_openY, true));
            _openDoorSound.PlayOneShot(transform.position);
        }
    }

    protected override void OnDeactivate()
    {
        if (transform.localPosition.y != _closedY)
        {
            StartCoroutine(MoveDoor(_closedY, false));
            _closeDoorSound.PlayOneShot(transform.position);
        }
    }

    protected override void OnReset()
    {
        if(!IsCurrentlyActive)
            transform.localPosition = new Vector3(transform.localPosition.x, _closedY, transform.localPosition.z);
        else
            transform.localPosition = new Vector3(transform.localPosition.x, _openY, transform.localPosition.z);
    }

    IEnumerator MoveDoor(float target, bool opening)
    {
        //Debug.Log("Opening Door");
        while(opening == IsCurrentlyActive && transform.localPosition.y != target)
        {
            float newPos = Mathf.Lerp(transform.localPosition.y, target, Time.fixedDeltaTime * _moveSpeed);
            transform.localPosition = new Vector3(transform.localPosition.x, newPos, transform.localPosition.z);
            yield return new WaitForEndOfFrame();
        }
        //Debug.Log("Door open");
    }
}
