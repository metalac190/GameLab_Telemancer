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
        _codeController = GetCodeController();
        UIEvents.current.OnOpenCodeMenu += UpdateCodeController;
        if (PlayerPrefs.GetInt("ImprovedWaterfalls") == 1)
            SetImprovedWaterfalls(true);
    }

    private void OnDisable()
    {
        if(_codeController != null)
            _codeController.ImprovedWaterfalls.OnSelect -= SetImprovedWaterfalls;
    }

    private void UpdateCodeController()
    {
        if (_codeController == null)
            _codeController = GetCodeController();
        _codeController.ImprovedWaterfalls.OnSelect += SetImprovedWaterfalls;
        UIEvents.current.OnOpenCodeMenu -= UpdateCodeController;
    }

    private void SetImprovedWaterfalls(bool active)
    {
        //Debug.Log("Waterfalls are " + (active ? "Improved" : "Normal"));
        foreach (VisualEffect mistVFX in mistObjs)
        {
            if (active)
                mistVFX.visualEffectAsset = betterMist;
            else
                mistVFX.visualEffectAsset = mist;
        }
        PlayerPrefs.SetInt("ImprovedWaterfalls", active ? 1 : 0);
    }

    private PlayerOptionsController GetCodeController()
    {
        PlayerOptionsController controller = PlayerOptionsController.Instance;
        if (controller == null)
            controller = FindObjectOfType<PlayerOptionsController>();
        if (controller != null)
            controller.ImprovedWaterfalls.SelectItem(PlayerPrefs.GetInt("ImprovedWaterfalls") == 1 ? true : false);
        return controller;
    }
}