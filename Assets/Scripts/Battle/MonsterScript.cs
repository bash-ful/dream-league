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

    private float health, damage, maxHP;
    private string playerName, spritePath;
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    public DamageIndicator damageIndicator;

    public GameObject something;

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
        } else {
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
        if ((health - damage) < 0)
        {
            health = 0;
            return;
        }

        health -= damage;
    }

    public void DealDamage(MonsterScript opponent)
    {
        opponent.TakeDamage(damage);
        damageIndicator.ShowDamageIndicator(damage, something.transform.position);
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
}
