﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundDetection : MonoBehaviour {

    //private BoxCollider collider;
    private PlayerController pc;

    private void Awake() {
        Debug.Log("ah");
        //collider = GetComponent<BoxCollider>();
        pc = GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other) {
        pc.grounded = true;
    }

    private void OnTriggerExit(Collider other) {
        pc.grounded = false;
    }

    private void OnTriggerStay(Collider other) {
        pc.grounded = true;
    }

}