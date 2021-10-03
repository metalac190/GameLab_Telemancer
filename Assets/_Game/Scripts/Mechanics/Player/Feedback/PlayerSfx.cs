using AudioSystem;
using UnityEngine;

public class PlayerSfx : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private SFXOneShot _activateWarpSound = null;
    [SerializeField] private SFXOneShot _boltCastSound = null;
    [SerializeField] private SFXOneShot _activateResidueSound = null;
    [SerializeField] private SFXOneShot _objectImpactResidueSound = null;


    public void CastBolt()
    {
        if (_boltCastSound != null) {
            _boltCastSound.PlayOneShot(transform.position);
        }
    }

    public void ActivateWarp()
    {
        if (_activateWarpSound != null) {
            _activateWarpSound.PlayOneShot(transform.position);
        }
    }

    public void ResidueReady()
    {
        if (_objectImpactResidueSound != null) {
            _objectImpactResidueSound.PlayOneShot(transform.position);
        }
    }

    public void ActivateResidue()
    {
        if (_activateResidueSound != null) {
            _activateResidueSound.PlayOneShot(transform.position);
        }
    }
}