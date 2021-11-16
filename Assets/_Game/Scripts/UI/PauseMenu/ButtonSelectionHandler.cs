using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ButtonSelectionHandler : MonoBehaviour
{
    [SerializeField] private Button _resumeBtn, _ckptBtn;
    private bool _inDoubleTapWindow;
    
    private void OnEnable()
    {
        _resumeBtn.Select();
        _inDoubleTapWindow = false;
    }

    private void Update()
    {
        // Pressing R selects 'restart checkpoint'; Double tapping R presses the button
        if (Keyboard.current.rKey.wasPressedThisFrame)
            if (_inDoubleTapWindow)
                _ckptBtn.onClick.Invoke();
            else
            {
                _ckptBtn.Select();
                StartCoroutine(DoubleTapDelay(0.5f));
            }
                
    }

    private IEnumerator DoubleTapDelay(float t)
    {
        _inDoubleTapWindow = true;
        Debug.Log("PauseMenu: Enter DoubleTap");
        yield return new WaitForSecondsRealtime(t);
        Debug.Log("PauseMenu: Exit DoubleTap");
        _inDoubleTapWindow = false;
    }
}