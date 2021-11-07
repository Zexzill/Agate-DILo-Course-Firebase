using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UserDataManager
{
    private const string PROGRESS_KEY = "Progress";

    public static UserProgressData Progress;

    public static void load()
    {
        if(!PlayerPrefs.HasKey(PROGRESS_KEY))
        {
            Progress = new UserProgressData();
            Save();
        }
        else
        {
            string json = PlayerPrefs.GetString(PROGRESS_KEY);
            Progress = JsonUtility.FromJson<UserProgressData>(json);
        }
    }

    public static void Save()
    {
        string json = JsonUtility.ToJson(Progress);
        PlayerPrefs.SetString(PROGRESS_KEY, json);
    }

    public static bool HasResources(int index)
    {
        bool hasResources = false;

        for(int i = 0; i < Progress.ResourcesLevel.Count; i++)
        {
            if(index == i)
            {
                hasResources = true;
            }
        }

        return hasResources;
    }
}
