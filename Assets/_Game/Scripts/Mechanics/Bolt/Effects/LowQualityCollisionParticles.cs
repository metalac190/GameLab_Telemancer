using UnityEngine;

namespace Mechanics.Bolt.Effects
{
    public class LowQualityCollisionParticles : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _successfulHit = null;
        [SerializeField] private ParticleSystem _failedHit = null;

        public void Play(bool successful)
        {
            if (successful) {
                _successfulHit.Play();
            } else {
                _failedHit.Play();
            }
        }

        public void Stop()
        {
            _successfulHit.Stop();
            _failedHit.Stop();
        }
    }
}