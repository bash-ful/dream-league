using System.Collections;
using UnityEngine;

public class MonsterScript : MonoBehaviour
{
    public bool isPlayer;
    public int monsterID;
    #region Monster Stats
    private string elementType;

    public float health, maxHP, baseDamage, modifiedDamage;
    private float damageTakenModifier = 1;
    private float damageDealtModifier = 1;
    private float damageReflectModifier = 0;
    public int cheatDeathCount = 0;
    private float cheatDeathPercentage = 0;
    private int stunDuration = 0;
    private int[] moves = new int[4];

    public float AllInExtraDamage = 0;
    public float AllInExtraDamageTaken = 0;

    #endregion
    #region UI
    private string playerName, spritePath;
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    public FloatingIndicator damageIndicator;

    public GameObject damageIndicatorSpawnPosition;

    #endregion

    void Update()
    {
        modifiedDamage = baseDamage * damageDealtModifier;
    }

    public void MonsterInit()
    {
        Monster playerMonster;
        if (isPlayer)
        {
            playerMonster = DataSaver.Instance.dts.equippedMonsters[0];
        }
        else
        {
            playerMonster = MonsterManager.Instance.GetMonsterFromID(monsterID);
        }
        playerName = playerMonster.name;
        health = playerMonster.baseHealth;
        maxHP = health;
        elementType = playerMonster.elementType;
        baseDamage = playerMonster.baseDamage;
        spritePath = playerMonster.spritePath;
        Sprite newSprite = Resources.Load<Sprite>(spritePath);
        RuntimeAnimatorController animController = Resources.Load<RuntimeAnimatorController>(spritePath);
        moves = playerMonster.moves;

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

    public void TakeDamage(float damage, IndicatorType type)
    {
        health = Mathf.Clamp(health - (damage * damageTakenModifier), 0, health);
        damageIndicator.ShowIndicator($"DMG {damage * damageTakenModifier}", type, damageIndicatorSpawnPosition.transform.position);

        if (health <= 0 && cheatDeathCount > 0)
        {
            StartCoroutine(WaitAndRevive());
        }
    }

    public IEnumerator WaitAndRevive()
    {
        yield return new WaitForSeconds(1);
        float reviveAmount = maxHP * (cheatDeathPercentage / 100);
        health = Mathf.Clamp(reviveAmount, 0, maxHP);
        cheatDeathCount--;

        damageIndicator.ShowIndicator($"REVIVE {reviveAmount}", IndicatorType.Heal, damageIndicatorSpawnPosition.transform.position);
    }



    public void DealDamage(MonsterScript opponent, float moveBaseDamage, string moveElementType)
    {
        IndicatorType type = IndicatorType.BaseDamage;
        float elementModifier = 1f;
        if ((moveElementType == "fire" && opponent.elementType == "grass") ||
            (moveElementType == "grass" && opponent.elementType == "rock") ||
            (moveElementType == "rock" && opponent.elementType == "water") ||
            (moveElementType == "water" && opponent.elementType == "fire"))
        {
            Debug.Log("super effective");
            elementModifier = 2f;
            type = IndicatorType.SuperEffectiveDamage;
        }
        else if ((moveElementType == "grass" && opponent.elementType == "fire") ||
            (moveElementType == "rock" && opponent.elementType == "grass") ||
            (moveElementType == "water" && opponent.elementType == "rock") ||
            (moveElementType == "fire" && opponent.elementType == "water"))
        {
            Debug.Log("not effective");
            elementModifier = 0.5f;
            type = IndicatorType.NotEffectiveDamage;
        }

        if (stunDuration > 0)
        {
            damageIndicator.ShowIndicator("STUNNED", IndicatorType.WeakenedDamage, damageIndicatorSpawnPosition.transform.position);
            return;
        }
        opponent.TakeDamage((baseDamage + moveBaseDamage) * elementModifier, type);

        if (opponent.DamageReflectModifier > 0)
        {
            float reflectDamage = baseDamage * opponent.DamageReflectModifier;
            TakeDamage(reflectDamage, IndicatorType.BaseDamage);
        }

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
        return health <= 0 && cheatDeathCount <= 0; ;
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

    public float GetModifiedDamage()
    {
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

    public float DamageReflectModifier
    {
        get { return damageReflectModifier; }
        set { damageReflectModifier = value; }
    }

    public int CheatDeathCount
    {
        get { return cheatDeathCount; }
        set { cheatDeathCount = value; }
    }

    public float CheatDeathPercentage
    {
        get { return cheatDeathPercentage; }
        set { cheatDeathPercentage = value; }
    }

    public int StunDuration
    {
        get { return stunDuration; }
        set { stunDuration = value; }

    }

    public void LearnMove(int movesetIndex, int moveIndex)
    {
        if (movesetIndex < 4 && movesetIndex >= 0)
        {
            moves[movesetIndex] = moveIndex;
        }
    }

    public int[] GetMovesID()
    {
        return moves;
    }

    public int GetMoveID(int movesetIndex)
    {
        return moves[movesetIndex];
    }

}
