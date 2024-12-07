
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[System.Serializable]
public class Monster
{
    public int id;
    public string name, abilityName, abilityDescription, spritePath, profileSpritePath, elementType;
    public float baseHealth, baseDamage;
    public int[] moves = new int[4];
}

[System.Serializable]
public class MonsterList
{
    public List<Monster> monsterList;
}

public class MonsterManager : MonoBehaviour
{
    public static MonsterManager Instance { get; private set; }
    private MonsterList monsterList;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Init();

    }

    private void Init()
    {
        TextAsset monstersJson = Resources.Load<TextAsset>("Json/Monsters");

        if (monstersJson != null)
        {
            monsterList = JsonUtility.FromJson<MonsterList>(monstersJson.text);
        }
        else
        {
            Debug.LogError("error loading monsters json");
        }
    }

    public Monster GetMonsterFromID(int itemID)
    {
        return monsterList.monsterList.Find(monster => monster.id == itemID);
    }

    public Sprite GetMonsterSprite(Monster monster)
    {
        return Resources.Load<Sprite>(monster.spritePath);
    }

    public Sprite GetMonsterProfileSprite(Monster monster)
    {
        return Resources.Load<Sprite>(monster.profileSpritePath);
    }

    public List<Monster> MonsterList
    {
        get { return new List<Monster>(monsterList.monsterList); }
    }
}