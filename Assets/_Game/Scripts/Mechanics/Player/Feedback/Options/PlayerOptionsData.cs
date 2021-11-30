using UnityEngine;

namespace Mechanics.Player.Feedback.Options
{
    public class PlayerOptionsData : ScriptableObject
    {
        public bool IsCodeActive;
        public bool Invincibility;
        public bool InfiniteJumps;
        public bool NoBoltCooldown;
        public bool NoWarpCooldown;
        public bool NoResidueCooldown;
        public bool InfiniteBoltDistance;
        public float BoltMoveSpeed;
    }
}
