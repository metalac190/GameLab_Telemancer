using Mechanics.WarpBolt;
using UnityEngine;
using System;

/// Summary:
/// Temporary Large / Big Rock Script. Mainly for reference and testing
/// Inherits from Warp Residue Interactable, and overrides the WarpBoltImpact() to swap itself with the player
public class Temp_LargeRock : WarpResidueInteractable
{
    // A simple offset for the teleportation location
    [SerializeField] private Vector3 _teleportOffset = Vector3.up;
    // the position of the object on start
    private Vector3 _spawnPos = new Vector3();
    private Quaternion _spawnRot = new Quaternion();

    private Rigidbody _rb = null;

    private void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody>();
        _spawnPos = transform.position;
        _spawnRot = transform.rotation;
    }

    private void OnEnable()
    {
        UIEvents.current.OnPlayerRespawn += OnPlayerRespawn;
    }

    private void OnDisable()
    {
        if(UIEvents.current != null)
            UIEvents.current.OnPlayerRespawn -= OnPlayerRespawn;
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

    public void OnPlayerRespawn()
    {
        _rb.velocity = new Vector3(0, 0, 0);
        transform.position = _spawnPos;
        transform.rotation = _spawnRot;
    }
}