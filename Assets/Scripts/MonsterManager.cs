
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Monster
{
    public int id;
    public string name, spritePath, spritePathFirstFrame;
    public float baseHealth, baseDamage;
}

[System.Serializable]
public class MonsterList
{
    public List<Monster> monsterList;
}

public class MonsterManager : MonoBehaviour
{
    private MonsterList monsterList;

    void Start()
    {
        print("MonsterManager Init");
        Init();
    }
    public void Init()
    {
        TextAsset monstersJson = Resources.Load<TextAsset>("Json/Monsters");

        if (monstersJson != null)
        {
            monsterList = JsonUtility.FromJson<MonsterList>(monstersJson.text);
            foreach (Monster monster in monsterList.monsterList)
            {
                print($"name: {monster.name}");
            }
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
}