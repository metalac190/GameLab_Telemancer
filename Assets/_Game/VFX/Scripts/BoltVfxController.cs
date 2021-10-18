using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BoltVfxController : MonoBehaviour
{
    [SerializeField] private float _timeToDissipate = 0.5f;
    [SerializeField] private VisualEffect _effectToPlay = null;
    [SerializeField] private List<GameObject> _objsToDisable = new List<GameObject>();

    public float Dissipate()
    {
        if (_effectToPlay == null) return 0;

        _effectToPlay.SetBool("isFizzling", true);
        foreach (var obj in _objsToDisable) {
            obj.SetActive(false);
        }
        return _timeToDissipate;
    }

    public void Reset()
    {
        if (_effectToPlay != null) {
            _effectToPlay.SetBool("isFizzling", false);
        }
        foreach (var obj in _objsToDisable) {
            obj.SetActive(true);
        }
    }
}