using UnityEngine;

public class MonsterScript : MonoBehaviour
{
    public int monsterID;

    private float health, maxHP, baseDamage, modifiedDamage;
    private float damageTakenModifier = 1;
    private float damageDealtModifier = 1;

    private string playerName, spritePath;
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    public FloatingIndicator damageIndicator;
    private readonly MonsterManager monsterManager;

    public GameObject damageIndicatorSpawnPosition;

    void Update()
    {
        modifiedDamage = baseDamage * damageDealtModifier;
    }
    public void MonsterInit()
    {
        Monster playerMonster = monsterManager.GetMonsterFromID(monsterID);
        playerName = playerMonster.name;
        health = playerMonster.baseHealth;
        maxHP = health;
        baseDamage = playerMonster.baseDamage;
        spritePath = playerMonster.spritePath;
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
        return baseDamage;
    }

    public float GetModifiedDamage() {
        return modifiedDamage;
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
