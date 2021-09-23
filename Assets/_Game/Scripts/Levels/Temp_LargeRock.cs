using Mechanics.WarpBolt;
using Mechanics.WarpResidue;
using UnityEngine;

// Temporary Large / Big Rock Script.
// With the IWarpInteractable interface, OnWarpBoltImpact is called when the warp bolt hits it
// It passes BoltData, which contains the PlayerController (to teleport) and the BoltController (to move / change warp bolt physics)
public class Temp_LargeRock : WarpResidueInteractable
{
    [SerializeField] private Vector3 _teleportOffset = Vector3.up;

    public override bool OnWarpBoltImpact(BoltData data)
    {
        // On the player controller, teleport this transform with an offset
        data.PlayerController.Teleport(transform, _teleportOffset);

        // Boolean return value to determine whether to dissipate warp bolt after impact
        return true;
    }

    // Activate Residue Effects on object
    public override bool OnSetWarpResidue(BoltData data)
    {
        base.OnSetWarpResidue(data);
        return true;
    }

    // Swap Player and Object or just call OnWarpBoltImpact() to not duplicate code
    public override void OnActivateWarpResidue(BoltData data)
    {
        base.OnActivateWarpResidue(data);
        OnWarpBoltImpact(data);
    }
}