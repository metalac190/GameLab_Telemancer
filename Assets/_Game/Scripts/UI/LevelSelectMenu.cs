using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.UI
{
    public class LevelSelectMenu : MonoBehaviour
    {
        //[SerializeField] private Button _playLvl1, _playLvl2, _playLvl3;
        [SerializeField] private Button[] _playLvl, _loadLvl;
        [SerializeField] private GameObject[] _thumbnailImgs, _lockedImgs;


        public void Start()
        {
            foreach (var i in _lockedImgs)
            {
                i.SetActive(false);
            }
            
            int savedLevel = PlayerPrefs.GetInt("Level");
            int savedCkpt = PlayerPrefs.GetInt("Checkpoint");

            for (int i = 1; i <= 3; i++)
            {
                _loadLvl[i - 1].gameObject.SetActive(i == savedLevel && savedCkpt != 0);
            }
        }
        
        public void 
    }
}