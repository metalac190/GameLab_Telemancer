using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

namespace Mechanics.Bolt.Effects
{
    public class BoltVfxController : MonoBehaviour
    {
        [SerializeField] private VisualEffect _effectToPlay = null;
        [SerializeField] private LightningController _lightning;
        [SerializeField] [Range(0, 1)] private float _fizzlingDeltaRange = 1;

        private void Awake()
        {
            if (_lightning == null) {
                _lightning = GetComponent<LightningController>();
            }
        }

        private void SetFizzling(bool fizzling)
        {
            _effectToPlay.SetBool("isFizzling", fizzling);
        }

        private void SetFizzlingDelta(float delta)
        {
            _effectToPlay.SetFloat("fizzlingDelta", delta);
        }

        public void Dissipate(float dissipateTime)
        {
            if (_lightning != null) {
                _lightning.DissipateShrink();
            }
            if (_effectToPlay != null) {
                SetFizzling(true);
                StartCoroutine(DissipateDeltaRoutine(dissipateTime * _fizzlingDeltaRange));
            }
        }

        private IEnumerator DissipateDeltaRoutine(float dissipateTime)
        {
            for (float t = dissipateTime; t > 0; t -= Time.deltaTime) {
                float delta = t / dissipateTime;
                SetFizzlingDelta(delta);
                yield return null;
            }
            SetFizzlingDelta(0);
        }

        public void OnReset()
        {
            if (_effectToPlay != null) {
                SetFizzling(false);
                SetFizzlingDelta(1);
            }
            if (_lightning != null) {
                _lightning.OnReset();
            }
        }
    }
}