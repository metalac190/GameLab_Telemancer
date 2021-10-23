using System.Collections;
using Mechanics.Player;
using UnityEngine;
using UnityEngine.VFX;

namespace Mechanics.Bolt.Effects
{
    public class BoltVfxController : MonoBehaviour
    {
        [SerializeField] private VisualEffect _effectToPlay = null;
        [SerializeField] private LightningController _lightning;
        [SerializeField] private bool _timeAliveIncludesFizzle = false;
        [SerializeField] [Range(0, 1)] private float _fizzlingDeltaRange = 1;

        private static string _timeAliveDelta = "timeAliveDelta";
        private static string _isFizzling = "isFizzling";
        private static string _fizzlingDelta = "fizzlingDelta";

        private float _airFizzleTime;

        private void Awake()
        {
            if (_lightning == null) {
                _lightning = GetComponent<LightningController>();
            }
        }

        public void SetLifetime(float timeAlive, float lifeSpan)
        {
            if (_timeAliveIncludesFizzle) {
                lifeSpan += PlayerState.Settings.BoltAirFizzleTime;
            }
            float delta = timeAlive / lifeSpan;
            delta = Mathf.Clamp01(delta);
            _effectToPlay.SetFloat(_timeAliveDelta, delta);
        }

        public void Dissipate(float dissipateTime)
        {
            if (_lightning != null) {
                _lightning.DissipateShrink();
            }
            if (_effectToPlay != null) {
                _effectToPlay.SetBool(_isFizzling, true);
                StartCoroutine(DissipateDeltaRoutine(dissipateTime * _fizzlingDeltaRange));
            }
        }

        private IEnumerator DissipateDeltaRoutine(float dissipateTime)
        {
            for (float t = dissipateTime; t > 0; t -= Time.deltaTime) {
                float delta = t / dissipateTime;
                _effectToPlay.SetFloat(_fizzlingDelta, delta);
                yield return null;
            }
            _effectToPlay.SetFloat(_fizzlingDelta, 0);
        }

        public void OnReset()
        {
            if (_effectToPlay != null) {
                _effectToPlay.SetBool(_isFizzling, false);
                _effectToPlay.SetFloat(_fizzlingDelta, 1);
            }
            if (_lightning != null) {
                _lightning.OnReset();
            }
        }
    }
}