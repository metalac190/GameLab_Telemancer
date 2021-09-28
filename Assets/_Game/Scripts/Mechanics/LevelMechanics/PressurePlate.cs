using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private List<LevelActivatable> _activatables = new List<LevelActivatable>(); // the list of objects to be toggled by this pressure plate
    private int _id = 0;

    private void Awake()
    {
        _id = gameObject.GetInstanceID();
    }

    private void Start()
    {
        foreach(LevelActivatable obj in _activatables)
        {
            if(obj != null) { obj.AddToSwitches(_id); }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Warp Bolt"))
        {
            foreach (LevelActivatable obj in _activatables)
            {
                if (obj != null) { obj.Toggle(_id); }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Warp Bolt"))
        {
            foreach (LevelActivatable obj in _activatables)
            {
                if (obj != null) { obj.Toggle(_id); }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        foreach (LevelActivatable obj in _activatables)
        {
            if (obj != null) { Gizmos.DrawLine(transform.position, obj.transform.position); }
        }
    }
}
