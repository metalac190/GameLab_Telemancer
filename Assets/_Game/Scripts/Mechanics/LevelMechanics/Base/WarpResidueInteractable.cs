using Mechanics.Bolt;
using UnityEngine;

/// Summary:
/// A generic script for Warp Residue Objects
/// Can be inherited by for objects that use warp residue
/// 
/// Requires a Residue Effect that accesses all MeshRenderers on child objects and applies the Residue Effect
[RequireComponent(typeof(ResidueEffect))]
public class WarpResidueInteractable : MonoBehaviour, IWarpInteractable
{
    private ResidueEffect _residueEffect;

    private void Awake()
    {
        _residueEffect = GetComponent<ResidueEffect>();
        if (_residueEffect == null) {
            _residueEffect = gameObject.AddComponent<ResidueEffect>();
        }
        _residueEffect.enabled = false;
    }

    /// Called on a NON-RESIDUE bolt impact
    public virtual bool OnWarpBoltImpact(BoltData data)
    {
        // Use data.PlayerController or data.WarpBolt to activate effects

        // Returns true to dissipate the bolt
        return true;
    }

    /// Called on a RESIDUE bolt impact
    public virtual bool OnSetWarpResidue(BoltData data)
    {
        // Activates the residue shader effects
        _residueEffect.enabled = true;
        // Returns true to dissipate the bolt
        return true;
    }

    /// Called when the player activates the warp residue
    public virtual void OnActivateWarpResidue(BoltData data)
    {
        // Activate effects like a normal Warp Bolt Impact
        OnWarpBoltImpact(data);
        // Disable the warp residue
        OnDisableWarpResidue();
    }

    /// Called when the warp residue is canceled or disabled
    public virtual void OnDisableWarpResidue()
    {
        // Disables the residue shader effects
        _residueEffect.enabled = false;
    }
}