using Mechanics.WarpBolt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelayStone : MonoBehaviour, IWarpInteractable
{
    [SerializeField] private RelayStone _relayPair;

    public void OnActivateWarpResidue(BoltData data)
    {
        throw new System.NotImplementedException();
    }

    public void OnDisableWarpResidue()
    {
        throw new System.NotImplementedException();
    }

    public bool OnSetWarpResidue(BoltData data)
    {
        throw new System.NotImplementedException();
    }

    public bool OnWarpBoltImpact(BoltData data)
    {
        throw new System.NotImplementedException();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, _relayPair.transform.position);

        Gizmos.color = Color.red;
        //Ray ray = new Ray(transform.position, transform.forward * 5);
        Vector3 direction = transform.TransformDirection(Vector3.forward * 5);
        Gizmos.DrawRay(transform.position, direction);
    }
}
