using System.Collections;
using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using System.Collections.Generic;
using System;
using System.Linq;


[System.Serializable]
public class DataToSave
{
    public string userName;
    public int maxLevelCleared = 0;
    public bool starterReceived = false;
    public bool tutorialCleared = false;
    public int dreamCoinAmount = 0;
    public int specialCurrencyAmount = 0;
    public List<InventoryItem> inventory;
    public List<InventoryItem> equippedItems;
    public List<Monster> equippedMonsters;
    public List<Monster> unlockedMonsters;

}

public class DataSaver : MonoBehaviour
{
    private const int INVENTORY_LIMIT = 40;

    public static DataSaver Instance;
    public DataToSave dts;
    private DatabaseReference dbRef;
    public GameObject inventoryFullPanelPrefab;


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

    public void UpdateMaxLevelCleared(int level){
        Instance.dts.maxLevelCleared = level;
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

    public bool IsInventoryFull()
    {
        return dts.inventory.Count >= INVENTORY_LIMIT;
    }

    public void AddItemToInventory(int itemId)
    {
        if (IsInventoryFull())
        {
            Debug.LogError("inventory is full");
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
            RemoveItemFromEquipped(itemToRemove.uniqueId);
            dts.inventory.Remove(itemToRemove);
        }
        SaveDataFn();

    }

    public void AddItemToEquipped(string uniqueId)
    {
        if (dts.equippedItems.Count >= 3)
        {
            Debug.LogError("maximum equipped items reached.");
            return;
        }
        InventoryItem itemToEquip = dts.inventory.Find(item => item.uniqueId == uniqueId);
        if (itemToEquip != null)
        {
            dts.equippedItems.Add(itemToEquip);
        }
        SaveDataFn();

    }

    public void RemoveItemFromEquipped(string uniqueId)
    {
        InventoryItem itemToRemove = dts.equippedItems.Find(item => item.uniqueId == uniqueId);
        if (itemToRemove != null)
        {
            dts.equippedItems.Remove(itemToRemove);
        }
        SaveDataFn();

    }

    public bool IsItemEquipped(string uniqueId)
    {
        return dts.equippedItems.Any(x => x.uniqueId == uniqueId);
    }

    public void AddMonsterToInventory(int monsterId)
    {
        dts.unlockedMonsters.Add(MonsterManager.Instance.GetMonsterFromID(monsterId));
        SaveDataFn();
    }

    public void AddMonsterToEquipped(int monsterId)
    {
        if (dts.equippedMonsters.Count >= 3)
        {
            Debug.LogError("maximum equipped monsters reached.");
            return;
        }
        Monster monsterToEquip = dts.unlockedMonsters.Find(monster => monster.id == monsterId);
        if (monsterToEquip != null)
        {
            dts.equippedMonsters.Add(monsterToEquip);
        }
        SaveDataFn();

    }

    public void ShowInventoryFullPanel()
    {
        StartCoroutine(ThrowAndDestroyObject());
    }

    private IEnumerator ThrowAndDestroyObject()
    {
        GameObject canvas = GameObject.Find("UI");
        if (canvas == null)
        {
            Debug.LogError("Canvas not found in the target scene!");
            yield break;
        }

        // Calculate spawn position relative to the Canvas
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Vector3 spawnPosition = canvasRect.position + new Vector3(0, 0, 0f);

        // Instantiate the prefab under the Canvas
        GameObject obj = Instantiate(inventoryFullPanelPrefab, spawnPosition, Quaternion.identity, canvas.transform);
        // Wait for a few seconds
        yield return new WaitForSeconds(1);

        // Destroy or deactivate the object
        Destroy(obj);
        // Alternatively, you can deactivate it:
        // obj.SetActive(false);
    }
}
