using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterUI : MonoBehaviour
{
    public MonsterScript monster;
    public TMP_Text damageIndicator, nameText, healthText, damageText;
    private float monsterMaxHP;

    void Start()
    {
        nameText.text = monster.GetName();
        monsterMaxHP = monster.GetMaxHP();
    }

    void Update()
    {
        damageText.text = monster.GetModifiedDamage().ToString();
        healthText.text = $"{(int)monster.GetHealth()}/{monsterMaxHP}";
    }



}
