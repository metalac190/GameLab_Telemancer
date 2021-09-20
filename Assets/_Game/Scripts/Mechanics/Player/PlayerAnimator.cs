using UnityEngine;

// The main Player Animator script that controls all animation for the player
// Mostly used by the Player Casting Script and the 'Player Interactions' Script

// NOTE: Each Animation Function returns how long that animation will take
public class PlayerAnimator : MonoBehaviour
{
    private float _fallTime;

    public float OnPetToad()
    {
        return 0;
    }

    public float OnStartle()
    {
        return 0;
    }

    public float OnJump()
    {
        return 0;
    }

    public float OnFall()
    {
        _fallTime = Time.time;
        return 0;
    }

    public float OnLand()
    {
        return 0;
    }

    public float OnKill()
    {
        return 0;
    }

    public float OnCastBolt()
    {
        return 0.2f;
    }

    public float OnResidueActive()
    {
        return 0;
    }

    public float OnInstantWarp()
    {
        return 0;
    }

    public float OnInteractableWarp()
    {
        return 0;
    }

    private void ResetToIdle()
    {
        // Resets all triggers and returns animation to idle
        // For instance, after warping or after landing a fall
    }
}