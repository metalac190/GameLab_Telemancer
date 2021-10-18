using Mechanics.WarpBolt;
using UnityEngine;
using System;
using AudioSystem;

/// Summary:
/// Temporary Large / Big Rock Script. Mainly for reference and testing
/// Inherits from Warp Residue Interactable, and overrides the WarpBoltImpact() to swap itself with the player
public class LargeRock : WarpResidueInteractable
{
    // A simple offset for the teleportation location
    [SerializeField] private Vector3 _teleportOffset = Vector3.up;
    // the position of the object on start
    private Vector3 _spawnPos = new Vector3();
    private Quaternion _spawnRot = new Quaternion();

    [Header("Position Thresholds")]
    [SerializeField] private float MaxX = 100;
    [SerializeField] private float MaxY = 100;
    [SerializeField] private float MaxZ = 100;

    [Header("Audio")]
    [SerializeField] private SFXOneShot _impactSound = null;

    private Rigidbody _rb = null;

    private void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody>();
        _spawnPos = transform.position;
        _spawnRot = transform.rotation;
    }

    private void OnEnable()
    {
        UIEvents.current.OnPlayerRespawn += Reset;
    }

    private void OnDisable()
    {
        if(UIEvents.current != null)
            UIEvents.current.OnPlayerRespawn -= Reset;
    }

    private void Update()
    {
        Vector3 distance = transform.position - _spawnPos;
        if (Mathf.Abs(distance.x) > MaxX || Mathf.Abs(distance.y) > MaxY || Mathf.Abs(distance.z) > MaxZ)
            Reset();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Default"))
            _impactSound?.PlayOneShot(transform.position);
    }

    /// Called on either WarpBoltImpact or WarpResidueActivated, see WarpResidueInteractable
    public override bool OnWarpBoltImpact(BoltData data)
    {
        // On the player controller, teleport this transform with an offset
        data.PlayerController.Teleport(transform, _teleportOffset);

        // Boolean return value to determine whether to dissipate warp bolt after impact
        return true;
    }

    // The other 3 functions can be inherited from for extra capabilities

    public void Reset()
    {
        _rb.velocity = new Vector3(0, 0, 0);
        transform.position = _spawnPos;
        transform.rotation = _spawnRot;
    }
}