using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpPad : MonoBehaviour
{
    [SerializeField] private GameObject _wispReciever = null;
    public GameObject WispReciever { get => _wispReciever; }
}
