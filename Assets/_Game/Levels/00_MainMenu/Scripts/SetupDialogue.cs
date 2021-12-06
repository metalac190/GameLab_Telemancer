using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupDialogue : MonoBehaviour
{
    private int numTalks = 30;
    private int temp;

    public void RandomizeTedTalks()
    {
        string talkList = "";

        // Initialize and fill array
        int[] talks = new int[numTalks];
        for (int i = 1; i <= numTalks; i++)
        {
            talks[i - 1] = i;
        }

        // Shuffle array
        for (int i = 0; i < numTalks; i++)
        {
            int randNum = UnityEngine.Random.Range(0, numTalks);
            temp = talks[randNum];
            talks[randNum] = talks[i];
            talks[i] = temp;
        }

        // Create randomized string and save to PlayerPrefs
        for (int i = 0; i < talks.Length; i++)
        {
            talkList += talks[i];
            if (i < talks.Length - 1)
                talkList += ",";
        }
        PlayerPrefs.SetString("TedTalks", talkList);
        PlayerPrefs.SetInt("TedTalkIndex", 0);
    }
}
