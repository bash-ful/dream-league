using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ItemSlot
{
    public string itemGUID;
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
    public MasterScript masterScript;
    public AudioSource itemUse;

    void Start()
    {

        Init();
    }

    private void Init()
    {
        Sprite itemSpriteToAdd;
        Item item;
        DataSaver.Instance.LoadDataFn();
        List<InventoryItem> equippedItems = DataSaver.Instance.dts.equippedItems;
        for (int i = 0; i < 3; i++)
        {
            if (i >= equippedItems.Count)
            {
                itemSlots.itemSlots[i].button.interactable = false;
                ImageTransparencyScript.UpdateImageTransparency(itemSlots.itemSlots[i].button.image);
                continue;
            }
            InventoryItem equippedItem = equippedItems[i];
            item = ItemManager.Instance.GetItemFromID(equippedItem.itemId);
            if (item == null)
            {
                ImageTransparencyScript.UpdateImageTransparency(itemSlots.itemSlots[i].button.image);
                continue;
            }
            itemSpriteToAdd = ItemManager.Instance.GetItemSprite(item);
            if (itemSpriteToAdd == null)
            {
                ImageTransparencyScript.UpdateImageTransparency(itemSlots.itemSlots[i].button.image);
            }
            else
            {
                itemSlots.itemSlots[i].button.image.sprite = itemSpriteToAdd;
            }
            itemSlots.itemSlots[i].itemGUID = equippedItem.uniqueId;
            itemSlots.itemSlots[i].item = item;

        }
    }

    public void OnItemSlotClick(int buttonIndex)
    {
        masterScript.UseItem(itemSlots.itemSlots[buttonIndex].item.itemID);
        itemUse.Play();
        DataSaver.Instance.RemoveItemFromInventory(itemSlots.itemSlots[buttonIndex].itemGUID);
        itemSlots.itemSlots[buttonIndex].button.image.sprite = null;
        ImageTransparencyScript.UpdateImageTransparency(itemSlots.itemSlots[buttonIndex].button.image);
    }
}
