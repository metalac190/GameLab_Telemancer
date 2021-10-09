using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : LevelActivatable
{
    [Header("Door")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _openY = 0;
    [SerializeField] private float _closedY = 0;

    protected override void OnActivate()
    {
        StartCoroutine(MoveDoor(_openY, true));
    }

    protected override void OnDeactivate()
    {
        StartCoroutine(MoveDoor(_closedY, false));
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
        Debug.Log("Opening Door");
        while(opening == IsCurrentlyActive && transform.localPosition.y != target)
        {
            float newPos = Mathf.Lerp(transform.localPosition.y, target, Time.fixedDeltaTime * _moveSpeed);
            //transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * _moveSpeed);
            transform.localPosition = new Vector3(transform.localPosition.x, newPos, transform.localPosition.z);
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("Door open");
    }
}
