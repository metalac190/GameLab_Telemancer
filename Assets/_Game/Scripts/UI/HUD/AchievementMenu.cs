using System;
using System.Collections.Generic;
using _Game.Scripts.UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class AchievementMenu : MonoBehaviour
{
    [SerializeField] private GameObject _gridContainer;
    [SerializeField] private GameObject _achvPrefab;
    private List<GameObject> _achvObjs;

    public void Start()
    {
        _achvObjs = new List<GameObject>();
        
        for (int i = 0; i < AchievementManager.current.getAchievementCount(); i++)
        {
            string n = AchievementManager.current.getAchievementName(i);
            string d = AchievementManager.current.getAchievementDesc(i);
            bool unlocked = AchievementManager.current.isUnlocked(i);
            bool hiddenDesc = AchievementManager.current.isDescHidden(i);
            GameObject item = Instantiate(_achvPrefab, _gridContainer.transform);
            item.GetComponent<AchievementMenuItem>().tmpname.text = n;
            item.GetComponent<AchievementMenuItem>().tmpdesc.text = (unlocked || !hiddenDesc) ? d : "? ? ?";
            item.GetComponent<AchievementMenuItem>().setUnlocked(unlocked);
            _achvObjs.Add(item);
        }
    }

    public void OnEnable()
    {
        for (int i = 0; i < _achvObjs.Count; i++)
        {
            string n = AchievementManager.current.getAchievementName(i);
            string d = AchievementManager.current.getAchievementDesc(i);
            bool unlocked = AchievementManager.current.isUnlocked(i);
            bool hiddenDesc = AchievementManager.current.isDescHidden(i);
            GameObject item = _achvObjs[i];
            item.GetComponent<AchievementMenuItem>().tmpname.text = n;
            item.GetComponent<AchievementMenuItem>().tmpdesc.text = (unlocked || !hiddenDesc) ? d : "? ? ?";
            item.GetComponent<AchievementMenuItem>().setUnlocked(unlocked);
        }
    }
}