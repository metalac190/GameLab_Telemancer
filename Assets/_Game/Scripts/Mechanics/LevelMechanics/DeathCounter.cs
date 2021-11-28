using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCounter : MonoBehaviour
{
    private int _deathCount;

    private void Start()
    {
        _deathCount = PlayerPrefs.GetInt("PlayerDeaths", 0);
        UIEvents.current.OnPlayerDied += PlayerDied;
        Debug.Log("Player Deaths: " + _deathCount);
    }

    private void PlayerDied()
    {
        _deathCount++;
        PlayerPrefs.SetInt("PlayerDeaths", _deathCount);
        PlayerPrefs.Save();
        
        Debug.Log("Player Deaths: " + _deathCount);
        
        if (_deathCount >= 15)
        {
            AchievementManager.current.unlockAchievement(AchievementManager.Achievements.Die15Times);
        }
    }
}
