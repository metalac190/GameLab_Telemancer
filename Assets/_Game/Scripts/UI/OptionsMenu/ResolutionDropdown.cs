using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.UI.OptionsMenu
{
    public class ResolutionDropdown : MonoBehaviour
    {
        private Resolution[] _resolutions;

        [SerializeField] private TMPro.TMP_Dropdown _dropdown;
        
        private void Start()
        {
            _resolutions = Screen.resolutions;
            _dropdown.ClearOptions();

            int currentRes = 0;
            List<string> dropdownOptions = new List<string>();
            for (int i = 0; i < _resolutions.Length; i++)
            {
                dropdownOptions.Add(_resolutions[i].width + "x" + _resolutions[i].height 
                                    + " @" + _resolutions[i].refreshRate + "hz");

                if (_resolutions[i].width == Screen.width &&
                    _resolutions[i].height == Screen.height)
                    currentRes = i;
            }

            int savedRes = (int)PlayerPrefs.GetFloat("Resolution", -1f);
            if (savedRes != -1)
                currentRes = savedRes;
            
            _dropdown.AddOptions(dropdownOptions);
            _dropdown.value = currentRes;
            _dropdown.RefreshShownValue();
        }
        
        public void SetValue(int value)
        {
            Resolution res = _resolutions[value];
            bool fullscreen = (int)PlayerPrefs.GetFloat("Fullscreen") == 1;
            Screen.SetResolution(res.width, res.height, fullscreen);
            PlayerPrefs.SetFloat("Resolution", value);
        }
    }
}