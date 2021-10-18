using Mechanics.Bolt;
using Mechanics.Player;
using UnityEngine;
using UnityEngine.VFX;

public class BoltVfxController : MonoBehaviour
{
    [SerializeField] private VisualEffect _effectToPlay = null;
    [SerializeField] private LightningController _lightning;

    private void Awake()
    {
        if (_lightning == null) {
            _lightning = GetComponent<LightningController>();
        }
    }

    public float Dissipate()
    {
        if (_lightning != null) {
            _lightning.DissipateShrink();
        }
        if (_effectToPlay != null) {
            _effectToPlay.SetBool("isFizzling", true);
            return PlayerState.settings.dissipateTime;
        }
        return 0;
    }

    public void OnReset()
    {
        if (_effectToPlay != null) {
            _effectToPlay.SetBool("isFizzling", false);
        }
        if (_lightning != null) {
            _lightning.OnReset();
        }
    }
}