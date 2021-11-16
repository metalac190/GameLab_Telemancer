using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.UI
{
    public class LevelSelectMenu : MonoBehaviour
    {
        //[SerializeField] private Button _playLvl1, _playLvl2, _playLvl3;
        [SerializeField] private Button[] _loadLvl;
        [SerializeField] private GameObject[] _lockedImgs;
        [SerializeField] private GameObject _menuContainer;

        public void Awake()
        {
            _menuContainer.SetActive(false);
            foreach (var i in _lockedImgs)
            {
                i.SetActive(false);
            }
            
            int savedLevel = PlayerPrefs.GetInt("Level");
            int savedCkpt = PlayerPrefs.GetInt("Checkpoint");
            bool hasSave = (savedLevel != 0 && savedCkpt != 0);

            if (hasSave)
            {
                for (int i = 0; i < 3; i++)
                {
                    _loadLvl[i].gameObject.SetActive(i + 2 == savedLevel && savedCkpt != 0);
                }
            }
            else
            {
                foreach (var i in _loadLvl)
                {
                    i.gameObject.SetActive(false);
                }
            }


        }

        public void Start()
        {
            UIEvents.current.OnOpenLevelSelectMenu += b => _menuContainer.SetActive(b);
        }
    }
}