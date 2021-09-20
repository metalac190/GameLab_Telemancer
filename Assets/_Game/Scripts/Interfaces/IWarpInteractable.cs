﻿// Implement this interface for anything that should be interactable by the warp bolt

using Mechanics.WarpBolt;

public interface IWarpInteractable
{
    // Returns whether the warp bolt should dissipate or not
    bool OnWarpBoltImpact(BoltData data);
}