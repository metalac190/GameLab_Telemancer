using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager current;
    private List<string> _achievementList;
    private List<string[]> _achievementDetails;

    [Header("Achievement UI")] 
    [SerializeField] private GameObject _achvContainer;
    [SerializeField] private TMP_Text _achvName, _achvDesc;

    [Header("Animation Timing")] 
    [SerializeField] private float moveUpDuration = 1f;
    [SerializeField] private float holdDuration = 2.5f;
    [SerializeField] private float moveDownDuration = 0.8f;

    public enum Achievements
    {
        Die15Times = 0,
        KillAllTeds = 1,
        AllDialogue = 2,
        Reach14Speed = 3,
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
        _achievementList = new List<string>();
        _achievementDetails = new List<string[]>();
        
        _achievementList.Add("achv_Die15Times");
        _achievementDetails.Add(new string[]
        {
            "Mind The Gap",
            "Have Ted save you from falling 15 times",
            "Hidden"
        });
        
        _achievementList.Add("achv_KillAllTeds");
        _achievementDetails.Add(new string[]
        {
            "It's Two O'Clock Somewhere",
            "In every level, banish every Ted to \"Two O'Clock\""
        });
        
        _achievementList.Add("achv_AllDialogue");
        _achievementDetails.Add(new string[]
        {
            "Lore Master",
            "Read all Ted interactions"
        });
        
        _achievementList.Add("achv_GoFast");
        _achievementDetails.Add(new string[]
        {
            "It's Not A Movement Shooter...",
            "Reach a velocity of 14 units per second",
            "Hidden"
        });
        
        _achievementList.Add("achv_RockOOB");
        _achievementDetails.Add(new string[]
        {
            "Working As Intended",
            "Have a rock fall out of the playable area",
            "Hidden"
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
            "Up Up Down Down Left Right...",
            "Unlock the cheats menu",
            "Hidden"
        });
    }

    public void Start()
    {
        _achvContainer.SetActive(false);
    }

    public bool isUnlocked(Achievements achv)
    {
        int a = PlayerPrefs.GetInt(_achievementList[(int)achv], 0);
        return a == 1;
    }
    
    public bool isUnlocked(int x)
    {
        int a = PlayerPrefs.GetInt(_achievementList[x], 0);
        return a == 1;
    }

    public void unlockAchievement(Achievements achv)
    {
        if (isUnlocked(achv)) return;
        
        PlayerPrefs.SetInt(_achievementList[(int)achv], 1);
        PlayerPrefs.Save();
        StartCoroutine(playAnimation(achv));
    }

    private IEnumerator playAnimation(Achievements achv)
    {
        // init values
        float time = 0;
        
        _achvContainer.SetActive(true);
        _achvContainer.transform.localPosition = new Vector3(0, -400, 0);
            
        // change text
        _achvName.text = _achievementDetails[(int)achv][0];
        _achvDesc.text = _achievementDetails[(int)achv][1];

        // move up from offscreen
        Vector3 startPosition = transform.localPosition;
        Vector3 targetPosition = new Vector3(0, 0, 0);

        while (time < moveUpDuration)
        {
            float t = time / moveUpDuration;
            t = t * t * (3f - 2f * t);
            
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
            time += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = targetPosition;
            
        // hold
        yield return new WaitForSecondsRealtime(holdDuration);

        // move down to offscreen
        startPosition = transform.localPosition;
        targetPosition = new Vector3(0, -400, 0);
        time = 0;

        while (time < moveDownDuration)
        {
            float t = time / moveDownDuration;
            t = t * t * (3f - 2f * t);
            
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
            time += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = targetPosition;
            
        // hide container
        _achvContainer.SetActive(false);

        yield return null;
    }

    public string getAchievementName(Achievements achv)
    {
        return _achievementDetails[(int)achv][0];
    }
    
    public string getAchievementName(int a)
    {
        return _achievementDetails[a][0];
    }
    
    public string getAchievementDesc(Achievements achv)
    {
        return _achievementDetails[(int)achv][1];
    }
    
    public string getAchievementDesc(int a)
    {
        return _achievementDetails[a][1];
    }

    public int getAchievementCount()
    {
        return _achievementList.Count;
    }

    public bool isDescHidden(int a)
    {
        return _achievementDetails[a].Length == 3;
    }

    public void CheckSpeedrunTime(float t)
    {
        float min = Mathf.FloorToInt(t / 60);
        float sec = Mathf.FloorToInt(t % 60);
        sec += min * 60;
        var lvl = SceneManager.GetActiveScene().buildIndex;
        switch (lvl)
        {
            case 2:
                //lvl 1
                if (sec <= 45)
                    unlockAchievement(Achievements.ParTimeLvl1);
                break;
            case 3:
                //lvl 2
                if (sec <= 60)
                    unlockAchievement(Achievements.ParTimeLvl2);
                break;
            case 4:
                //lvl 3
                if (sec <= 70)
                    unlockAchievement(Achievements.ParTimeLvl3);
                break;
        }
    }

    public void SaveTedDeathProgress()
    {
        int lvl1Completed = PlayerPrefs.GetInt("TedsDead_Lvl1", 0);
        int lvl2Completed = PlayerPrefs.GetInt("TedsDead_Lvl2", 0);
        int lvl3Completed = PlayerPrefs.GetInt("TedsDead_Lvl3", 0);
        
        var lvl = SceneManager.GetActiveScene().buildIndex;
        switch (lvl)
        {
            case 2:
                //lvl 1
                lvl1Completed = 1;
                PlayerPrefs.SetInt("TedsDead_Lvl1", 1);
                break;
            case 3:
                //lvl 2
                lvl2Completed = 1;
                PlayerPrefs.SetInt("TedsDead_Lvl2", 1);
                break;
            case 4:
                //lvl 3
                lvl3Completed = 1;
                PlayerPrefs.SetInt("TedsDead_Lvl3", 1);
                break;
        }

        if (lvl1Completed == 1 && lvl2Completed == 1 && lvl3Completed == 1)
        {
            unlockAchievement(Achievements.KillAllTeds);
        }
    }
}