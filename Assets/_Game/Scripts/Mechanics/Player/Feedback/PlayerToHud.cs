using System;
using System.Collections;
using UnityEngine;

public class PlayerToHud : MonoBehaviour
{
    [SerializeField] private float _attemptDisplayTime = 0.1f;

    private InteractableEnums _previousTarget;
    private bool _lockBoltDebug;
    private bool _lockWarpDebug;
    private bool _lockResidueDebug;

    public void OnHudColorChange(InteractableEnums type)
    {
        // If target type hasn't changed, then don't update color
        if (type == _previousTarget) return;

        UIEvents.current.ChangeXhairColor(type);
        _previousTarget = type;
    }

    public void OnUpdateUnlockedAbilities(bool warpAbility, bool residueAbility)
    {
        UIEvents.current.UnlockWarpAbility(warpAbility);
        UIEvents.current.UnlockResidueAbility(residueAbility);
    }

    #region Bolt

    public void UpdateBoltState(AbilityStateEnum boltState)
    {
        switch (boltState) {
            case AbilityStateEnum.Idle:
                break;
            case AbilityStateEnum.Ready:
                break;
        }
    }

    public void OnBoltAction(AbilityActionEnum boltAction)
    {
        switch (boltAction) {
            case AbilityActionEnum.InputDetected:
                // Set Icon Yellow
                break;
            case AbilityActionEnum.AttemptedUnsuccessful:
                // Set Icon Red
                break;
            case AbilityActionEnum.AttemptedSuccessful:
                // Set Icon Green
                break;
            case AbilityActionEnum.Acted:
                break;
        }
    }

    public void SetBoltCooldown(float delta)
    {
        if (_lockBoltDebug) return;
        // Delta is a value from 0 to 1

        // Set Icon Cooldown with var delta
    }

    private IEnumerator BoltInputDebugDelay()
    {
        _lockBoltDebug = true;
        yield return new WaitForSecondsRealtime(_attemptDisplayTime);
        _lockBoltDebug = false;
    }

    #endregion

    #region Warp

    public void UpdateWarpState(AbilityStateEnum warpState)
    {
        switch (warpState) {
            case AbilityStateEnum.Idle:
                UIEvents.current.WarpReady(false);
                break;
            case AbilityStateEnum.Ready:
                UIEvents.current.WarpReady(true);
                break;
        }
    }

    public void OnWarpAction(AbilityActionEnum warpAction)
    {
        switch (warpAction) {
            case AbilityActionEnum.InputDetected:
                // Set Icon Yellow
                break;
            case AbilityActionEnum.AttemptedUnsuccessful:
                // Set Icon Red
                break;
            case AbilityActionEnum.AttemptedSuccessful:
                // Set Icon Green
                break;
            case AbilityActionEnum.Acted:
                break;
        }
    }

    public void SetWarpCooldown(float delta)
    {
        if (_lockWarpDebug) return;
        // Delta is a value from 0 to 1

        // Set Icon Cooldown with var delta
    }

    private IEnumerator WarpInputDebugDelay()
    {
        _lockWarpDebug = true;
        yield return new WaitForSecondsRealtime(_attemptDisplayTime);
        _lockWarpDebug = false;
    }

    #endregion

    #region Residue

    public void UpdateResidueState(AbilityStateEnum residueState)
    {
        switch (residueState) {
            case AbilityStateEnum.Idle:
                UIEvents.current.ResidueReady(false);
                break;
            case AbilityStateEnum.Ready:
                UIEvents.current.ResidueReady(true);
                break;
        }
    }

    public void OnResidueAction(AbilityActionEnum residueAction)
    {
        switch (residueAction) {
            case AbilityActionEnum.InputDetected:
                // Set Icon Yellow
                break;
            case AbilityActionEnum.AttemptedUnsuccessful:
                // Set Icon Red
                break;
            case AbilityActionEnum.AttemptedSuccessful:
                // Set Icon Green
                break;
            case AbilityActionEnum.Acted:
                break;
        }
    }

    public void SetResidueCooldown(float delta)
    {
        if (_lockResidueDebug) return;
        // Delta is a value from 0 to 1

        // Set Icon Cooldown with var delta
    }

    private IEnumerator ResidueInputDebugDelay()
    {
        _lockResidueDebug = true;
        yield return new WaitForSecondsRealtime(_attemptDisplayTime);
        _lockResidueDebug = false;
    }

    #endregion

    public void OnPrepareToCast(bool wasSuccessful = true)
    {
        UIEvents.current.CastBolt(wasSuccessful);
    }

    public void OnWarpReady(bool ready = true)
    {
        UIEvents.current.WarpReady(ready);
    }

    public void OnActivateWarp(bool wasSuccessful = true)
    {
        UIEvents.current.CastWarp(wasSuccessful);
    }

    public void OnActivateResidue(bool wasSuccessful = true)
    {
        UIEvents.current.CastResidue(wasSuccessful);
    }

    public void OnResidueReady(bool ready = true)
    {
        UIEvents.current.ResidueReady(ready);
    }
}