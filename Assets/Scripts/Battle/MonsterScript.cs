using UnityEditor.U2D.Aseprite;
using UnityEngine;

[System.Serializable]
public class Monster
{
    public string name, spritePath, spritePathFirstFrame;
    public float baseHealth, baseDamage;
}

[System.Serializable]
public class MonsterList
{
    public Monster[] monsterList;
}
public class MonsterScript : MonoBehaviour
{
    public int monsterID;

    public float health, damage, maxHP, baseDamage;
    public float damageTakenModifier = 1;
    public float damageDealtModifier = 1;

    private string playerName, spritePath;
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    public FloatingIndicator damageIndicator;

    public GameObject damageIndicatorSpawnPosition;

    public void MonsterInit(MonsterList monsterList)
    {
        playerName = monsterList.monsterList[monsterID].name;
        health = monsterList.monsterList[monsterID].baseHealth;
        maxHP = health;
        damage = monsterList.monsterList[monsterID].baseDamage;
        spritePath = monsterList.monsterList[monsterID].spritePath;
        Sprite newSprite = Resources.Load<Sprite>(spritePath);
        RuntimeAnimatorController animController = Resources.Load<RuntimeAnimatorController>(spritePath);

        if (animController != null)
        {
            animator.runtimeAnimatorController = animController;
        }
        else
        {
            Debug.LogError("cannot find animcontroller");
        }

        if (newSprite != null)
        {
            spriteRenderer.sprite = newSprite;
        }
        else
        {
            Debug.LogError("Failed to load sprite from Resources folder.");
        }

    }

    public void TakeDamage(float damage)
    {
        health = Mathf.Clamp(health - (damage * damageTakenModifier), 0, health);
        damageIndicator.ShowIndicator($"DMG {damage}", IndicatorType.BaseDamage, damageIndicatorSpawnPosition.transform.position);
    }

    public void DealDamage(MonsterScript opponent)
    {
        float modifiedDamage = damage * damageDealtModifier;
        opponent.TakeDamage(modifiedDamage);

    }

    public void Heal(float healAmount)
    {
        if (healAmount <= 0)
        {
            return;
        }

        float currentHealth = GetHealth();
        float maxHP = GetMaxHP();

        float amountHealed = Mathf.Min(healAmount, maxHP - currentHealth); // Calculate actual amount that can be healed

        health += amountHealed; // Increase health by the healed amount
        damageIndicator.ShowIndicator($"HEAL {amountHealed}", IndicatorType.Heal, damageIndicatorSpawnPosition.transform.position);
    }

    public bool IsDead()
    {
        return health == 0;
    }

    public float GetHealth()
    {
        return health;
    }

    public string GetName()
    {
        return playerName;
    }

    public float GetDamage()
    {
        return damage;
    }

    public float GetMaxHP()
    {
        return maxHP;
    }

    public float DamageTakenModifier
    {
        get { return damageTakenModifier; }
        set { damageTakenModifier = value; }
    }
    public float DamageDealtModifier
    {
        get { return damageDealtModifier; }
        set { damageDealtModifier = value; }
    }
}
