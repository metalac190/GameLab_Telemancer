using System;
using UnityEngine;
using UnityEngine.SceneManagement;


public class EasterEggManager : MonoBehaviour
{
    public static EasterEggManager current;
    public Action OnEasterEggFound;
    [SerializeField] private int _EasterEggsInLevel;
    private int _eggsFound;

    public void Awake()
    {
        current = this;
        OnEasterEggFound += foundEasterEgg;
    }

    private void foundEasterEgg()
    {
        _eggsFound++;
        
        // Show something on the hud
        Debug.Log("easter eggs found: " + _eggsFound + "/" + _EasterEggsInLevel);
        UIEvents.current.NotifyPlayer("Secrets Found: " + _eggsFound + "/" + _EasterEggsInLevel);
        
        if (_eggsFound == _EasterEggsInLevel)
            allEggsFound();
    }

    private void allEggsFound()
    {
        var lvl = SceneManager.GetActiveScene().buildIndex;
        switch (lvl)
        {
            case 2:
                //lvl 1
                AchievementManager.current.unlockAchievement(
                    AchievementManager.Achievements.AllLvl1EasterEggs);
                break;
            case 3:
                //lvl 2
                AchievementManager.current.unlockAchievement(
                    AchievementManager.Achievements.AllLvl2EasterEggs);
                break;
            case 4:
                //lvl 3
                AchievementManager.current.unlockAchievement(
                    AchievementManager.Achievements.AllLvl3EasterEggs);
                break;
        }
    }
}