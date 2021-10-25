﻿using System.Collections;
using System.Collections.Generic;
using Mechanics.Bolt;
using UnityEngine;

public class RelayStone : WarpResidueInteractable
{
    [Header("Relay Stone")]
    [SerializeField] private RelayStone _relayPair = null;

    [Header("Debuging")]
    [SerializeField] private float _trajectoryRayGizmo = 5;

    public override bool OnWarpBoltImpact(BoltData data)
    {
        // Redirect the warp bolt
        // adding some value to transform.position so that the bolt doesn't spawn inside the other relay stone and immediately collide
        
        data.BoltManager.RedirectBolt(_relayPair.transform.position, _relayPair.transform.rotation, 0);
        StartCoroutine(_relayPair.IgnoreCollisionWithBolt(data));
        //StartCoroutine(IgnoreCollisionWithBolt(data));
        // Don't dissipate the warp bolt!
        return false;
    }

    

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if(_relayPair != null)
            Gizmos.DrawLine(transform.position, _relayPair.transform.position);

        Gizmos.color = Color.red;
        Vector3 direction = transform.TransformDirection(Vector3.forward * _trajectoryRayGizmo);
        Gizmos.DrawRay(transform.position, direction);
    }

    
    public IEnumerator IgnoreCollisionWithBolt(BoltData data)
    {
        BoltController currentBolt = data.BoltManager.CurrentBolt;
        Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), currentBolt.Collider, true);
        yield return new WaitForSecondsRealtime(0.25f);
        Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), currentBolt.Collider, false);
    }
}
