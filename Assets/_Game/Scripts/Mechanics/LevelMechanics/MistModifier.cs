using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Mechanics.Player.Feedback.Options;

public class MistModifier : MonoBehaviour
{
    [SerializeField] VisualEffectAsset mist;
    [SerializeField] VisualEffectAsset betterMist;
    [SerializeField] VisualEffect[] mistObjs;
    private PlayerOptionsController _codeController;

    void Start()
    {
        CheckExistingCode();
        UIEvents.current.OnOpenCodeMenu += UpdateCodeController;
        if(PlayerPrefs.GetInt("ImprovedWaterfalls") == 1)
            SetImprovedWaterfalls(true);
    }

    private void UpdateCodeController()
    {
        CheckExistingCode();
        _codeController.ImprovedWaterfalls.OnSelect += SetImprovedWaterfalls;
        UIEvents.current.OnOpenCodeMenu -= UpdateCodeController;
    }

    private void SetImprovedWaterfalls(bool active)
    {
        //Debug.Log("Waterfalls are " + (active ? "Improved" : "Normal"));
        foreach(VisualEffect mistVFX in mistObjs)
        {
            if(active)
                mistVFX.visualEffectAsset = betterMist;
            else
                mistVFX.visualEffectAsset = mist;
        }
        PlayerPrefs.SetInt("ImprovedWaterfalls", active ? 1 : 0);
    }

    private void CheckExistingCode()
    {
        _codeController = PlayerOptionsController.Instance;
        if (_codeController == null)
            _codeController = FindObjectOfType<PlayerOptionsController>();
    }
}