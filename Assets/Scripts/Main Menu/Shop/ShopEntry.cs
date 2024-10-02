using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopEntry : MonoBehaviour
{
    private Item item;
    public TMP_Text entryPriceText;
    public Image entryImage;
    public Button entryButton;
    private Sprite itemSprite;
    public SelectedItemInfoCard card;
    public ShopManager shopManager;
    public int entryIndex;

    public Item EntryItem
    {
        get { return item; }
        set { item = value; }
    }

    public void UpdateEntry(bool isSpecialCurrencyShop)
    {
        if (item == null)
        {
            ClearEntry();
            entryButton.interactable = false;
            return;
        }
        entryButton.interactable = true;
        int price = item.price;
        if (isSpecialCurrencyShop)
        {
            price /= 50;
        }
        entryPriceText.text = price.ToString();
        itemSprite = Resources.Load<Sprite>(item.spriteResourcesPath);
        entryImage.sprite = itemSprite;
        ImageTransparencyScript.UpdateImageTransparency(entryImage);
        entryImage.preserveAspect = true;
    }

    public void ClearEntry()
    {
        entryPriceText.text = "";
        entryImage.sprite = null;
        ImageTransparencyScript.UpdateImageTransparency(entryImage);
    }

    public void OnImagePress(bool isSpecialCurrencyShop)
    {
        shopManager.selectedShopEntrySlot = entryIndex;
        if (isSpecialCurrencyShop)
        {
            shopManager.isSpecialCurrencyShop = true;
            
        }
        else
        {
            shopManager.isSpecialCurrencyShop = false;
        }
        card.UpdateCard(item.itemID, isSpecialCurrencyShop);
    }
}
