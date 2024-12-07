using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterSelector : MonoBehaviour
{
    private int selected = 1;
    // starter monster ids are 30-32
    private int offset = 30;
    public GameObject[] borders;
    public GameObject MonsterSelectMenu;
    void Start()
    {
        DataSaver.Instance.LoadDataFn();
        if(DataSaver.Instance.dts.starterReceived) {
            return;
        }
        MonsterSelectMenu.SetActive(true);
        SelectMonster(1);
    }

    public void SelectMonster(int index) {
        selected = index;
        int i = 0;
        while(i < borders.Length) {
            if(i != index) {
                borders[i].SetActive(false);
            } else {
                borders[i].SetActive(true);
            }
            i++;
        }
    }

    public void ChooseMonster() {
        DataSaver.Instance.AddMonsterToInventory(selected + offset);
        DataSaver.Instance.AddMonsterToEquipped(selected + offset);
        DataSaver.Instance.dts.starterReceived = true;
        DataSaver.Instance.SaveDataFn();
    }
}
