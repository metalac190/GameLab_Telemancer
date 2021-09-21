using System.Collections.Generic;
using UnityEngine;

namespace Mechanics.WarpBolt.Effects
{
    public class LightningController : MonoBehaviour
    {
        [SerializeField] List<Transform> _endpoints = new List<Transform>();

        [SerializeField] private float _duration = 4;

        private List<Vector3> _finalEndpoints;

        private float _timer0;
        private float _timer1;

        private void Awake()
        {
            _finalEndpoints = new List<Vector3>(_endpoints.Count);

            foreach (var point in _endpoints) {
                _finalEndpoints.Add(point.localPosition);
            }
        }

        private void OnEnable()
        {
            _timer0 = 0;
            _timer1 = 0;
        }

        private void Update()
        {
            if (_timer1 > _duration) return;

            _timer0 += Time.deltaTime;
            _timer1 += _timer0 / 10;
            float delta = _timer1 / _duration;

            for (int i = 0; i < _endpoints.Count; i++) {
                _endpoints[i].localPosition = Vector3.Lerp(Vector3.zero, _finalEndpoints[i], delta);
                //Debug.Log(_endpoints[i].localPosition + " " + delta + " " + _finalEndpoints[i]);
            }
        }
    }
}