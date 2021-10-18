using Mechanics.Bolt;

/// Summary:
/// Implement this interface for anything that should be interactable by the warp bolt
public interface IWarpInteractable
{
    // Called when the Warp Bolt impacts and residue is not unlocked
    // Returns whether the warp bolt should dissipate or not
    bool OnWarpBoltImpact(BoltData data);

    // Called when the Warp Bolt impacts and residue is unlocked
    // Return true to dissipate bolt and activate residue
    // Return false if the bolt should not activate residue and should not dissipate
    bool OnSetWarpResidue(BoltData data);

    // Called when the player activates Warp Residue
    // Typically just calls OnWarpBoltImpact() and OnDisableWarpResidue()
    void OnActivateWarpResidue(BoltData data);

    // Called when the Residue is canceled or deactivated
    void OnDisableWarpResidue();
}