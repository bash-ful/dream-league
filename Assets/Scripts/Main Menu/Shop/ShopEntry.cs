using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopEntry : MonoBehaviour
{
    private Item item;
    public TMP_Text entryPriceText;
    public Image entryImage;
    private Sprite itemSprite;
    public SelectedItemInfoCard card;
    public ShopManager shopManager;
    public int entryIndex;


    public Item EntryItem
    {
        get { return item; }
        set { item = value; }
    }

    public void UpdateEntry()
    {
        entryPriceText.text = item.price.ToString();
        itemSprite = Resources.Load<Sprite>(item.spriteResourcesPath);
        entryImage.sprite = itemSprite;
        ImageTransparencyScript.UpdateImageTransparency(entryImage);
        entryImage.preserveAspect = true;
    }

    public void OnImagePress()
    {
        shopManager.selectedShopEntrySlot = entryIndex;
        card.UpdateCard(item.itemID, false);
    }
}