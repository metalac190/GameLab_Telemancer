// Implement this interface for anything that should be interactable by the warp bolt

using Mechanics.WarpBolt;

public interface IWarpInteractable
{
    // Returns whether the warp bolt should dissipate or not
    bool OnWarpBoltImpact(BoltData data);

    // Return true to dissipate bolt and activate residue
    // Return false if the bolt should not activate residue but keep going instead
    bool OnSetWarpResidue(BoltData data);

    // Activate Warp Residue -- Same as OnWarpBoltImpact? Not Sure, msg Brandon if you have any thoughts
    void OnActivateWarpResidue(BoltData data);

    void OnDisableWarpResidue();
}