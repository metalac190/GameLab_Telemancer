using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerToHud : MonoBehaviour
{
    [SerializeField] private float _inputFlashTime = 0.1f;
    [SerializeField] private Xhair _crosshair = null;

    [Header("HUD Images")]
    [SerializeField] private Image _hudXhair = null;
    [SerializeField] private Image _castImage = null;
    [SerializeField] private Image _warpImage = null;
    [SerializeField] private Image _residueImage = null;

    [Header("Ability Colors")]
    [SerializeField] private Color _disabledColor = new Color(1, 1, 1, 36f / 255f);
    [SerializeField] private Color _normalColor = new Color(1, 1, 1, 0.5f);
    [SerializeField] private Color _usedColor = new Color(0.5f, 0.75f, 0.5f, 0.75f);
    [SerializeField] private Color _readyToUseColor = new Color(0.8f, 0.7f, 0.4f, 0.6f);
    [SerializeField] private Color _failedColor = new Color(0.75f, 0.5f, 0.5f, 0.5f);

    [Header("Crosshair Colors")]
    [SerializeField] private Color _xhairColorNormal = Color.white;
    [SerializeField] private Color _xhairColorWarp = new Color(162, 191, 240, 1f);
    [SerializeField] private Color _xhairColorInteract = Color.green;

    public void OnUpdateUnlockedAbilities(bool warpAbility, bool residueAbility)
    {
        if (_warpImage != null) {
            _warpImage.color = warpAbility ? _normalColor : _disabledColor;
        }
        if (_residueImage != null) {
            _residueImage.color = residueAbility ? _normalColor : _disabledColor;
        }
    }

    public void OnPrepareToCast(bool wasSuccessful = true)
    {
        if (_castImage != null) {
            StartCoroutine(InputDebug(_castImage, wasSuccessful));
        }
    }

    public void OnCastBolt()
    {
        StartCoroutine(_crosshair.FillBoltStatusBar(0.5f));
    }

    public void OnWarpReady(bool ready = true)
    {
        if (_warpImage != null) {
            _warpImage.color = ready ? _readyToUseColor : _normalColor;
        }
    }

    public void OnActivateWarp(bool wasSuccessful = true)
    {
        if (_warpImage != null) {
            StartCoroutine(InputDebug(_warpImage, wasSuccessful));
        }
    }

    public void OnResidueReady(bool ready = true)
    {
        if (_residueImage != null) {
            _residueImage.color = ready ? _readyToUseColor : _normalColor;
        }
    }

    public void OnActivateResidue(bool wasSuccessful = true)
    {
        if (_residueImage != null) {
            StartCoroutine(InputDebug(_residueImage, wasSuccessful));
        }
    }

    public void OnHudColorChange(InteractableEnums type)
    {
        if (_hudXhair == null) return;

        // Looking at Interactable is either -1, 0, or 1, for Null, Object, and Interactable, respectfully
        switch (type) {
            case InteractableEnums.WarpInteractable:
                _hudXhair.color = _xhairColorWarp;
                break;
            case InteractableEnums.PlayerInteractable:
                _hudXhair.color = _xhairColorInteract;
                break;
            case InteractableEnums.Object:
            case InteractableEnums.Null:
                _hudXhair.color = _xhairColorNormal;
                break;
        }
    }

    private IEnumerator InputDebug(Image image, bool successful)
    {
        image.color = successful ? _usedColor : _failedColor;
        yield return new WaitForSecondsRealtime(_inputFlashTime);
        image.color = _normalColor;
    }
}