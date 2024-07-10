using UnityEngine;
using UnityEngine.UI;

public class HPBarController : MonoBehaviour
{
    public Image healthRemainingBar;
    public MonsterScript monster;
    private float currentHP, maxHP;

    void Update() {
        currentHP = monster.GetHealth();
        maxHP = monster.GetMaxHP();
        healthRemainingBar.fillAmount = currentHP/maxHP;
    }

}

