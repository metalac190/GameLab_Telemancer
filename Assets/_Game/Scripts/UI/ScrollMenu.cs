using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScrollMenu : MonoBehaviour
{
   private string _warpName, _warpDesc;
   private string _residueName, _residueDesc;

   [SerializeField] private GameObject _menuContainer;
   [SerializeField] private GameObject _warpLockedIcon;
   [SerializeField] private TextMeshProUGUI _warpNameTxt, _warpDescTxt;
   [SerializeField] private GameObject _residueLockedIcon;
   [SerializeField] private TextMeshProUGUI _residueNameTxt, _residueDescTxt;
   
   private void Awake()
   {
      _warpName = _warpNameTxt.text;
      _warpDesc = _warpDescTxt.text;
      _residueName = _residueNameTxt.text;
      _residueDesc = _residueDescTxt.text;
      
      _menuContainer.SetActive(false);
   }

   private void Start()
   {
      UIEvents.current.OnUnlockWarpAbility += UnlockWarp;
      UIEvents.current.OnUnlockResidueAbility += UnlockResidue;
      UIEvents.current.OnOpenScrollMenu += b => _menuContainer.SetActive(b);
      UIEvents.current.OnPauseGame += delegate(bool b) { if (!b) _menuContainer.SetActive(false); };
   }

   private void UnlockWarp(bool b)
   {
      _warpNameTxt.text = b ? _warpName : "?????";
      _warpDescTxt.text = b ? _warpDesc : "?????";
      _warpLockedIcon.SetActive(!b);
   }

   private void UnlockResidue(bool b)
   {
      _residueNameTxt.text = b ? _residueName : "?????";
      _residueDescTxt.text = b ? _residueDesc : "?????";
      _residueLockedIcon.SetActive(!b);
   }
}
