﻿using Mechanics.WarpBolt;
using UnityEngine;

/// Summary:
/// Temporary Large / Big Rock Script. Mainly for reference and testing
/// Inherits from Warp Residue Interactable, and overrides the WarpBoltImpact() to swap itself with the player
public class Temp_LargeRock : WarpResidueInteractable
{
    // A simple offset for the teleportation location
    [SerializeField] private Vector3 _teleportOffset = Vector3.up;

    /// Called on either WarpBoltImpact or WarpResidueActivated, see WarpResidueInteractable
    public override bool OnWarpBoltImpact(BoltData data)
    {
        // On the player controller, teleport this transform with an offset
        data.PlayerController.Teleport(transform, _teleportOffset);

        // Boolean return value to determine whether to dissipate warp bolt after impact
        return true;
    }

    // The other 3 functions can be inherited from for extra capabilities
}