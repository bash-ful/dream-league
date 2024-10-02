using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class InventoryItem
{
    public int itemId;
    public string uniqueId;
}
public class InventoryManager : MonoBehaviour
{
    public GameObject ItemsPanel;
    public TMP_Text dreamCoinAmountText, specialCurrencyAmountText;
    public SelectedItemInfoCard card;

    private List<InventoryItem> inventoryList;

    public int selectedInventorySlot = -1;

    void Awake()
    {
        ReloadInventory();
    }

    public void ReloadInventory()
    {
        StartCoroutine(WaitForInventoryLoad());
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

    private IEnumerator WaitForInventoryLoad()
    {
        while (string.IsNullOrEmpty(DataSaver.Instance.dts.userName))
        {
            yield return null;
        }
        DataSaver.Instance.LoadDataFn();
        inventoryList = DataSaver.Instance.dts.inventory;
        InventoryItem inventoryItem;
        Transform parentTransform = ItemsPanel.transform;
        int childCount = parentTransform.childCount;

        for (int i = 0; i < 40; i++)
        {
            Transform child = parentTransform.GetChild(i);
            InventoryEntry inventoryEntry = child.GetComponent<InventoryEntry>();
            inventoryEntry.GetComponent<Button>().interactable = false;
            inventoryEntry.invItem = null;
            inventoryEntry.UpdateEntry();
        }

        for (int index = 0; index < inventoryList.Count; index++)
        {
            Transform child = parentTransform.GetChild(index);
            InventoryEntry inventoryEntry = child.GetComponent<InventoryEntry>();
            inventoryEntry.GetComponent<Button>().interactable = true;
            inventoryItem = inventoryList[index];


            inventoryEntry.invItem = inventoryItem;
            inventoryEntry.UpdateEntry();

        }
    }

    public void SellItem()
    {
        if (selectedInventorySlot == -1)
        {
            return;
        }
        InventoryEntry entry = ItemsPanel.transform.GetChild(selectedInventorySlot).GetComponent<InventoryEntry>();
        InventoryItem invItem = DataSaver.Instance.dts.inventory.Find(x => x.uniqueId == entry.invItem.uniqueId);
        Item item = ItemManager.Instance.GetItemFromID(invItem.itemId);
        DataSaver.Instance.AddDreamCoins(item.price / 2);
        ItemsPanel.transform.GetChild(selectedInventorySlot).GetComponent<InventoryEntry>().ClearEntry();
        DataSaver.Instance.RemoveItemFromInventory(invItem.uniqueId);
        ReloadInventory();
        card.ResetCard();
        selectedInventorySlot = -1;
    }

    public void EquipItem()
    {
        if (selectedInventorySlot == -1)
        {
            return;
        }
        InventoryEntry entry = ItemsPanel.transform.GetChild(selectedInventorySlot).GetComponent<InventoryEntry>();
        InventoryItem invItem = DataSaver.Instance.dts.inventory.Find(x => x.uniqueId == entry.invItem.uniqueId);
        if (DataSaver.Instance.IsItemEquipped(invItem.uniqueId))
        {
            DataSaver.Instance.RemoveItemFromEquipped(invItem.uniqueId);
        }
        else
        {
            DataSaver.Instance.AddItemToEquipped(invItem.uniqueId);
        }
        ReloadInventory();
        card.UpdateCard(invItem.uniqueId, true, false);
    }

}
