using Mechanics.Bolt;
using UnityEngine;

namespace VFX.Scripts
{
    public class BoltTestSpawner : MonoBehaviour
    {
        [SerializeField] private bool _stopMovingOnDissipate = false;

        private BoltController _boltController;

        private void Start()
        {
            _boltController = FindObjectOfType<BoltController>();
        }

        public void Respawn()
        {
            if (_boltController == null) return;
            _boltController.PrepareToFire(transform.position, transform.forward, false);
            _boltController.Fire(transform.position, transform.forward);
        }

        public void Dissipate()
        {
            if (_boltController == null) return;
            _boltController.Dissipate(_stopMovingOnDissipate);
        }
    }
}