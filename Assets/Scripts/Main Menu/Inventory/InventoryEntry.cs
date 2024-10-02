using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryEntry : MonoBehaviour
{
    public InventoryItem invItem;
    public InventoryManager inventoryManager;
    public int inventoryIndex;

    public SelectedItemInfoCard card;


    public void UpdateEntry()
    {
        if (invItem == null)
        {
            GetComponent<Button>().image.sprite = null;
            GetComponentInChildren<TMP_Text>(true).gameObject.SetActive(false);
            ImageTransparencyScript.UpdateImageTransparency(GetComponent<Button>().image);
            return;
        }
        if (DataSaver.Instance.IsItemEquipped(invItem.uniqueId))
        {
            GetComponentInChildren<TMP_Text>(true).gameObject.SetActive(true);
            int slotIndex = DataSaver.Instance.dts.equippedItems.FindIndex(x => x.uniqueId == invItem.uniqueId) + 1;
            GetComponentInChildren<TMP_Text>(true).text = slotIndex.ToString();
        }
        else
        {
            GetComponentInChildren<TMP_Text>(true).gameObject.SetActive(false);
        }
        Item item = ItemManager.Instance.GetItemFromID(invItem.itemId);
        Sprite itemSprite = Resources.Load<Sprite>(item.spriteResourcesPath);
        GetComponent<Button>().image.sprite = itemSprite;
        ImageTransparencyScript.UpdateImageTransparency(GetComponent<Button>().image);
        GetComponent<Button>().image.preserveAspect = true;
    }

    public void ClearEntry()
    {
        invItem = null;
        UpdateEntry();
    }

    public void OnImagePress()
    {
        card.UpdateCard(invItem.uniqueId, true, false);
        inventoryManager.selectedInventorySlot = inventoryIndex;
    }
}
