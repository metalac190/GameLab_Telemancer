using System.Collections;
using System.Collections.Generic;
using Mechanics.Bolt;
using UnityEngine;

public class WarpRod : WarpResidueInteractable
{
    [SerializeField] private GameObject _warpPad;
    [SerializeField] private Vector3 _teleportOffset = Vector3.up;

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

    /*
    public bool OnSetWarpResidue(BoltData data)
    {
        return true;
    }

    public void OnActivateWarpResidue(BoltData data)
    {
        OnWarpBoltImpact(data);
    }

    public void OnDisableWarpResidue() // Added as a hotfix
    {
        // hwat
    }
    */

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (_warpPad != null)
            Gizmos.DrawLine(transform.position, _warpPad.transform.position);
    }
}