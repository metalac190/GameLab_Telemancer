using System.Collections;
using Mechanics.Player;
using UnityEngine;

namespace Mechanics.Bolt.Effects
{
    public class BoltLowQualityEffect : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _capsuleRenderer = null;
        [SerializeField] private bool _timeAliveIncludesFizzle = false;
        [SerializeField] private Transform _objToScale = null;

        private static string _timeAliveDelta = "timeAliveDelta";

        public void SetLifetime(float timeAlive, float lifeSpan)
        {
            if (_timeAliveIncludesFizzle) {
                lifeSpan += PlayerState.Settings.BoltAirFizzleTime;
            }
            float delta = timeAlive / lifeSpan;
            delta = Mathf.Clamp01(delta);
            if (_capsuleRenderer != null) {
                _capsuleRenderer.material.SetFloat(_timeAliveDelta, delta);
            }
            if (_objToScale != null) {
                float boltShellSize = PlayerState.Settings.BoltShellSizeOverLife.Evaluate(delta);
                boltShellSize = Mathf.Clamp01(boltShellSize);
                _objToScale.localScale = Vector3.one * boltShellSize;
            }
        }

        public void Dissipate(float dissipateTime)
        {
        }

        public void OnReset()
        {
            if (_capsuleRenderer != null) {
                _capsuleRenderer.material.SetFloat(_timeAliveDelta, 1);
            }
        }
    }
}