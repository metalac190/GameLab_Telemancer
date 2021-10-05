using UnityEngine;
using UnityEngine.VFX;

public class PlayerVfx : MonoBehaviour
{
    [SerializeField] private Transform _whereToFlash = null;
    [SerializeField] private VfxController _castFlash = null;

    private void OnEnable()
    {
        // Ensure that VFX is in scene (allows for prefab reference)
        if (_castFlash != null && !_castFlash.gameObject.activeInHierarchy) {
            Transform location = _whereToFlash != null ? _whereToFlash : transform;
            _castFlash = Instantiate(_castFlash, location);
        }
    }

    public void OnCastBolt()
    {
        if (_castFlash != null) {
            _castFlash.Play();
        }
    }
}