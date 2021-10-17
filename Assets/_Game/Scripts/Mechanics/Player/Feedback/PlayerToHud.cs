using System;
using System.Collections;
using UnityEngine;

public class PlayerToHud : MonoBehaviour
{
    [SerializeField] private float _attemptDisplayTime = 0.1f;

    private InteractableEnums _previousTarget;

    public void OnHudColorChange(InteractableEnums type)
    {
        // If target type hasn't changed, then don't update color
        if (type == _previousTarget) return;

        UIEvents.current.ChangeXhairColor(type);
        _previousTarget = type;
    }

    public void OnUpdateUnlockedAbilities(bool boltAbility, bool warpAbility, bool residueAbility)
    {
        UIEvents.current.UnlockBoltAbility(boltAbility);
        UIEvents.current.UnlockWarpAbility(warpAbility);
        UIEvents.current.UnlockResidueAbility(residueAbility);

        _boltDisplay = boltAbility ? AbilityHudEnums.Normal : AbilityHudEnums.Disabled;
        _warpDisplay = warpAbility ? AbilityHudEnums.Normal : AbilityHudEnums.Disabled;
        _residueDisplay = residueAbility ? AbilityHudEnums.Normal : AbilityHudEnums.Disabled;
    }

    // Bolt Ability HUD Display

    public void SetBoltDisplay(AbilityHudEnums type)
    {
        UIEvents.current.SetBoltDisplay(type);
    }

    public void SetBoltCooldown(float delta)
    {
        if (_lockBoltDebug) return;
        UIEvents.current.SetBoltCooldown(delta);
    }

    #region Bolt Display Management

    private AbilityHudEnums _boltDisplay;
    private bool _lockBoltDebug;
    private Coroutine _boltDebugDelay;

    public void UpdateBoltState(AbilityStateEnum boltState)
    {
        switch (boltState) {
            case AbilityStateEnum.Idle:
                _boltDisplay = AbilityHudEnums.Normal;
                break;
            case AbilityStateEnum.Ready:
                _boltDisplay = AbilityHudEnums.ReadyToUse;
                break;
        }

        if (_lockBoltDebug) return;
        SetBoltDisplay(_boltDisplay);
    }

    public void OnBoltAction(AbilityActionEnum boltAction)
    {
        if (_boltDebugDelay != null) {
            StopCoroutine(_boltDebugDelay);
            SetBoltDisplay(_boltDisplay);
        }

        switch (boltAction) {
            case AbilityActionEnum.InputDetected:
                SetBoltDisplay(AbilityHudEnums.InputDetected);
                _boltDebugDelay = StartCoroutine(BoltInputDebugDelay());
                break;
            case AbilityActionEnum.AttemptedUnsuccessful:
                SetBoltDisplay(AbilityHudEnums.Failed);
                _boltDebugDelay = StartCoroutine(BoltInputDebugDelay());
                break;
            case AbilityActionEnum.Acted:
                SetBoltDisplay(AbilityHudEnums.Used);
                // Should be removed probably. 
                UIEvents.current.SetCastBolt(true);
                _boltDebugDelay = StartCoroutine(BoltInputDebugDelay());
                break;
        }
    }

    private IEnumerator BoltInputDebugDelay()
    {
        _lockBoltDebug = true;
        yield return new WaitForSecondsRealtime(_attemptDisplayTime);
        _lockBoltDebug = false;
        _boltDebugDelay = null;
        SetBoltDisplay(_boltDisplay);
    }

    #endregion

    // Warp Ability HUD Display

    public void SetWarpDisplay(AbilityHudEnums type)
    {
        UIEvents.current.SetWarpDisplay(type);
    }

    public void SetWarpCooldown(float delta)
    {
        if (_lockWarpDebug) return;
        UIEvents.current.SetWarpCooldown(delta);
    }

    #region Warp Display Management

    private AbilityHudEnums _warpDisplay;
    private bool _lockWarpDebug;
    private Coroutine _warpDebugDelay;

    public void UpdateWarpState(AbilityStateEnum warpState)
    {
        switch (warpState) {
            case AbilityStateEnum.Idle:
                _warpDisplay = AbilityHudEnums.Normal;
                break;
            case AbilityStateEnum.Ready:
                _warpDisplay = AbilityHudEnums.ReadyToUse;
                break;
        }

        if (_lockWarpDebug) return;
        SetWarpDisplay(_warpDisplay);
    }

    public void OnWarpAction(AbilityActionEnum warpAction)
    {
        if (_warpDebugDelay != null) {
            StopCoroutine(_warpDebugDelay);
            SetWarpDisplay(_warpDisplay);
        }

        switch (warpAction) {
            case AbilityActionEnum.InputDetected:
                SetWarpDisplay(AbilityHudEnums.InputDetected);
                _warpDebugDelay = StartCoroutine(WarpInputDebugDelay());
                break;
            case AbilityActionEnum.AttemptedUnsuccessful:
                SetWarpDisplay(AbilityHudEnums.Failed);
                _warpDebugDelay = StartCoroutine(WarpInputDebugDelay());
                break;
            case AbilityActionEnum.Acted:
                SetWarpDisplay(AbilityHudEnums.Used);
                _warpDebugDelay = StartCoroutine(WarpInputDebugDelay());
                break;
        }
    }

    private IEnumerator WarpInputDebugDelay()
    {
        _lockWarpDebug = true;
        yield return new WaitForSecondsRealtime(_attemptDisplayTime);
        _lockWarpDebug = false;
        _warpDebugDelay = null;
        SetWarpDisplay(_warpDisplay);
    }

    #endregion

    // Residue Ability HUD Display

    public void SetResidueDisplay(AbilityHudEnums type)
    {
        UIEvents.current.SetResidueDisplay(type);
    }

    public void SetResidueCooldown(float delta)
    {
        if (_lockResidueDebug) return;
        UIEvents.current.SetResidueCooldown(delta);
    }

    #region Residue Display Management

    private AbilityHudEnums _residueDisplay;
    private bool _lockResidueDebug;
    private Coroutine _residueDebugDelay;

    public void UpdateResidueState(AbilityStateEnum residueState)
    {
        switch (residueState) {
            case AbilityStateEnum.Idle:
                _residueDisplay = AbilityHudEnums.Normal;
                break;
            case AbilityStateEnum.Ready:
                _residueDisplay = AbilityHudEnums.ReadyToUse;
                break;
        }

        if (_lockResidueDebug) return;
        SetResidueDisplay(_residueDisplay);
    }

    public void OnResidueAction(AbilityActionEnum residueAction)
    {
        if (_residueDebugDelay != null) {
            StopCoroutine(_residueDebugDelay);
            SetResidueDisplay(_residueDisplay);
        }

        switch (residueAction) {
            case AbilityActionEnum.InputDetected:
                SetResidueDisplay(AbilityHudEnums.InputDetected);
                _residueDebugDelay = StartCoroutine(ResidueInputDebugDelay());
                break;
            case AbilityActionEnum.AttemptedUnsuccessful:
                SetResidueDisplay(AbilityHudEnums.Failed);
                _residueDebugDelay = StartCoroutine(ResidueInputDebugDelay());
                break;
            case AbilityActionEnum.Acted:
                SetResidueDisplay(AbilityHudEnums.Used);
                _residueDebugDelay = StartCoroutine(ResidueInputDebugDelay());
                break;
        }
    }

    private IEnumerator ResidueInputDebugDelay()
    {
        _lockResidueDebug = true;
        yield return new WaitForSecondsRealtime(_attemptDisplayTime);
        _lockResidueDebug = false;
        _residueDebugDelay = null;
        SetResidueDisplay(_residueDisplay);
    }

    #endregion
}