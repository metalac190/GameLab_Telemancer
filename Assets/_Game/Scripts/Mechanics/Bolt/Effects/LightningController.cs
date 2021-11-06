using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mechanics.Player;
using UnityEngine;

namespace Mechanics.Bolt.Effects
{
    /// Summary:
    /// The controller / animator for the lightning.
    /// Lerps the lightning position from the bolt to its full trail length
    public class LightningController : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _objToDisable = new List<GameObject>();
        [SerializeField] private List<Transform> _endpoints = new List<Transform>();
        [SerializeField] private List<Transform> _objToScale = new List<Transform>();

        private List<Vector3> _finalEndpoints;

        private void Awake()
        {
            _endpoints = _endpoints.Where(item => item != null).ToList();
            _objToScale = _objToScale.Where(item => item != null).ToList();

            _finalEndpoints = new List<Vector3>(_endpoints.Count);

            foreach (var point in _endpoints) {
                _finalEndpoints.Add(point.localPosition);
            }
        }

        public void SetLifetime(float delta)
        {
            float lightningSize = PlayerState.Settings.LightningSizeOverLife.Evaluate(delta);
            lightningSize = Mathf.Clamp01(lightningSize);
            for (int i = 0; i < _endpoints.Count; i++) {
                _endpoints[i].localPosition = Vector3.Lerp(Vector3.zero, _finalEndpoints[i], lightningSize);
            }
            float boltShellSize = PlayerState.Settings.BoltShellSizeOverLife.Evaluate(delta);
            boltShellSize = Mathf.Clamp01(boltShellSize);
            foreach (var obj in _objToScale) {
                obj.localScale = Vector3.one * boltShellSize;
            }
        }

        public void SetEffectActive(bool active)
        {
            foreach (var obj in _objToDisable) {
                obj.SetActive(active);
            }
        }
    }
}