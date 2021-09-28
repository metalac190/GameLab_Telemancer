using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTest : MonoBehaviour {

    private void OnTriggerEnter(Collider other) {
        Debug.Log("enter");
    }

    private void OnTriggerExit(Collider other) {
        Debug.Log("exit");
    }

}