using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectedItemInfoCard : MonoBehaviour
{
    public GameObject StarsPanel, BuffIconsPanel;
    public Image itemBackground, itemImage;
    public TMP_Text itemNameText, itemBuffDescriptionText, itemDebuffDescriptionText, itemPriceText;
    private Image[] starImages, buffImages;
    public int invEntry;
    public InventoryManager inventoryManager;

    void OnDisable()
    {
        ResetCard();
    }

    void Start()
    {
        starImages = StarsPanel.GetComponentsInChildren<Image>(true);
        buffImages = BuffIconsPanel.GetComponentsInChildren<Image>(true);
        ResetCard();

    }
    public void ResetCard()
    {
        itemNameText.text = "";
        itemBuffDescriptionText.text = "";
        itemDebuffDescriptionText.text = "";
        itemPriceText.text = "";
        itemImage.sprite = null;
        itemBackground.sprite = null;
        ResetBuffIcons();
        ResetStars();
        UpdateItemImages();
    }

    private void UpdateItemImages()
    {
        ImageTransparencyScript.UpdateImageTransparency(itemImage);
        ImageTransparencyScript.UpdateImageTransparency(itemBackground);
    }

    public void UpdateCard(int itemID, bool isSelling)
    {
        ResetCard();
        Item item = ItemManager.Instance.GetItemFromID(itemID);

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
                case EffectType.HealPercentageOfMaxHP:
                    buffImages[i].sprite = Resources.Load<Sprite>("UI Scenes/Shop/Card/Item Effects Icon/Instant Heal");
                    break;
                case EffectType.DamagePercentageOfMaxHP:
                case EffectType.ModifySelfDamageDealtModifier:
                case EffectType.ModifyEnemyDamageDealtModifier:
                    if (effect.value > 1)
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
                    if (effect.value > 1)
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
