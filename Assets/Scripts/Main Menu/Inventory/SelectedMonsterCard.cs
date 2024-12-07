using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectedMonsterCard : MonoBehaviour
{
    public Image monsterImage;
    // public Image monsterBackground;
    public TMP_Text monsterNameText, monsterHealthText, monsterDamageText;
    public TMP_Text[] moves;
    public Button equipButton;
    public GameObject FrontCard, BackCard;
    private int currentIndex = 0;
    private bool toggle = false;

    void OnDisable()
    {
        ResetCard();
    }

    public void Init()
    {
        ResetCard();
    }

    public void ToggleCard() {
        FrontCard.SetActive(toggle);
        BackCard.SetActive(!toggle);
        toggle = !toggle;
        UpdateCard(currentIndex);
    }

    void Awake()
    {
        Init();
    }
    public void ResetCard()
    {
        monsterNameText.text = "";
        monsterHealthText.text = "";
        monsterDamageText.text = "";
        monsterImage.sprite = null;
        // monsterBackground.sprite = null;
        if (equipButton != null)
        {
            TMP_Text equipButtonText = equipButton.GetComponentInChildren<TMP_Text>();
            if (equipButtonText != null)
            {
                equipButtonText.text = "";
            }
        }
        UpdateItemImages();
    }

    private void UpdateItemImages()
    {
        ImageTransparencyScript.UpdateImageTransparency(monsterImage);
        // ImageTransparencyScript.UpdateImageTransparency(monsterBackground);
    }

    public void UpdateCard(int monsterIndex)
    {
        currentIndex = monsterIndex;
        ResetCard();
        TMP_Text equipButtonText = equipButton.GetComponentInChildren<TMP_Text>();
        equipButton.gameObject.SetActive(true);
        List<Monster> monsterList = DataSaver.Instance.dts.unlockedMonsters;
        Monster compare = monsterList[monsterIndex];

        if (DataSaver.Instance.IsMonsterEquipped(compare.id))
        {
            equipButtonText.text = "Unequip";
            equipButton.interactable = true;
        }
        else if (DataSaver.Instance.dts.equippedMonsters.Count >= 3)
        {
            equipButtonText.text = "Slots Full";
            equipButton.interactable = false;
        }
        else
        {
            equipButtonText.text = "Equip";
            equipButton.interactable = true;
        }

        monsterNameText.text = compare.name;
        monsterHealthText.text = compare.baseHealth.ToString();
        monsterDamageText.text = compare.baseDamage.ToString();
        for(int i = 0; i < 4; i++) {
            moves[i].text = MoveManager.Instance.GetMoveFromID(compare.moves[i]).name;
        }

        monsterImage.sprite = MonsterManager.Instance.GetMonsterSprite(compare);
        UpdateItemImages();
    }
}