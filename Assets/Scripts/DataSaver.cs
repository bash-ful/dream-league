using System.Collections;
using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using System.Collections.Generic;
using System;


[System.Serializable]
public class DataToSave
{
    public string userName;
    public int maxLevelCleared;
    public bool tutorialCleared;
    public int dreamCoinAmount, specialCurrencyAmount;
    public List<InventoryItem> inventory;
}

public class DataSaver : MonoBehaviour
{
    private const int INVENTORY_LIMIT = 40;

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
        return user?.UserId;
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

    public void AddDreamCoins(int amount)
    {
        Instance.dts.dreamCoinAmount += amount;
        SaveDataFn();
    }

    public void DeductDreamCoins(int amount)
    {
        AddDreamCoins(-amount);
    }

    public void AddSpecialCurrency(int amount)
    {
        Instance.dts.specialCurrencyAmount += amount;
        SaveDataFn();
    }

    public void DeductSpecialCurrency(int amount)
    {
        AddSpecialCurrency(-amount);
    }

    public List<InventoryItem> GetInventoryList()
    {
        return dts.inventory;
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

    public void AddItemToInventory(int itemId)
    {
        if (dts.inventory.Count >= INVENTORY_LIMIT)
        {
            Debug.LogError("Inventory limit reached. Cannot add more items.");
            return;
        }
        string uniqueId = Guid.NewGuid().ToString();
        InventoryItem newItem = new() { itemId = itemId, uniqueId = uniqueId };
        dts.inventory.Add(newItem);
        SaveDataFn();
    }

    public void RemoveItemFromInventory(string uniqueId)
    {
        InventoryItem itemToRemove = dts.inventory.Find(item => item.uniqueId == uniqueId);
        if (itemToRemove != null)
        {
            dts.inventory.Remove(itemToRemove);
            SaveDataFn();
        }
    }

}
