using System.Collections;
using UnityEngine;
using System;
using Firebase.Database;
using Firebase.Auth;

[Serializable]
public class DataToSave
{
    public string userName;
    public int maxLevelCleared;
    public bool tutorialCleared;
    public int dreamCoinAmount;
}

public class DataSaver : MonoBehaviour
{
    public static DataSaver Instance;
    public DataToSave dts;
    private DatabaseReference dbRef;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private string GetUserId()
    {
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        return user != null ? user.UserId : null;
    }

    public void SaveDataFn()
    {
        string userId = GetUserId();
        if (userId == null)
        {
            Debug.LogError("No user is logged in.");
            return;
        }

        string json = JsonUtility.ToJson(dts);
        dbRef.Child("users").Child(userId).SetRawJsonValueAsync(json);
    }

    public void LoadDataFn()
    {
        StartCoroutine(LoadDataEnum());
    }

    IEnumerator LoadDataEnum()
    {
        string userId = GetUserId();
        if (userId == null)
        {
            Debug.LogError("No user is logged in.");
            yield break;
        }

        var serverData = dbRef.Child("users").Child(userId).GetValueAsync();
        yield return new WaitUntil(predicate: () => serverData.IsCompleted);

        print("process is complete");

        DataSnapshot snapshot = serverData.Result;
        string jsonData = snapshot.GetRawJsonValue();

        if (jsonData != null)
        {
            print("server data found");
            dts = JsonUtility.FromJson<DataToSave>(jsonData);
        }
        else
        {
            print("no data found");
        }
    }
}
