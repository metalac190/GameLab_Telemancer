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