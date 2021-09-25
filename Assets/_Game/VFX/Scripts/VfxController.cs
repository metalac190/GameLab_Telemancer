using UnityEngine;
using UnityEngine.VFX;

public class VfxController : MonoBehaviour
{
    [SerializeField] private VisualEffect _effectToPlay = null;

    public void Play()
    {
        if (_effectToPlay != null) {
            _effectToPlay.Play();
        }
    }
}