using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class MonsterEntry
{
    public Image profile;
}
[System.Serializable]
public class MonsterEntryList
{
    public List<MonsterEntry> monsterEntryList;
}
public class MonsterEntryManager : MonoBehaviour
{
    public MonsterEntryList monsterEntryList;
    public int multiplier = 0;
    public SelectedMonsterInfoCard card;

    void Start()
    {
        UpdateMonsterEntries();
    }
    public void OnImagePress(int monsterID)
    {
        card.UpdateCard(monsterID + (10 * multiplier));
    }

    public void UpdateMonsterEntries()
    {
        Monster monster;
        for (int i = 0; i < monsterEntryList.monsterEntryList.Count; i++)
        {
            monster = MonsterManager.Instance.GetMonsterFromID(i + (10 * multiplier));
            Sprite spriteToAdd = MonsterManager.Instance.GetMonsterProfileSprite(monster);
            if (spriteToAdd != null)
            {
                monsterEntryList.monsterEntryList[i].profile.sprite = spriteToAdd;
            }
            ImageTransparencyScript.UpdateImageTransparency(monsterEntryList.monsterEntryList[i].profile);
        }
    }

    public void OnChapterPress(int chapter)
    {
        multiplier = chapter;
        UpdateMonsterEntries();
        card.ResetCard();
    }

    public void ResetMultiplier() {
        multiplier = 0;
        UpdateMonsterEntries();
    }
}
