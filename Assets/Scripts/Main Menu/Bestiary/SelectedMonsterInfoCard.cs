using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class SelectedMonsterInfoCard : MonoBehaviour
{
    public Image monsterImage, frameImage;
    public TMP_Text monsterNameText, monsterHPText, monsterDamageText, monsterAbilityNameText, monsterAbilityDescriptionText;

    void Start()
    {
        ResetCard();
    }
    public void ResetCard()
    {
        monsterNameText.text = "";
        monsterHPText.text = "";
        monsterDamageText.text = "";
        monsterAbilityNameText.text = "";
        monsterAbilityDescriptionText.text = "";
        monsterImage.sprite = null;
        frameImage.sprite = null;
        UpdateMonsterImages();
    }

    private void UpdateMonsterImages()
    {
        ImageTransparencyScript.UpdateImageTransparency(monsterImage);
        ImageTransparencyScript.UpdateImageTransparency(frameImage);
    }

    public void UpdateFrame(int chapter)
    {
        frameImage.sprite = chapter switch
        {
            1 => Resources.Load<Sprite>("UI Scenes/Bestiary/HUD/Chapter 1/Monster_Frame/Monster_Frame"),
            2 => Resources.Load<Sprite>("UI Scenes/Bestiary/HUD/Chapter 2/Monster_Frame/Monster_Frame"),
            3 => Resources.Load<Sprite>("UI Scenes/Bestiary/HUD/Chapter 3/Monster_Frame/Monster_Frame"),
            4 => Resources.Load<Sprite>("UI Scenes/Bestiary/HUD/Chapter 4/Monster_Frame/Monster_Frame"),
            _ => Resources.Load<Sprite>("UI Scenes/Bestiary/HUD/Chapter 1/Monster_Frame/Monster_Frame"),
        };
    }

    public int GetMonsterChapter(int monsterID)
    {
        if (monsterID < 10)
        {
            return 1;
        }
        else if (monsterID < 20)
        {
            return 2;
        }
        else if (monsterID < 30)
        {
            return 3;
        }
        else if (monsterID < 40)
        {
            return 4;
        }
        return 1;
    }

    public void UpdateCard(int monsterID)
    {
        ResetCard();
        Monster monster = MonsterManager.Instance.GetMonsterFromID(monsterID);
        print($"displaying monster: {monster.name}");

        monsterNameText.text = monster.name;
        monsterHPText.text = monster.baseHealth.ToString();
        monsterDamageText.text = monster.baseDamage.ToString();
        monsterAbilityNameText.text = monster.abilityName;
        monsterAbilityDescriptionText.text = monster.abilityDescription;
        monsterImage.sprite = MonsterManager.Instance.GetMonsterSprite(monster);
        int chapter = GetMonsterChapter(monsterID);
        UpdateFrame(chapter);
        UpdateMonsterImages();
    }

}
