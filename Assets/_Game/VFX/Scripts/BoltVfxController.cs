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

    /*
    private float _spawnRate = -1;

    public void SetRate(float delta)
    {
        if (_spawnRate < 0) {
            _spawnRate = _effectToPlay.GetFloat("Smoke Spawn Rate");
            Debug.Log(_spawnRate);
        }
        if (_effectToPlay != null) {
            _effectToPlay.SetFloat("Smoke Spawn Rate", _spawnRate * delta);
        }
    }
    */

    public void Dissipate(float dissipateTime)
    {
        if (_lightning != null) {
            _lightning.DissipateShrink();
        }
        if (_effectToPlay != null) {
            _effectToPlay.SetBool("isFizzling", true);
        }
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