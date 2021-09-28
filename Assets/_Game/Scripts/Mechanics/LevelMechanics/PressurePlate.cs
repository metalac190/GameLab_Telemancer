using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private List<LevelActivatable> _activatables = new List<LevelActivatable>(); // the list of objects to be toggled by this pressure plate
    private int _id = 0;
    [SerializeField] private bool _isPressed = false;
    //private bool _

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
        if (!_isPressed)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("Warp Bolt"))
            {
                _isPressed = true;
                foreach (LevelActivatable obj in _activatables)
                {
                    if (obj != null) { obj.Toggle(_id); }
                }
                Debug.Log("pressure plate entered");
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Warp Bolt"))
        {
            _isPressed = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Warp Bolt"))
        {
            _isPressed = false;
            StartCoroutine(DeactivateOnCooldown());
            Debug.Log("pressure plate exited: " + other.gameObject.name);
        }
    }

    IEnumerator DeactivateOnCooldown()
    {
        yield return new WaitForSeconds(0.1f);
        if(_isPressed)
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
