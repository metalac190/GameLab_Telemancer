using System.Collections;
using System.Collections.Generic;
using Mechanics.Bolt;
using UnityEngine;

public class DelayStone : MonoBehaviour, IWarpInteractable
{
    [Header("Delay Stone")]
    [SerializeField] private float _delayTime = 3;

    [Header("Debuging")]
    [SerializeField] private float _trajectoryRayGizmo = 5;

    public bool OnWarpBoltImpact(BoltData data)
    {
        // Redirect the warp bolt
        // adding some value to transform.position so that the bolt doesn't spawn inside the other relay stone and immediately collide
        data.BoltManager.RedirectBolt(gameObject, transform.position + (transform.forward * 2), transform.rotation, _delayTime);
        Debug.Log("bolt redirected");

        // Don't dissipate the warp bolt!
        return false;
    }

    public bool OnSetWarpResidue(BoltData data)
    {
        // Ignore Warp Residue, instead call WarpBoltImpact
        OnWarpBoltImpact(data);

        // Don't dissipate the warp bolt!
        return false;
    }

    // Warp Residue doesn't matter for relay stone
    public void OnActivateWarpResidue(BoltData data)
    {
    }

    public void OnDisableWarpResidue()
    {
    }

    public bool DoesResidueReturnToHoldAnimation()
    {
        return true;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 direction = transform.TransformDirection(Vector3.forward * _trajectoryRayGizmo);
        Gizmos.DrawRay(transform.position, direction);
    }
}