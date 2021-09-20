using Mechanics.WarpBolt;
using UnityEngine;

// Temporary Large / Big Rock Script.
// With the IWarpInteractable interface, OnWarpBoltImpact is called when the warp bolt hits it
// It passes BoltData, which contains the PlayerController (to teleport) and the BoltController (to move / change warp bolt physics)
public class Temp_LargeRock : MonoBehaviour, IWarpInteractable
{
    [SerializeField] private Vector3 _teleportOffset = Vector3.up;

    public bool OnWarpBoltImpact(BoltData data)
    {
        // On the player controller, teleport this transform with an offset
        data.PlayerController.Teleport(transform, _teleportOffset);

        // Boolean return value to determine whether to dissipate warp bolt after impact
        return true;
    }
}