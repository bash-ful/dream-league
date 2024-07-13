using TMPro;
using UnityEngine;

public class MonsterUI : MonoBehaviour
{
    public MonsterScript monster;
    public TMP_Text damageIndicator, nameText, healthText, damageText;
    private float monsterMaxHP;


    void Start()
    {

        monsterMaxHP = monster.GetMaxHP();
    }

    void Update()
    {
        nameText.text = monster.GetName();
        damageText.text = monster.GetModifiedDamage().ToString();
        healthText.text = $"{(int)monster.GetHealth()}/{monsterMaxHP}";
    }


}
