using System.Collections;
using System.Collections.Generic;
using Mechanics.Bolt;
using UnityEngine;
using UnityEngine.VFX;

public class BoltVfxController : MonoBehaviour
{
    [SerializeField] private float _timeToDissipate = 0.5f;
    [SerializeField] private VisualEffect _effectToPlay = null;
    [SerializeField] private LightningController _lightning = null;

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

    public float Dissipate()
    {
        if (_lightning != null) {
            _lightning.DissipateShrink();
        }
        if (_effectToPlay != null) {
            _effectToPlay.SetBool("isFizzling", true);
            return _timeToDissipate;
        }
        return 0;
    }

    public void Reset()
    {
        if (_effectToPlay != null) {
            _effectToPlay.SetBool("isFizzling", false);
        }
        if (_lightning != null) {
            _lightning.Reset();
        }
    }
}