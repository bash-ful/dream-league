using System.Collections;
using UnityEngine;

public class MonsterScript : MonoBehaviour
{
    public int monsterID;
    #region Monster Stats

    private float health, maxHP, baseDamage, modifiedDamage;
    private float damageTakenModifier = 1;
    private float damageDealtModifier = 1;
    private float damageReflectModifier = 0;
    public int cheatDeathCount = 0;
    private float cheatDeathPercentage = 0;
    private int stunDuration = 0;
    private readonly int[] moves = new int[4];

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
        Monster playerMonster = MonsterManager.Instance.GetMonsterFromID(monsterID);
        playerName = playerMonster.name;
        health = playerMonster.baseHealth;
        maxHP = health;
        baseDamage = playerMonster.baseDamage;
        spritePath = playerMonster.spritePath;
        Sprite newSprite = Resources.Load<Sprite>(spritePath);
        RuntimeAnimatorController animController = Resources.Load<RuntimeAnimatorController>(spritePath);
        moves[0] = 1;
        moves[1] = 2;
        moves[2] = 3;
        moves[3] = 0;

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
        damageIndicator.ShowIndicator($"DMG {damage * damageTakenModifier}", IndicatorType.BaseDamage, damageIndicatorSpawnPosition.transform.position);

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



    public void DealDamage(MonsterScript opponent, float moveBaseDamage)
    {
        if (stunDuration > 0)
        {
            damageIndicator.ShowIndicator("STUNNED", IndicatorType.WeakenedDamage, damageIndicatorSpawnPosition.transform.position);
            return;
        }
        opponent.TakeDamage(baseDamage + moveBaseDamage);

        if (opponent.DamageReflectModifier > 0)
        {
            float reflectDamage = baseDamage * opponent.DamageReflectModifier;
            TakeDamage(reflectDamage);
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
        return health == 0 && cheatDeathCount <= 0; ;
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

    public void LearnMove(int movesetIndex, int moveIndex){
        if(movesetIndex < 4 && movesetIndex >= 0) {
            moves[movesetIndex] = moveIndex;
        }
    }

    public int GetMoveID(int movesetIndex) {
        return moves[movesetIndex];
    }

}
