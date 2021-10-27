using System.Collections;
using System.Collections.Generic;
using Mechanics.Bolt;
using UnityEngine;

public class WarpRodTestCopy : WarpResidueInteractable
{
    [SerializeField] private GameObject _warpPad = null;
    [SerializeField] private Vector3 _teleportOffset = Vector3.up;
    
    [Header("VFX")]
    [SerializeField] GameObject _wispPrefab = null;
    [SerializeField] private GameObject _warpPadWispTarget = null;
    [SerializeField] private GameObject _crystal = null;
    [SerializeField] private float _baseIntensity = 100f;
    [SerializeField] private float _intensityMultiplier = 5f;
    [SerializeField] private float _emisionFrequency = 2f;
    private Material _crystalMat = null;
    private Color _crystalColor;


    private void Start()
    {
        if (_crystal != null)
        {
            _crystalMat = _crystal.GetComponent<Renderer>().material;
            _crystalColor = _crystalMat.GetColor("_EmissiveColor");
        }
    }

    private void FixedUpdate()
    {
        float intensity = _intensityMultiplier * ((Mathf.Sin(Time.time * _emisionFrequency) + 1) / 2) + 1;
        //Debug.Log(intensity);
        _crystalMat?.SetColor("_EmissiveColor", _crystalColor * intensity * _baseIntensity);
        if(intensity > (_intensityMultiplier + 0.999f) && _wispPrefab != null)
        {
            Vector3 heading = (_warpPadWispTarget.transform.position - transform.position).normalized;
            Quaternion face = Quaternion.LookRotation(heading);
            GameObject wisp = Instantiate(_wispPrefab, _crystal.transform.position, face);
            wisp.GetComponent<ConnectionWisp>()._Target = _warpPadWispTarget.transform.position;
        }
        
    }

    public override bool OnWarpBoltImpact(BoltData data)
    {
        //Debug.Log("rodrodrodrodrod");
        if (_warpPad != null)
        {
            data.PlayerController.TeleportToPosition(_warpPad.transform.position, _teleportOffset);
        }

        // Boolean return value to determine whether to dissipate warp bolt after impact
        return true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (_warpPad != null)
            Gizmos.DrawLine(transform.position, _warpPad.transform.position);
    }
}