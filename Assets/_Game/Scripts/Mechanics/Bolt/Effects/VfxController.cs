using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

namespace Mechanics.Bolt.Effects
{
    public class VfxController : MonoBehaviour
    {
        [SerializeField] private VisualEffect _effectToPlay = null;

        private bool _missingVfx;

        private void OnValidate()
        {
            _missingVfx = _effectToPlay == null;
        }

        public void Play()
        {
            if (_missingVfx) return;
            _effectToPlay.Play();
        }

        public void Play(bool successful)
        {
            if (_missingVfx) return;
            _effectToPlay.SetBool("isSuccessful", successful);
            _effectToPlay.Play();
        }

        public void Stop()
        {
            if (_missingVfx) return;
            _effectToPlay.Stop();
        }
    }
}