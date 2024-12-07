using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]

public class MonsterDeckManager : MonoBehaviour
{
    public GameObject MonsterDeckPanel;
    public SelectedMonsterCard card;

    private List<Monster> monsterList;
    public int selectedMonsterSlot = -1;

    void Awake()
    {
        ReloadInventory();
    }

    public void ReloadInventory()
    {
        StartCoroutine(WaitForInventoryLoad());
    }

    private IEnumerator WaitForInventoryLoad()
    {
        while (string.IsNullOrEmpty(DataSaver.Instance.dts.userName))
        {
            yield return null;
        }
        DataSaver.Instance.LoadDataFn();
        monsterList = DataSaver.Instance.dts.unlockedMonsters;
        Transform parentTransform = MonsterDeckPanel.transform;
        Monster monster;

        for (int i = 0; i < 9; i++)
        {
            Transform child = parentTransform.GetChild(i);
            MonsterDeckEntry monsterDeckEntry = child.GetComponent<MonsterDeckEntry>();
            monsterDeckEntry.GetComponent<Button>().interactable = false;
            monsterDeckEntry.monster = null;
            monsterDeckEntry.UpdateEntry();
        }

        for (int index = 0; index < monsterList.Count; index++)
        {
            Transform child = parentTransform.GetChild(index);
            MonsterDeckEntry monsterDeckEntry = child.GetComponent<MonsterDeckEntry>();
            monsterDeckEntry.GetComponent<Button>().interactable = true;
            monster = monsterList[index];
            monsterDeckEntry.monster = monster;
            monsterDeckEntry.UpdateEntry();
        }
    }

    public void EquipMonster()
    {
        if (selectedMonsterSlot == -1)
        {
            return;
        }
        MonsterDeckEntry entry = MonsterDeckPanel.transform.GetChild(selectedMonsterSlot).GetComponent<MonsterDeckEntry>();
        
        Monster monster = entry.monster;
        if (DataSaver.Instance.IsMonsterEquipped(monster.id))
        {
            DataSaver.Instance.RemoveMonsterFromEquipped(monster.id);
        }
        else
        {
            DataSaver.Instance.AddMonsterToEquipped(monster.id);
        }
        ReloadInventory();
        card.UpdateCard(DataSaver.Instance.dts.unlockedMonsters.FindIndex(x => x.id == monster.id));
    }

}
