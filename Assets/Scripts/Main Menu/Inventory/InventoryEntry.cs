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
            print("inv item is null now!");
            GetComponent<Button>().image.sprite = null;
            ImageTransparencyScript.UpdateImageTransparency(GetComponent<Button>().image);
            return;
        }
        Item item = ItemManager.Instance.GetItemFromID(invItem.itemId);
        Sprite itemSprite = Resources.Load<Sprite>(item.spriteResourcesPath);
        GetComponent<Button>().image.sprite = itemSprite;
        ImageTransparencyScript.UpdateImageTransparency(GetComponent<Button>().image);
        GetComponent<Button>().image.preserveAspect = true;
    }

    public void ClearEntry() {
        invItem = null;
        UpdateEntry();
    }

    public void OnImagePress()
    {
        card.UpdateCard(invItem.itemId, true);
        inventoryManager.selectedInventorySlot = inventoryIndex;
    }
}
