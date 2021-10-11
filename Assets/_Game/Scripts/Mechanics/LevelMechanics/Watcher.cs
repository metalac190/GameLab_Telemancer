using System.Collections;
using System.Collections.Generic;
using Mechanics.Player;
using UnityEngine;

public class Watcher : MonoBehaviour
{
    [Header("Watcher")]
    [SerializeField] private float _viewRadius = 5f;
    [SerializeField] [Range(1, 179)] private float _viewAngle = 75f;
    [SerializeField] private bool _DeactivateBolt = true;
    [SerializeField] private bool _DeactivateWarp = false;
    [SerializeField] private bool _DeactivateResidue = false;

    [Header("Object References DO NOT CHANGE")]
    [SerializeField] private GameObject _visionSource = null;
    [SerializeField] private Light _visionLight = null;
    [SerializeField] SphereCollider _trigger = null;
    [SerializeField] LayerMask targetMask;
    [SerializeField] LayerMask obstacleMask;

    private bool _isPlayerInRange = false;
    private bool _isPlayerInView = false;

    private PlayerState _playerState;

    private void Start()
    {
        _playerState = FindObjectOfType<PlayerState>();

        _trigger.center = _visionSource.transform.localPosition;
        _trigger.radius = _viewRadius;

        _visionLight.range = _viewRadius;
        _visionLight.spotAngle = _viewAngle;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player Trigger"))
        {
            _isPlayerInRange = true;
            StartCoroutine(RunCheckForPlayer());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player Trigger"))
        {
            _isPlayerInRange = false;
            if(_isPlayerInView)
            {
                _isPlayerInView = false;
                OnPlayerExitedView();
            }
        }
    }

    IEnumerator RunCheckForPlayer()
    {
        while(_isPlayerInRange)
        {
            CheckForPlayerInView();
            yield return new WaitForFixedUpdate();
        }
    }

    private void CheckForPlayerInView()
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(_visionSource.transform.position, _viewRadius, targetMask);

        for(int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - _visionSource.transform.position).normalized;
            if (Vector3.Angle(_visionSource.transform.forward, dirToTarget) < _viewAngle / 2)
            {
                float distToTarget = Vector3.Distance(_visionSource.transform.position, target.position);
                if(!Physics.Raycast(_visionSource.transform.position, dirToTarget, distToTarget, obstacleMask))
                {
                    if(!_isPlayerInView)
                    {
                        _isPlayerInView = true;
                        OnPlayerEnteredView();
                    }
                }
                else if(_isPlayerInView)
                {
                    _isPlayerInView = false;
                    OnPlayerExitedView();
                }
            }
            else if (_isPlayerInView)
            {
                _isPlayerInView = false;
                OnPlayerExitedView();
            }
        }
    }

    private void OnPlayerEnteredView()
    {
        //Debug.Log("Player in view");
        if(_DeactivateBolt)
            _playerState.SetBoltUnlock(false);
        if (_DeactivateWarp)
            _playerState.SetWarpUnlock(false);
        if (_DeactivateResidue)
            _playerState.SetResidueUnlock(false);
    }

    private void OnPlayerExitedView()
    {
        //Debug.Log("Player Not in view");
        if (_DeactivateBolt)
            _playerState.SetBoltUnlock(true);
        if (_DeactivateWarp)
            _playerState.SetWarpUnlock(true);
        if (_DeactivateResidue)
            _playerState.SetResidueUnlock(true);
    }

    private Vector3 DirFromAnglesXZ(float angleInDegrees, bool angleIsGlobal)
    {
        if(!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(_visionSource.transform.position, _viewRadius);
        Vector3 viewAnglesA = DirFromAnglesXZ(_viewAngle / 2, false);
        Vector3 viewAnglesB = DirFromAnglesXZ(-_viewAngle / 2, false);
        Gizmos.DrawLine(_visionSource.transform.position, _visionSource.transform.position + viewAnglesA * _viewRadius);
        Gizmos.DrawLine(_visionSource.transform.position, _visionSource.transform.position + viewAnglesB * _viewRadius);
    }

    private void OnDrawGizmosSelected()
    {
        _trigger.center = _visionSource.transform.localPosition;
        _trigger.radius = _viewRadius;

        _visionLight.range = _viewRadius;
        _visionLight.spotAngle = _viewAngle;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_visionSource.transform.position, _viewRadius);
    }
}
