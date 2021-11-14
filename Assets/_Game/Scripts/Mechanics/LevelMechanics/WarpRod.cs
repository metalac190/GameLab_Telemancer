using System.Collections;
using System.Collections.Generic;
using Mechanics.Bolt;
using UnityEngine;
using AudioSystem;

public class WarpRod : WarpResidueInteractable
{
    [Header("Warp Rod")]
    [SerializeField] private GameObject _warpPad = null;
    [SerializeField] private Vector3 _teleportOffset = Vector3.up;

    [Header("Wisp")]
    [SerializeField] private bool _shootWisps = true;
    [SerializeField] GameObject _wispPrefab = null;
    private GameObject _wispReciever = null;

    [Header("Emission")]
    [SerializeField] private bool _useEmission = true;
    [SerializeField] private GameObject _crystal = null;
    [SerializeField] private float _baseIntensity = 100f;
    [SerializeField] private float _intensityMultiplier = 5f;
    [SerializeField] private float _emisionFrequency = 2f;
    private Material _crystalMat = null;
    private Color _crystalColor;

    private bool _isWispOnCooldown = false;

    [Header("SFX")]
    [SerializeField] SFXOneShot _warpRodActivationSFX = null;

    private void Start()
    {
        if (_crystal != null)
        {
            _crystalMat = _crystal.GetComponent<Renderer>().material;
            _crystalColor = _crystalMat.GetColor("_EmissiveColor");
        }
        if(_warpPad != null)
        {
            _wispReciever = _warpPad.GetComponent<WarpPad>().WispReciever;
        }
    }

    private void FixedUpdate()
    {
        float intensity = _intensityMultiplier * ((Mathf.Sin(Time.time * _emisionFrequency) + 1) / 2) + 1; // sin wave cycles through values between 0 and 1

        if (_useEmission)
        {
            _crystalMat?.SetColor("_EmissiveColor", _crystalColor * intensity * _baseIntensity);
        }

        if (intensity > (_intensityMultiplier + 0.99f) && !_isWispOnCooldown && _shootWisps && _wispPrefab != null) // shoot wisp at peak of sin wave
        {
            StartCoroutine(WispCooldown());
            Vector3 heading = (_wispReciever.transform.position - transform.position).normalized;
            Quaternion face = Quaternion.LookRotation(heading);
            GameObject wisp = Instantiate(_wispPrefab, _crystal.transform.position, face);
            wisp.GetComponent<ConnectionWisp>()._Target = _wispReciever.transform.position;
        }

    }

    public override bool OnWarpBoltImpact(BoltData data)
    {
        //Debug.Log("rodrodrodrodrod");
        if (_warpPad != null)
        {
            _warpRodActivationSFX.PlayOneShot(transform.position);
            data.PlayerController.TeleportToPosition(_warpPad.transform.position, _teleportOffset);
        }

        // Boolean return value to determine whether to dissipate warp bolt after impact
        return true;
    }

    IEnumerator WispCooldown()
    {
        _isWispOnCooldown = true;
        yield return new WaitForSeconds(0.15f);
        _isWispOnCooldown = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (_warpPad != null)
            Gizmos.DrawLine(transform.position, _warpPad.GetComponent<WarpPad>().WispReciever.transform.position);
    }
}