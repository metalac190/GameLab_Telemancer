using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningAdjuster : MonoBehaviour
{
    [SerializeField] Transform[] _endpoints = { };
    private Vector3[] _finalEndpoints;
    private Quaternion[] _finalEndrotations;
    //private Vector3 _posLastFrame = new Vector3(0, 0, 0);

    private float _timeStart;
    private float _timeStop;
    [SerializeField] float _duration;
    [SerializeField] float interpolator = 0f;

    private void OnEnable()
    {
        _timeStart = Time.time;
        _timeStop = Time.time + _duration;

        _finalEndpoints = new Vector3[_endpoints.Length];
        _finalEndrotations = new Quaternion[_endpoints.Length];

        if (interpolator == 0f) {
            for (int i = 0; i < _endpoints.Length; i++) {
                _finalEndpoints[i] = _endpoints[i].position;
                _finalEndrotations[i] = _endpoints[i].rotation;
            }
        }

        /*foreach (Transform pos in _endpoints)
        {
            pos.position = Vector3.zero;
            //pos.rotation = Quaternion.identity;
        }*/
    }

    private void Update()
    {
        interpolator = Mathf.Clamp01((Time.time - _timeStart) / (_timeStop - _timeStart));

        if (interpolator <= 1f) {
            for (int i = 0; i < _endpoints.Length; i++) {
                //_finalEndpoints[i] += (transform.position - _posLastFrame);

                Debug.Log(_finalEndpoints[i] + transform.position);

                _endpoints[i].position = Vector3.Lerp(transform.position, _finalEndpoints[i] + transform.position, interpolator);
                _endpoints[i].rotation = Quaternion.Slerp(transform.rotation, _finalEndrotations[i] * transform.rotation, interpolator);
            }
        }

        //_posLastFrame = transform.position;
    }

    private void OnDrawGizmos()
    {
        if (_finalEndpoints == null) return;
        Gizmos.color = Color.red;
        foreach (Vector3 pos in _finalEndpoints) {
            Gizmos.DrawSphere(pos + transform.position, 0.1f);
        }
    }
}