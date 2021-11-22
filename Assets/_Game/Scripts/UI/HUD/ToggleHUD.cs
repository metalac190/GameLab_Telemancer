using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ToggleHUD : MonoBehaviour
{
    private bool hudEnabled = true;
    private bool handsEnabled = true;
    private int counter = 1;
    [SerializeField] CanvasGroup hudGroup;
    [SerializeField] GameObject septemberArms;

    private void Start() {
        septemberArms = GameObject.Find("September_Arms");
    }

    void Update()
    {
        if(Keyboard.current.f1Key.wasPressedThisFrame){
            SwitchHidden(counter % 3);
            counter++;
        }
    }

    void SwitchHidden(int hidden)
    {
        switch(hidden)
        {
            // Everything is shown
            case 0: 
                hudGroup.alpha = 1;
                septemberArms?.SetActive(true);
                Debug.Log("Showing all");
                break;

            // HUD hidden
            case 1: 
                hudGroup.alpha = 0;
                septemberArms?.SetActive(true);
                Debug.Log("HUD Hidden");
                break;
            
            // Arms + HUD hidden
            case 2:
                hudGroup.alpha = 0;
                septemberArms?.SetActive(false);
                Debug.Log("Arms and HUD Hidden");
                break;
        }
    }
}
