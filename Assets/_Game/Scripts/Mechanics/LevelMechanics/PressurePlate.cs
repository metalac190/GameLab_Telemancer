using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private List<LevelActivatable> _activatables; // the list of objects to be toggled by this pressure plate

    private void OnTriggerEnter(Collider other)
    {
        foreach (LevelActivatable obj in _activatables)
        {
            obj.Toggle();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        foreach (LevelActivatable obj in _activatables)
        {
            obj.Toggle();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        foreach (LevelActivatable obj in _activatables)
        {
            Gizmos.DrawLine(transform.position, obj.transform.position);
        }
    }
}
