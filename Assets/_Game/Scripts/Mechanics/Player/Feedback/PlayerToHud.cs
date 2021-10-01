using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerToHud : MonoBehaviour
{
    private InteractableEnums _previousTarget;

    public void OnUpdateUnlockedAbilities(bool warpAbility, bool residueAbility)
    {
        UIEvents.current.UnlockWarpAbility(warpAbility);
        UIEvents.current.UnlockResidueAbility(residueAbility);
    }

    public void OnPrepareToCast(bool wasSuccessful = true)
    {
        UIEvents.current.CastBolt(wasSuccessful);
    }

    public void OnCastBolt()
    {
        // TODO: find a reason for this function to exist
    }
    
    public void OnWarpReady(bool ready = true)
    {
        UIEvents.current.WarpReady();
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
        UIEvents.current.ResidueReady();
    }

    public void OnHudColorChange(InteractableEnums type)
    {
        // If target type hasn't changed, then don't update color
        if (type == _previousTarget) return;

        UIEvents.current.ChangeXhairColor(type);
        _previousTarget = type;
    }
}