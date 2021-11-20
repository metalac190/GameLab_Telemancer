using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager current;
    private List<string> _achievementList;
    private List<string[]> _achievementDetails;
    private Queue<int> _achievementQueue;

    [Header("Achievement UI")] 
    [SerializeField] private GameObject _achvContainer;
    [SerializeField] private TMP_Text _achvName, _achvDesc;

    public enum Achievements
    {
        Die15Times = 0,
        KillAllTeds = 1,
        AllDialogue = 2,
        Reach16Speed = 3,
        RockOutOfBounds = 4,
        AllLvl1EasterEggs = 5,
        AllLvl2EasterEggs = 6,
        AllLvl3EasterEggs = 7,
        ParTimeLvl1 = 8,
        ParTimeLvl2 = 9,
        ParTimeLvl3 = 10,
        KonamiCode = 11
    }
    
    public void Awake()
    {
        current = this;

        _achievementList.Add("achv_Die15Times");
        _achievementDetails.Add(new string[]
        {
            "Mind The Gap",
            "Have Ted save you from falling 15 times"
        });
        
        _achievementList.Add("achv_KillAllTeds");
        _achievementDetails.Add(new string[]
        {
            "Bad Ending",
            "Kill every Ted in every level"
        });
        
        _achievementList.Add("achv_AllDialogue");
        _achievementDetails.Add(new string[]
        {
            "Lore Master",
            "Read all 75 Ted interactions"
        });
        
        _achievementList.Add("achv_GoFast");
        _achievementDetails.Add(new string[]
        {
            "It's Not A Movement Shooter...",
            "Reach a velocity of 16 units per second"
        });
        
        _achievementList.Add("achv_RockOOB");
        _achievementDetails.Add(new string[]
        {
            "Working As Intended",
            "Have a rock fall out of the playable area"
        });
        
        _achievementList.Add("achv_EasterEggsL1");
        _achievementDetails.Add(new string[]
        {
            "GnomeHold Egg Hunt",
            "Find all easter eggs in level 1"
        });
        
        _achievementList.Add("achv_EasterEggsL2");
        _achievementDetails.Add(new string[]
        {
            "The Mines Egg Hunt",
            "Find all easter eggs in level 2"
        });
        
        _achievementList.Add("achv_EasterEggsL3");
        _achievementDetails.Add(new string[]
        {
            "Krystalkhrona Egg Hunt",
            "Find all easter eggs in level 3"
        });
        
        _achievementList.Add("achv_ParTimeL1");
        _achievementDetails.Add(new string[]
        {
            "GnomeHold Speedrunner",
            "Beat level 1 in 45 seconds or less"
        });
        
        _achievementList.Add("achv_ParTimeL2");
        _achievementDetails.Add(new string[]
        {
            "The Mines Speedrunner",
            "Beat level 2 in 60 seconds or less"
        });
        
        _achievementList.Add("achv_ParTimeL3");
        _achievementDetails.Add(new string[]
        {
            "Krystalkhrona Speedrunner",
            "Beat level 3 in 70 seconds or less"
        });
        
        _achievementList.Add("achv_KonamiCode");
        _achievementDetails.Add(new string[]
        {
            "Up Up Down Down...",
            "Unlock the cheats menu"
        });
    }
    
    

    public bool isUnlocked(Achievements achv)
    {
        int a = PlayerPrefs.GetInt(_achievementList[(int)achv], 0);
        return a == 1;
    }

    public void unlockAchevement(Achievements achv)
    {
        PlayerPrefs.SetInt(_achievementList[(int)achv], 1);
        PlayerPrefs.Save();
        _achievementQueue.Enqueue((int)achv);
    }

    private IEnumerator playAnimation()
    {
        while (_achievementQueue.Count != 0)
        {
            int x = _achievementQueue.Dequeue();
            
            _achvContainer.SetActive(true);
            _achvContainer.transform.localPosition
            
            // change text
            _achvName.text = _achievementDetails[x][0];
            _achvDesc.text = _achievementDetails[x][1];

            // move up from offscreen
            
            
            // hold

            // move down to offscreen
            
            _achvContainer.SetActive(false);
        }

        yield return null;
    }

    public string getAchevementName(Achievements achv)
    {
        return _achievementDetails[(int)achv][0];
    }
    
    public string getAchevementDesc(Achievements achv)
    {
        return _achievementDetails[(int)achv][1];
    }
}