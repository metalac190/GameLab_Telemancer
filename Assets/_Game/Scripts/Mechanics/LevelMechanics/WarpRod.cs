using Mechanics.WarpBolt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpRod : MonoBehaviour, IWarpInteractable
{
    [SerializeField] private GameObject _warpPad;
    [SerializeField] private Vector3 _teleportOffset = Vector3.up;

    public bool OnWarpBoltImpact(BoltData data)
    {
        data.PlayerController.Teleport(_warpPad.transform, _teleportOffset);

        // Boolean return value to determine whether to dissipate warp bolt after impact
        return true;
    }

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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, _warpPad.transform.position);
    }
}
