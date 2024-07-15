using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public GameObject ItemsPanel;

    public TMP_Text dreamCoinAmountText, specialCurrencyAmountText;
    private List<Item> shopItemList;
    public SelectedItemInfoCard card;
    public int selectedShopEntrySlot = -1;
    void Awake()
    {
        LoadCurrencies();
    }

    void Start()
    {
        shopItemList = new();
        for (int i = 1; i <= 8; i++)
        {
            shopItemList.Add(ItemManager.Instance.ItemList[i]);
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

    public void LoadCurrencies()
    {
        StartCoroutine(WaitForUserCoins());
    }

    private IEnumerator WaitForUserCoins()
    {
        while (string.IsNullOrEmpty(DataSaver.Instance.dts.userName))
        {
            yield return null;
        }
        dreamCoinAmountText.text = DataSaver.Instance.dts.dreamCoinAmount.ToString();
        specialCurrencyAmountText.text = DataSaver.Instance.dts.specialCurrencyAmount.ToString();

    }

    public void BuyItem()
    {
        if (selectedShopEntrySlot == -1)
        {
            return;
        }
        ShopEntry entry = ItemsPanel.transform.GetChild(selectedShopEntrySlot).GetComponent<ShopEntry>();
        Item item = entry.EntryItem;
        DataSaver.Instance.AddItemToInventory(item.itemID);
        DataSaver.Instance.DeductDreamCoins(item.price);
        card.ResetCard();
        LoadCurrencies();
        selectedShopEntrySlot = -1;
    }

}
