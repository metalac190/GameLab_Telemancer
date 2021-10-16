using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Handle crosshair logic and animations(?)
/// Might be replaced by a more complete UI script in the near future.
/// </summary>
public class Xhair : MonoBehaviour
{
    private float _maxPercentFilled = 0.19f;
    [SerializeField] private Image _chargeBarL, _chargeBarR;

    public IEnumerator FillBoltStatusBar(float duration)
    {
        float time = 0;
        while (time < duration)
        {
            // Borrowing this from the internet really quick
            float t = time / duration;
            t = t * t * (3f - 2f * t);
            
            float percentFilled = Mathf.Lerp(0, _maxPercentFilled, t);
            _chargeBarL.fillAmount = percentFilled;
            _chargeBarR.fillAmount = percentFilled;
            time += Time.deltaTime;
            yield return null;
        }
        _chargeBarL.fillAmount = _maxPercentFilled;
        _chargeBarR.fillAmount = _maxPercentFilled;
    }
}
