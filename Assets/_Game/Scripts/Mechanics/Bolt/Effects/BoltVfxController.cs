using System.Collections;
using Mechanics.Player;
using UnityEngine;
using UnityEngine.VFX;

namespace Mechanics.Bolt.Effects
{
    public class BoltVfxController : MonoBehaviour
    {
        [SerializeField] private VisualEffect _effectToPlay = null;
        [SerializeField] private MeshRenderer _capsuleRenderer = null;
        [SerializeField] private LightningController _lightning;
        [SerializeField] private bool _timeAliveIncludesFizzle = false;
        [SerializeField] [Range(0, 1)] private float _fizzlingDeltaRange = 1;

        private static string _timeAliveDelta = "timeAliveDelta";
        private static string _isFizzling = "isFizzling";
        private static string _fizzlingDelta = "fizzlingDelta";

        private Coroutine _dissipateRoutine;

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
            if (_effectToPlay != null) {
                _effectToPlay.SetFloat(_timeAliveDelta, delta);
            }
            if (_lightning != null) {
                _lightning.SetLifetime(delta);
            }
            if (_capsuleRenderer != null) {
                _capsuleRenderer.material.SetFloat(_timeAliveDelta, delta);
            }
        }

        public void Dissipate(float dissipateTime)
        {
            if (_effectToPlay != null) {
                _effectToPlay.SetBool(_isFizzling, true);
                if (_dissipateRoutine != null) {
                    StopCoroutine(_dissipateRoutine);
                }
                _dissipateRoutine = StartCoroutine(DissipateDeltaRoutine(dissipateTime, _fizzlingDeltaRange));
            }
        }

        private IEnumerator DissipateDeltaRoutine(float dissipateTime, float range)
        {
            float timer = dissipateTime * range;
            for (float t = timer; t > 0; t -= Time.deltaTime) {
                float delta = t / timer;
                _effectToPlay.SetFloat(_fizzlingDelta, delta);
                yield return null;
            }
            _effectToPlay.SetFloat(_fizzlingDelta, 0);
            for (float t = 0; t < dissipateTime - timer; t += Time.deltaTime) {
                yield return null;
            }
            if (_lightning != null) {
                _lightning.SetEffectActive(false);
            }
        }

        public void OnReset()
        {
            if (_effectToPlay != null) {
                _effectToPlay.SetBool(_isFizzling, false);
                _effectToPlay.SetFloat(_fizzlingDelta, 1);
            }
            if (_capsuleRenderer != null) {
                _capsuleRenderer.material.SetFloat(_timeAliveDelta, 1);
            }
            if (_lightning != null) {
                _lightning.SetEffectActive(true);
            }
            if (_dissipateRoutine != null) {
                StopCoroutine(_dissipateRoutine);
            }
        }
    }
}