using Mechanics.Bolt.Effects;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerVfx : MonoBehaviour
{
    [SerializeField] private Transform _whereToFlash = null;
    [SerializeField] private VfxController _castFlash;
    [SerializeField] private VfxController _dustEnvironment;

    private void OnEnable()
    {
        // Ensure that VFX is in scene (allows for prefab reference)
        if (_castFlash != null && !_castFlash.gameObject.activeInHierarchy) {
            Transform location = _whereToFlash != null ? _whereToFlash : transform;
            _castFlash = Instantiate(_castFlash, location);
        }
        if (_dustEnvironment != null && !_dustEnvironment.gameObject.activeInHierarchy) {
            _dustEnvironment = Instantiate(_dustEnvironment, transform);
        }
    }

    public void OnBoltReady()
    {
    }

    public void OnBoltUsed()
    {
        if (_castFlash != null) {
            _castFlash.Play();
        }
    }

    public void OnWarpReady()
    {
    }

    public void OnWarpUsed()
    {
    }

    public void OnResidueReady()
    {
    }

    public void OnResidueUsed()
    {
    }
}