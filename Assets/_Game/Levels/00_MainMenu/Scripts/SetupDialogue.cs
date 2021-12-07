using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupDialogue : MonoBehaviour
{
    private int numTalks = 31;
    private int levelTalks = 5;
    private int temp;

    public void RandomizeTedTalks()
    {
        string talkList = "";
        string levelTalkList = "";

        // Initialize and fill arrays
        int[] talks = new int[numTalks];
        int[] lvlTalks = new int[levelTalks];
        for (int i = 1; i <= numTalks; i++)
        {
            talks[i - 1] = i;
        }
        for (int i = 1; i <= levelTalks; i++)
        {
            lvlTalks[i - 1] = i;
        }

        // Shuffle arrays
        for (int i = 0; i < numTalks; i++)
        {
            int randNum = UnityEngine.Random.Range(0, numTalks);
            temp = talks[randNum];
            talks[randNum] = talks[i];
            talks[i] = temp;
        }
        for (int i = 0; i < levelTalks; i++)
        {
            int randNum = UnityEngine.Random.Range(0, levelTalks);
            temp = lvlTalks[randNum];
            lvlTalks[randNum] = lvlTalks[i];
            lvlTalks[i] = temp;
        }

        // Create randomized string and save to PlayerPrefs
        for (int i = 0; i < talks.Length; i++)
        {
            talkList += talks[i];
            if (i < talks.Length - 1)
                talkList += ",";
        }
        for (int i = 0; i < levelTalks; i++)
        {
            levelTalkList += lvlTalks[i];
            if (i < lvlTalks.Length - 1)
                levelTalkList += ",";
        }
        PlayerPrefs.SetString("TedTalks", talkList);
        PlayerPrefs.SetString("LevelTedTalks", levelTalkList);
        PlayerPrefs.SetInt("TedTalkIndex", 0);
        PlayerPrefs.SetInt("Level1TedTalkIndex", 0);
        PlayerPrefs.SetInt("Level2TedTalkIndex", 0);
        PlayerPrefs.SetInt("Level3TedTalkIndex", 0);
    }
}
