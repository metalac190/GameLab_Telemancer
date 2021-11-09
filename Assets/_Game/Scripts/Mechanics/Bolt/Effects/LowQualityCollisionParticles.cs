using System.Collections;
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

        public void AutoKill(float timer)
        {
            StartCoroutine(Kill(timer));
        }

        private IEnumerator Kill(float timer)
        {
            yield return new WaitForSecondsRealtime(timer);
            Destroy(gameObject);
        }
    }
}