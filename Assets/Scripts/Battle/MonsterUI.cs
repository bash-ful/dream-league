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

        monsterMaxHP = monster.GetMaxHP();
    }

    void Update()
    {
        nameText.text = monster.GetName();
        damageText.text = monster.GetDamage().ToString();
        healthText.text = $"{(int)monster.GetHealth()}/{monsterMaxHP}";
    }


}
