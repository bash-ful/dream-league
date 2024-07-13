using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class SelectedItemInfoCard : MonoBehaviour
{
    public GameObject StarsPanel, BuffIconsPanel;
    public Image itemBackground, itemImage;
    public TMP_Text itemNameText, itemBuffDescriptionText, itemDebuffDescriptionText, itemPriceText;
    public ItemManager itemManager;
    private Image[] starImages, buffImages;

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
        UpdateItemImages();
        UpdateBuffIcons();
    }

    private void UpdateItemImages()
    {
        ImageTransparencyScript.UpdateImageTransparency(itemImage);
        ImageTransparencyScript.UpdateImageTransparency(itemBackground);
    }

    public void UpdateCard(int itemID)
    {
        ResetCard();
        Item item = itemManager.GetItemFromID(itemID);

        itemNameText.text = item.name;
        itemBuffDescriptionText.text = item.buffInfo;
        itemDebuffDescriptionText.text = item.debuffInfo;
        itemPriceText.text = item.price.ToString();
        itemImage.sprite = itemManager.GetItemSprite(item);
        itemBackground.sprite = itemManager.GetBackgroundSprite(item);
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
        Item item = itemManager.GetItemFromID(itemID);
        ResetBuffIcons();
        for (int i = 0; i < buffImages.Length && i < item.effects.Count; i++)
        {
            Enum.TryParse(item.effects[i].type, out EffectType eff);
            switch (eff)
            {
                case EffectType.HealPercentageOfMaxHP:
                    buffImages[i].sprite = Resources.Load<Sprite>("Aseprite/UI Scenes/Shop/Card/Item Effects Icon/Instant Heal");
                    break;
                case EffectType.DamagePercentageOfMaxHP:
                case EffectType.IncreaseSelfDamageDealtPercentage:
                case EffectType.IncreaseEnemyDamageDealtPercentage:
                    buffImages[i].sprite = Resources.Load<Sprite>("Aseprite/UI Scenes/Shop/Card/Item Effects Icon/Stronger Damage");
                    break;
                case EffectType.DecreaseSelfDamageDealtPercentage:
                case EffectType.DecreaseEnemyDamageDealtPercentage:
                    buffImages[i].sprite = Resources.Load<Sprite>("Aseprite/UI Scenes/Shop/Card/Item Effects Icon/Weaker Damage");
                    break;
                case EffectType.DecreaseSelfDamageTakenPercentage:
                case EffectType.DecreaseEnemyDamageTakenPercentage:
                    buffImages[i].sprite = Resources.Load<Sprite>("Aseprite/UI Scenes/Shop/Card/Item Effects Icon/Stronger Defense");
                    break;
                case EffectType.ReflectDamagePercentage:
                    buffImages[i].sprite = Resources.Load<Sprite>("Aseprite/UI Scenes/Shop/Card/Item Effects Icon/Reflect Damage");
                    break;
                case EffectType.ModifyTimerBySeconds:
                    buffImages[i].sprite = Resources.Load<Sprite>("Aseprite/UI Scenes/Shop/Card/Item Effects Icon/Time");
                    break;
                case EffectType.Stun:
                    buffImages[i].sprite = Resources.Load<Sprite>("Aseprite/UI Scenes/Shop/Card/Item Effects Icon/Stun");
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
}
