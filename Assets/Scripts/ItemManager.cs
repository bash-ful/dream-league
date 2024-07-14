using System.Collections.Generic;
using UnityEngine;

public enum EffectType
{
    HealPercentageOfMaxHP,
    DamagePercentageOfMaxHP,
    ModifySelfDamageTakenModifier,
    ModifySelfDamageDealtModifier,

    ModifyEnemyDamageTakenModifier,

    ModifyEnemyDamageDealtModifier,

    ReflectDamage,
    CheatDeath,
    ModifyTimerBySeconds,
    Stun
}

[System.Serializable]
public class Effect
{
    public string type;
    public float value;
    public int effectDuration;
    public bool keepStacking;
}

[System.Serializable]
public class ActiveEffect
{
    public EffectType type;
    public float value;
    public int remainingDuration;
    public bool keepStacking;
}

[System.Serializable]
public class Item
{
    public int itemID;
    public string name;
    public string buffInfo;
    public string debuffInfo;
    public List<Effect> effects;
    public int price;
    public int rarity;
    public string spriteResourcesPath;
    public string backgroundResourcesPath;
}

[System.Serializable]
public class ItemList
{
    public List<Item> itemList;
}

public class ItemManager : MonoBehaviour
{
    private ItemList itemList;

    void Start()
    {
        Init();
    }
    public void Init()
    {
        TextAsset itemsJson = Resources.Load<TextAsset>("Json/Items");

        if (itemsJson != null)
        {
            itemList = JsonUtility.FromJson<ItemList>(itemsJson.text);
            foreach (Item item in itemList.itemList)
            {
                print($"name: {item.name}");
            }
        }
        else
        {
            Debug.LogError("error reading items JSON");
        }
    }

    public Item GetItemFromID(int itemID)
    {
        return itemList.itemList.Find(item => item.itemID == itemID);
    }

    public List<Item> GetItemList()
    {
        return itemList.itemList;
    }

    public Sprite GetItemSprite(Item item)
    {
        return Resources.Load<Sprite>(item.spriteResourcesPath);
    }
    public Sprite GetBackgroundSprite(Item item)
    {
        return Resources.Load<Sprite>(item.backgroundResourcesPath);
    }

}
