using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

public class ShopManager : MonoBehaviour
{
    public GameObject DreamCoinShopPanel, SpecialCurrencyShopPanel, InventoryFullPanel;

    public TMP_Text dreamCoinAmountText, specialCurrencyAmountText;
    public List<Item> dreamCoinShopList, specialCurrencyShopList;
    public SelectedItemInfoCard card;
    public int selectedShopEntrySlot = 0;
    public bool isSpecialCurrencyShop = false;
    void Awake()
    {
        LoadCurrencies();
        InitDreamCoinShop();
        InitSpecialCoinShop();
    }

    private void ScrambleList<T>(List<T> list)
    {
        // Use Fisher-Yates shuffle algorithm
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (list[n], list[k]) = (list[k], list[n]);
        }
    }

    private void InitDreamCoinShop()
    {
        dreamCoinShopList = ItemManager.Instance.ItemList.Where(x => x.rarity < 5).ToList();
        ScrambleList(dreamCoinShopList);
        Transform parentTransform = DreamCoinShopPanel.transform;
        int childCount = parentTransform.childCount;

        for (int index = 0; index < 8; index++)
        {
            Transform child = parentTransform.GetChild(index);

            ShopEntry shopEntry = child.GetComponent<ShopEntry>();
            if (dreamCoinShopList[index] == null)
            {
                shopEntry.ClearEntry();
                continue;
            }
            shopEntry.EntryItem = dreamCoinShopList[index];
            shopEntry.UpdateEntry(false);
        }

        for (int i = 0; i < childCount; i++)
        {
            Transform child = parentTransform.GetChild(i);
            ShopEntry shopEntry = child.GetComponent<ShopEntry>();
            if (shopEntry.EntryItem == null)
            {
                shopEntry.UpdateEntry(false);
            }
        }
    }

    private void InitSpecialCoinShop()
    {
        specialCurrencyShopList = ItemManager.Instance.ItemList.Where(x => x.rarity == 5).ToList();
        Transform parentTransform = SpecialCurrencyShopPanel.transform;
        int childCount = parentTransform.childCount;

        for (int index = 0; index < specialCurrencyShopList.Count; index++)
        {
            Transform child = parentTransform.GetChild(index);

            ShopEntry shopEntry = child.GetComponent<ShopEntry>();
            if (specialCurrencyShopList[index] == null)
            {
                shopEntry.ClearEntry();
                continue;
            }
            shopEntry.EntryItem = specialCurrencyShopList[index];
            shopEntry.UpdateEntry(true);
        }

        for (int i = 0; i < childCount; i++)
        {
            Transform child = parentTransform.GetChild(i);
            ShopEntry shopEntry = child.GetComponent<ShopEntry>();
            if (shopEntry.EntryItem == null)
            {
                shopEntry.UpdateEntry(true);
            }
        }
    }

    void Start()
    {
        SpecialCurrencyShopPanel.SetActive(false);
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
        if (DataSaver.Instance.IsInventoryFull())
        {
            DataSaver.Instance.ShowInventoryFullPanel();
            return;
        }
        ShopEntry entry = DreamCoinShopPanel.transform.GetChild(selectedShopEntrySlot).GetComponent<ShopEntry>();

        if (isSpecialCurrencyShop)
        {
            entry = SpecialCurrencyShopPanel.transform.GetChild(selectedShopEntrySlot).GetComponent<ShopEntry>();
        }
        Item item = entry.EntryItem;
        int price = item.price;



        if (isSpecialCurrencyShop)
        {
            price /= 50;
            DataSaver.Instance.DeductSpecialCurrency(price);
        }
        else
        {
            DataSaver.Instance.DeductDreamCoins(price);
        }
        DataSaver.Instance.AddItemToInventory(item.itemID);
        LoadCurrencies();
    }

    public void LoadSpecialCurrencyShop()
    {
        DreamCoinShopPanel.SetActive(false);
        SpecialCurrencyShopPanel.SetActive(true);
        isSpecialCurrencyShop = true;
    }
    public void LoadDreamCoinShop()
    {
        DreamCoinShopPanel.SetActive(true);
        SpecialCurrencyShopPanel.SetActive(false);
        isSpecialCurrencyShop = false;
    }

}
