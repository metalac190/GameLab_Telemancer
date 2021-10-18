using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioSystem;

public class PressurePlate : MonoBehaviour
{
    [Header("Pressure Plate")]
    [SerializeField] private List<LevelActivatable> _activatables = new List<LevelActivatable>(); // the list of objects to be toggled by this pressure plate
    private int _id = 0;
    private bool _isPressed = false; // if two things are on the pressure plate at once, don't want to double activate it

    [Header("Button Depression")]
    [SerializeField] private GameObject _movingButton = null;
    [SerializeField] private float _buttonOnPos = -0.001f;
    [SerializeField] private float _buttonOffPos = 0.0004f;
    [SerializeField] private float _buttonMoveSpeed = 2f;

    [Header("Audio")]
    [SerializeField] private SFXOneShot _pressurePlateDownSound = null;
    [SerializeField] private SFXOneShot _pressurePlateUpSound = null;

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
            if (other.gameObject.layer != LayerMask.NameToLayer("Warp Bolt") && 
                other.gameObject.layer != LayerMask.NameToLayer("Ground Detector") && 
                other.gameObject.layer != LayerMask.NameToLayer("Player"))
            {
                _isPressed = true;
                foreach (LevelActivatable obj in _activatables)
                {
                    if (obj != null) { obj.Toggle(_id); }
                }
                Debug.Log("pressure plate entered: " + other.gameObject.name);
                StartCoroutine(MoveButton(_buttonOnPos, true));
            }
        }
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Warp Bolt") &&
                other.gameObject.layer != LayerMask.NameToLayer("Ground Detector") &&
                other.gameObject.layer != LayerMask.NameToLayer("Player"))
        {
            if (!_isPressed)
            {
                _isPressed = true;
                foreach (LevelActivatable obj in _activatables)
                {
                    if (obj != null) { obj.Toggle(_id); }
                }
                StartCoroutine(MoveButton(_buttonOnPos, true));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Warp Bolt") &&
                other.gameObject.layer != LayerMask.NameToLayer("Ground Detector") &&
                other.gameObject.layer != LayerMask.NameToLayer("Player"))
        {
            if (_isPressed)
            {
                _isPressed = false;
                foreach (LevelActivatable obj in _activatables)
                {
                    if (obj != null) { obj.Toggle(_id); }
                }
            }
            //StartCoroutine(DeactivateOnCooldown());
            Debug.Log("pressure plate exited: " + other.gameObject.name);
            StartCoroutine(MoveButton(_buttonOffPos, false));
        }
        
    }
    
    IEnumerator MoveButton(float newZ, bool isActive)
    {
        if (_isPressed)
            _pressurePlateDownSound?.PlayOneShot(transform.position);
        else
            _pressurePlateUpSound?.PlayOneShot(transform.position);

        while(isActive == _isPressed && newZ != _movingButton.transform.localPosition.z)
        {
            float newPos = Mathf.Lerp(_movingButton.transform.localPosition.z, newZ, Time.fixedDeltaTime * _buttonMoveSpeed);
            _movingButton.transform.localPosition = new Vector3(_movingButton.transform.localPosition.x, _movingButton.transform.localPosition.x, newPos);
            yield return new WaitForFixedUpdate();
        }
    }
    

    /*
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
    */

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        foreach (LevelActivatable obj in _activatables)
        {
            if (obj != null) { Gizmos.DrawLine(transform.position, obj.transform.position); }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if(_movingButton != null)
            _movingButton.transform.localPosition = new Vector3(_movingButton.transform.localPosition.x, _movingButton.transform.localPosition.x, _buttonOffPos);
    }
}
