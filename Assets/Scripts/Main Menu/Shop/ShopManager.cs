using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public ItemManager itemManager;
    public GameObject ItemsPanel;

    private List<Item> shopItemList;

    void Start()
    {
        shopItemList = new();
        for (int i = 1; i <= 8; i++)
        {
            shopItemList.Add(itemManager.GetItemList()[i]);
        }
        Transform parentTransform = ItemsPanel.transform;
        int childCount = parentTransform.childCount;

        for (int index = 0; index < childCount; index++)
        {
            Transform child = parentTransform.GetChild(index);
            ShopEntry shopEntry = child.GetComponent<ShopEntry>();
            shopEntry.EntryItem = shopItemList[index];
            shopEntry.UpdateEntry();
        }
    }

}
