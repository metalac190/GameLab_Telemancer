using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.UI
{
    public class AchievementMenuItem : MonoBehaviour
    {
        public bool unlocked;
        public TMP_Text tmpname, tmpdesc;
        public Image _unlockIndicator;

        // public void OnBecameVisible()
        // {
        //     _unlockIndicator.color = unlocked ? new Color(198/255f, 160/255f, 69/255f, 1f) : Color.white;
        // }

        public void setUnlocked(bool i)
        {
            unlocked = i;
            //_unlockIndicator.color = i ? new Color(198/255f, 160/255f, 69/255f, 1f) : Color.white;
            _unlockIndicator.gameObject.SetActive(i);
        }
    }
}