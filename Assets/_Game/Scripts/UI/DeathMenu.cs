using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathMenu : MonoBehaviour {
    
    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
    }

    public void IsEnabled(bool isEnabled) {
        animator.SetBool("Enabled", isEnabled);
    }

}
