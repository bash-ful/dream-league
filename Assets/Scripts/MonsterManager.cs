
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Monster
{
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
        Init();
    }
    public void Init()
    {
        TextAsset monstersJson = Resources.Load<TextAsset>("Json/Monsters");

        if (monstersJson != null)
        {
            monsterList = JsonUtility.FromJson<MonsterList>(monstersJson.text);
        } else {
            Debug.LogError("error loading monsters json");
        }
    }

    public Monster GetMonsterFromID(int monsterID) {
        return monsterList.monsterList[monsterID];
    }
}