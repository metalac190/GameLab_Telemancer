using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityIcon : MonoBehaviour
{
    protected bool _inCastAnimation = false;
    protected bool _inCoolDownAnimation = false;
    
    public void CoolDown()
    {
        if (_inCoolDownAnimation) return;
        
        _inCoolDownAnimation = true;
        StartCoroutine(CoolDownAnimation());
    }
    
    private IEnumerator CoolDownAnimation()
    {
        yield return null;
    }

    public void Cast()
    {
        if (_inCastAnimation) return;
        
        _inCastAnimation = true;
        StartCoroutine(CastAnimation());
    }

    protected virtual IEnumerator CastAnimation()
    {
        yield return null;
    }
}
