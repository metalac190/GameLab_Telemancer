using Mechanics.WarpBolt;
using UnityEngine;

/// Summary:
/// Temporary Relay Stone Script. Mainly for reference and testing
/// Implements IWarpInteractable, should not allow for residue
public class Temp_RelayStone : MonoBehaviour, IWarpInteractable
{
    [SerializeField] private Transform _redirectionReference = null;

    public bool OnWarpBoltImpact(BoltData data)
    {
        // Redirect the warp bolt
        data.WarpBolt.Redirect(_redirectionReference, 0);

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
}