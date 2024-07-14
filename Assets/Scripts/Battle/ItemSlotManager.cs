using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ItemSlot
{
    public Item item;
    public Button button;
}
[System.Serializable]
public class ItemSlots
{
    public ItemSlot[] itemSlots;
}
public class ItemSlotManager : MonoBehaviour
{

    public ItemSlots itemSlots;
    public ItemManager itemManager;
    public MasterScript masterScript;

    void Start()
    {

        Init();
    }

    private void Init()
    {
        itemManager.Init();
        Sprite itemSpriteToAdd;
        Item item;
        for (int i = 0; i < 3; i++)
        {
            int randItemIndex = Random.Range(0, 8);
            item = itemManager.GetItemFromID(randItemIndex);
            if (item == null)
            {
                ImageTransparencyScript.UpdateImageTransparency(itemSlots.itemSlots[i].button.image);
                continue;
            }

            itemSpriteToAdd = itemManager.GetItemSprite(item);
            if (itemSpriteToAdd == null)
            {
                ImageTransparencyScript.UpdateImageTransparency(itemSlots.itemSlots[i].button.image);
            }
            else
            {
                itemSlots.itemSlots[i].button.image.sprite = itemSpriteToAdd;
            }

            itemSlots.itemSlots[i].item = item;

        }
    }

    public void OnItemSlotClick(int buttonIndex)
    {
        masterScript.UseItem(itemSlots.itemSlots[buttonIndex].item.itemID);
    }
}
