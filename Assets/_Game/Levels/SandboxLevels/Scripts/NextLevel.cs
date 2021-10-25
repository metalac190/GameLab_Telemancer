using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mechanics.Player;

public class NextLevel : MonoBehaviour
{
    [SerializeField] private PlayerFeedback player = null;
    [SerializeField] private int levelID = 1;
    [SerializeField] private bool boltUnlocked = true, warpUnlocked = false, residueUnlocked = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") {
            TransitionManager.tm.ChangeLevel(levelID);
            player.OnUpdateUnlockedAbilities(boltUnlocked, warpUnlocked, residueUnlocked);
        }
    }
}