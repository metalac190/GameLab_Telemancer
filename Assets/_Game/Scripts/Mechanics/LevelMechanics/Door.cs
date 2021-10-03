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
        StartCoroutine(MoveDoor(_openPosition, true));
    }

    protected override void OnDeactivate()
    {
        StartCoroutine(MoveDoor(_closedPosition, false));
    }

    IEnumerator MoveDoor(Vector3 target, bool opening)
    {
        Debug.Log("Opening Door");
        while(opening == IsCurrentlyActive && transform.position != target)
        {
            transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * _moveSpeed);
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("Door open");
    }
}
