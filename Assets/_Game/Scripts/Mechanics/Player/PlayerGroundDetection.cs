using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundDetection : MonoBehaviour {

    private PlayerController pc;

    private void Awake() {
        pc = GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other) {
        if(other.isTrigger)
            return;
        pc.grounded = true;
    }

    private void OnTriggerExit(Collider other) {
        if(other.isTrigger)
            return;
        pc.grounded = false;
    }

    private void OnTriggerStay(Collider other) {
        if(other.isTrigger)
            return;
        pc.grounded = true;
    }

}