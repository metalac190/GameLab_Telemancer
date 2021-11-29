using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporalRift : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // TODO: kill
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            //Debug.Log("Kill player");
            other.GetComponent<Mechanics.Player.PlayerState>()?.OnKill();
        }
        else if(other.gameObject.GetComponent<LargeRock>() != null)
        {
            other.gameObject.GetComponent<LargeRock>().Reset();
            AchievementManager.current.unlockAchievement(AchievementManager.Achievements.RockOutOfBounds);
        }
    }
}
