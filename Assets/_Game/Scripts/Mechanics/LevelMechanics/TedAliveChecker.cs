using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TedAliveChecker : MonoBehaviour
{
    void Start()
    {
        UIEvents.current.OnAcquireWarpScroll += CheckIfTedsAreDead;
        UIEvents.current.OnAcquireResidueScroll += CheckIfTedsAreDead;
        UIEvents.current.OnAcquireGameEndScroll += CheckIfTedsAreDead;
    }

    void CheckIfTedsAreDead()
    {
        NPC[] teds = Object.FindObjectsOfType<NPC>();
        bool tedsAreDead = true;
        int tedsAlive = 0;
        foreach (var ted in teds)
        {
            if (ted.gameObject.activeSelf)
            {
                tedsAreDead = false;
                tedsAlive++;
                //break;
            }
        }
        
        Debug.Log("Teds Alive: "+ tedsAlive);
        
        if (tedsAreDead)
            AchievementManager.current.SaveTedDeathProgress();
    }
}
