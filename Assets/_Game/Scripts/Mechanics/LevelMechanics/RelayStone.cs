using Mechanics.WarpBolt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelayStone : MonoBehaviour, IWarpInteractable
{
    [SerializeField] private RelayStone _relayPair = null;

    public bool OnWarpBoltImpact(BoltData data)
    {
        // Redirect the warp bolt
        // adding some value to transform.position so that the bolt doesn't spawn inside the other relay stone and immediately collide
        data.WarpBolt.Redirect(_relayPair.transform.position + (_relayPair.transform.forward * 2), _relayPair.transform.rotation, 0);
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

    

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if(_relayPair != null)
            Gizmos.DrawLine(transform.position, _relayPair.transform.position);

        Gizmos.color = Color.red;
        Vector3 direction = transform.TransformDirection(Vector3.forward * 5);
        Gizmos.DrawRay(transform.position, direction);
    }
}
