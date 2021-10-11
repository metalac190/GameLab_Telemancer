using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteBillboard : MonoBehaviour
{
    void LateUpdate() 
    {
        transform.LookAt(Camera.main.transform.position, new Vector3(0, 1, 0));
    }
}
