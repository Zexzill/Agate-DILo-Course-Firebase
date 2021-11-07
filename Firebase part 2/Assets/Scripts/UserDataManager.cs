using Firebase.Storage;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class UserDataManager
{
    private const string PROGRESS_KEY = "Progress";

    public static UserProgressData Progress;

    public static void LoadFromLocal()
    {
        if(!PlayerPrefs.HasKey(PROGRESS_KEY))
        {
            Save(true);
        }
    }

    public static IEnumerator LoadFromCloud (System.Action onComplete)
    {
        StorageReference targetStorage = GetTargetCloudStorage();
        bool isComplete = false;
        bool isSuccessful = false;
        
        // 1024x1024 = 1mb
        const long maxAllowedSize = 1024 * 1024;

        targetStorage.GetBytesAsync(maxAllowedSize).ContinueWith((Task<byte[]> task) =>
        {
            if (!task.IsFaulted)
            {
                string json = Encoding.Default.GetString(task.Result);
                Progress = JsonUtility.FromJson<UserProgressData>(json);
                isSuccessful = true;
            }

            isComplete = true;
        });

        while(!isComplete)
        {
            yield return null;
        }

        if(isSuccessful)
        {
            Save();
        }
        else
        {
            LoadFromLocal();
        }

        onComplete?.Invoke();
    }

    private static StorageReference GetTargetCloudStorage()
    {
        string deviceID = SystemInfo.deviceUniqueIdentifier;
        FirebaseStorage storage = FirebaseStorage.DefaultInstance;

        return storage.GetReferenceFromUrl($"{storage.RootReference}/{deviceID}");
    }

    public static void Load()
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
            Debug.Log("loaded");
        }
    }

    public static bool HasResources(int index)
    {
        bool hasResource = false;

        //cek apakah list memiliki index ini
        for(int i = 0; i < Progress.ResourceLevels.Count; i++)
        {
            if(index == i)
            {
                hasResource = true;
            }
        }

        return hasResource;
    }

    public static void Save(bool uploadToCloud = false)
    {
        string json = JsonUtility.ToJson(Progress);
        PlayerPrefs.SetString(PROGRESS_KEY, json);
        if(uploadToCloud)
        {
            AnalyticsManager.SetUserProperties("gold", Progress.Gold.ToString());
            byte[] data = Encoding.Default.GetBytes(json);
            StorageReference targetStorage = GetTargetCloudStorage();
            targetStorage.PutBytesAsync(data);
        }
    }
}
