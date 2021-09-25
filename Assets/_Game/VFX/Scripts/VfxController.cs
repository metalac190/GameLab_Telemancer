using System.Collections;
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

    public void Play(bool successful)
    {
        if (_effectToPlay != null) {
            _effectToPlay.SetBool("isSuccessful", successful);
            _effectToPlay.Play();
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