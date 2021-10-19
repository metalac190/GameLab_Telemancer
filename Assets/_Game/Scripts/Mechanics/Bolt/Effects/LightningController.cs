using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using UnityEngine;

namespace Mechanics.Bolt
{
    /// Summary:
    /// The controller / animator for the lightning.
    /// Lerps the lightning position from the bolt to its full trail length
    public class LightningController : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _objToDisable = new List<GameObject>();
        [SerializeField] private List<Transform> _endpoints = new List<Transform>();

        [SerializeField] private float _growDuration = 4;
        [SerializeField] private float _growDrag = 10;
        [SerializeField] private float _shrinkDuration = 3;
        [SerializeField] private float _shrinkDrag = 8;

        private float _delta;
        private Coroutine _routine;

        private List<Vector3> _finalEndpoints;

        private void Awake()
        {
            _endpoints = _endpoints.Where(item => item != null).ToList();

            _finalEndpoints = new List<Vector3>(_endpoints.Count);

            foreach (var point in _endpoints) {
                _finalEndpoints.Add(point.localPosition);
            }
        }

        public void Reset()
        {
            _delta = 0;
        }

        private void OnEnable()
        {
            SpawnGrow();
        }

        private void SpawnGrow()
        {
            if (_routine != null) {
                StopCoroutine(_routine);
            }
            foreach (var obj in _objToDisable) {
                obj.SetActive(true);
            }
            _routine = StartCoroutine(Grow());
        }

        private IEnumerator Grow()
        {
            float vel = 0;
            _delta = Mathf.Clamp01(_delta);
            while (_delta < 1) {
                for (int i = 0; i < _endpoints.Count; i++) {
                    _endpoints[i].localPosition = Vector3.Lerp(Vector3.zero, _finalEndpoints[i], _delta);
                }

                vel += Time.deltaTime / _growDuration;
                _delta += vel / _growDrag;
                yield return null;
            }
            for (int i = 0; i < _endpoints.Count; i++) {
                _endpoints[i].localPosition = _finalEndpoints[i];
            }
        }

        public void DissipateShrink()
        {
            if (_routine != null) {
                StopCoroutine(_routine);
            }
            _routine = StartCoroutine(Shrink());
        }

        private IEnumerator Shrink()
        {
            float vel = 0;
            _delta = Mathf.Clamp(_delta, 0, _shrinkDuration);
            while (_delta > 0) {
                for (int i = 0; i < _endpoints.Count; i++) {
                    _endpoints[i].localPosition = Vector3.Lerp(Vector3.zero, _finalEndpoints[i], _delta);
                }
                vel += Time.deltaTime / _shrinkDuration;
                _delta -= vel / _shrinkDrag;
                yield return null;
            }
            foreach (var point in _endpoints) {
                point.localPosition = Vector3.zero;
            }
            foreach (var obj in _objToDisable) {
                obj.SetActive(false);
            }
        }
    }
}