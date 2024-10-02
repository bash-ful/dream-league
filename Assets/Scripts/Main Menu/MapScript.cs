using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScript : MonoBehaviour
{
    public GameObject mainMenu, mapMenu;

    public void OnMapClick() {
        if(DataSaver.Instance.IsInventoryFull()) {
            DataSaver.Instance.ShowInventoryFullPanel();
            return;
        }

        mainMenu.SetActive(false);
        mapMenu.SetActive(true);

    }
}
