using System;
using System.Collections.Generic;
using System.Transactions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SelectedItemInfoCard : MonoBehaviour
{
    public GameObject StarsPanel, BuffIconsPanel;
    public Image itemBackground, itemImage, currencyImage;
    public TMP_Text itemNameText, itemBuffDescriptionText, itemDebuffDescriptionText, itemPriceText;
    public TMP_Text isEquippedText;
    private Image[] starImages, buffImages;
    public Button transactionButton, equipButton;
    public int invEntry;
    public InventoryManager inventoryManager;

    #region REMOVETHIS
    public ShopManager shopManager;

    #endregion

    void OnDisable()
    {
        ResetCard();
    }

    public void Init()
    {
        starImages = StarsPanel.GetComponentsInChildren<Image>(true);
        buffImages = BuffIconsPanel.GetComponentsInChildren<Image>(true);
        ResetCard();
    }

    void OnEnable()
    {
        if (shopManager != null)
        {
            UpdateCard(shopManager.dreamCoinShopList[0].itemID, shopManager.isSpecialCurrencyShop);
        }
    }

    void Awake()
    {
        Init();
        if (shopManager != null)
        {
            UpdateCard(shopManager.dreamCoinShopList[0].itemID, false);
        }

    }
    public void ResetCard()
    {
        itemNameText.text = "";
        itemBuffDescriptionText.text = "";
        itemDebuffDescriptionText.text = "";
        itemPriceText.text = "";
        itemImage.sprite = null;
        itemBackground.sprite = null;
        if (equipButton != null)
        {
            TMP_Text equipButtonText = equipButton.GetComponentInChildren<TMP_Text>();
            if (equipButtonText != null && isEquippedText != null)
            {
                equipButtonText.text = "";
                isEquippedText.text = "";
            }
        }
        if (transactionButton != null)
        {
            transactionButton.gameObject.SetActive(false);

        }
        if (currencyImage != null)
        {
            currencyImage.sprite = null;
            ImageTransparencyScript.UpdateImageTransparency(currencyImage);
        }
        ResetBuffIcons();
        ResetStars();
        UpdateItemImages();
    }

    private void UpdateItemImages()
    {
        ImageTransparencyScript.UpdateImageTransparency(itemImage);
        ImageTransparencyScript.UpdateImageTransparency(itemBackground);
    }

    public void UpdateCurrency(bool isSpecialCurrencyShop)
    {
        if (isSpecialCurrencyShop)
        {
            currencyImage.sprite = Resources.Load<Sprite>("UI Scenes/Shop/Card/Icons/Special Currency Small");
        }
        else
        {
            currencyImage.sprite = Resources.Load<Sprite>("UI Scenes/Shop/Card/Icons/Dream Coin Small");
        }
        ImageTransparencyScript.UpdateImageTransparency(currencyImage);
    }

    public void UpdateCard(int itemID, bool isSpecialCurrencyShop)
    {
        ResetCard();
        UpdateCurrency(isSpecialCurrencyShop);
        Item item = ItemManager.Instance.GetItemFromID(itemID);

        itemNameText.text = item.name;
        itemBuffDescriptionText.text = item.buffInfo;
        itemDebuffDescriptionText.text = item.debuffInfo;
        int price = item.price;
        if (isSpecialCurrencyShop)
        {
            price /= 50;
        }
        itemPriceText.text = price.ToString();
        itemImage.sprite = ItemManager.Instance.GetItemSprite(item);
        itemBackground.sprite = ItemManager.Instance.GetBackgroundSprite(item);
        UpdateItemImages();
        ShowStars(item.rarity);
        ShowBuffIcons(item.itemID);
    }


    public void UpdateCard(string uniqueId, bool isSelling, bool isSpecialCurrencyShop)
    {
        ResetCard();
        TMP_Text equipButtonText = equipButton.GetComponentInChildren<TMP_Text>();
        equipButton.gameObject.SetActive(true);
        if (DataSaver.Instance.IsItemEquipped(uniqueId))
        {
            isEquippedText.text = "Equipped";
            equipButtonText.text = "Unequip";
            equipButton.interactable = true;
        }
        else if (DataSaver.Instance.dts.equippedItems.Count >= 3)
        {
            isEquippedText.text = "";
            equipButtonText.text = "Slots Full";
            equipButton.interactable = false;
        }
        else
        {
            isEquippedText.text = "";
            equipButtonText.text = "Equip";
            equipButton.interactable = true;
        }
        InventoryItem invItem = DataSaver.Instance.dts.inventory.Find(x => x.uniqueId == uniqueId);
        Item item = ItemManager.Instance.GetItemFromID(invItem.itemId);

        itemNameText.text = item.name;
        itemBuffDescriptionText.text = item.buffInfo;
        itemDebuffDescriptionText.text = item.debuffInfo;
        int price = item.price;
        if (isSelling)
        {
            price /= 2;
        }
        itemPriceText.text = price.ToString();
        itemImage.sprite = ItemManager.Instance.GetItemSprite(item);
        itemBackground.sprite = ItemManager.Instance.GetBackgroundSprite(item);
        UpdateItemImages();
        ShowStars(item.rarity);
        transactionButton.gameObject.SetActive(true);
        ShowBuffIcons(item.itemID);
    }

    public void ShowStars(int rarity)
    {

        for (int i = 0; i < starImages.Length; i++)
        {
            starImages[i].gameObject.SetActive(i < rarity);
        }
    }

    public void ShowBuffIcons(int itemID)
    {
        Item item = ItemManager.Instance.GetItemFromID(itemID);
        ResetBuffIcons();
        for (int i = 0; i < buffImages.Length && i < item.effects.Count; i++)
        {
            Effect effect = item.effects[i];
            Enum.TryParse(effect.type, out EffectType eff);
            switch (eff)
            {
                case EffectType.HealPlayerPercentageOfMaxHP:
                    buffImages[i].sprite = Resources.Load<Sprite>("UI Scenes/Shop/Card/Item Effects Icon/Instant Heal");
                    break;
                case EffectType.DamageEnemyPercentageOfMaxHP:
                case EffectType.DamagePlayerPercentageOfMaxHP:
                case EffectType.ModifySelfDamageDealtModifier:
                case EffectType.ModifyEnemyDamageDealtModifier:
                    if (effect.value > 0)
                    {
                        buffImages[i].sprite = Resources.Load<Sprite>("UI Scenes/Shop/Card/Item Effects Icon/Stronger Damage");
                    }
                    else
                    {
                        buffImages[i].sprite = Resources.Load<Sprite>("UI Scenes/Shop/Card/Item Effects Icon/Weaker Damage");
                    }
                    break;
                case EffectType.ModifySelfDamageTakenModifier:
                case EffectType.ModifyEnemyDamageTakenModifier:
                    if (effect.value > 0)
                    {
                        buffImages[i].sprite = Resources.Load<Sprite>("UI Scenes/Shop/Card/Item Effects Icon/Stronger Defense");
                    }
                    else
                    {
                    }
                    break;
                case EffectType.ReflectDamage:
                    buffImages[i].sprite = Resources.Load<Sprite>("UI Scenes/Shop/Card/Item Effects Icon/Reflect Damage");
                    break;
                case EffectType.ModifyTimerBySeconds:
                    buffImages[i].sprite = Resources.Load<Sprite>("UI Scenes/Shop/Card/Item Effects Icon/Time");
                    break;
                case EffectType.Stun:
                    buffImages[i].sprite = Resources.Load<Sprite>("UI Scenes/Shop/Card/Item Effects Icon/Stun");
                    break;
            }

            UpdateBuffIcons();
        }
    }

    private void UpdateBuffIcons()
    {
        for (int i = 0; i < buffImages.Length; i++)
        {
            ImageTransparencyScript.UpdateImageTransparency(buffImages[i]);
        }
    }

    private void ResetBuffIcons()
    {
        foreach (Image image in buffImages)
        {
            image.sprite = null;
            ImageTransparencyScript.UpdateImageTransparency(image);
        }
    }

    private void ResetStars()
    {
        foreach (Image image in starImages)
        {
            image.gameObject.SetActive(false);
        }
    }

}