using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterUI : MonoBehaviour
{
    public MonsterScript monster;
    // public DamageIndicatorPool damageIndicatorPool;
    public Text nameText, healthText, damageText;
    public TMP_Text damageIndicator;


    void Update()
    {
        nameText.text = monster.GetName();
        healthText.text = $"{(int)monster.GetHealth()}/{(int)monster.GetMaxHP()}";
        damageText.text = monster.GetDamage().ToString();
    }

    // public void ShowDamageIndicator(float damage){
    //     damageIndicatorPool.ShowDamageIndicatorScript(transform.position, damage);
    // }
}
