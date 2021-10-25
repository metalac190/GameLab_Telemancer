using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScrollMenu : MonoBehaviour
{
   private string _warpName, _warpDesc;
   private string _residueName, _residueDesc;

   [SerializeField] private GameObject _menuContainer = null;
   [SerializeField] private GameObject _warpLockedIcon = null;
   [SerializeField] private TextMeshProUGUI _warpNameTxt = null, _warpDescTxt = null;
   [SerializeField] private GameObject _residueLockedIcon = null;
   [SerializeField] private TextMeshProUGUI _residueNameTxt = null, _residueDescTxt = null;
   
   private void Awake()
   {
      _warpName = _warpNameTxt.text;
      _warpDesc = _warpDescTxt.text;
      _residueName = _residueNameTxt.text;
      _residueDesc = _residueDescTxt.text;
      
      _menuContainer.SetActive(false);
      
      // TODO: replace this with a real solution
      var lvl = SceneManager.GetActiveScene().buildIndex;
      UnlockWarp(lvl >= 3);
      UnlockResidue(lvl >= 4);
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
      _warpNameTxt.text = b ? _warpName : "Locked";
      _warpDescTxt.text = b ? _warpDesc : "Spell not yet discovered";
      _warpLockedIcon.SetActive(!b);
   }

   private void UnlockResidue(bool b)
   {
      _residueNameTxt.text = b ? _residueName : "Locked";
      _residueDescTxt.text = b ? _residueDesc : "Spell not yet discovered";
      _residueLockedIcon.SetActive(!b);
   }
}
