using UnityEngine;

public class MonsterScript : MonoBehaviour
{

    public float health, damage;
    public string playerName;
    
    public void TakeDamage(float damage)
    {
        if ((health - damage) >= 0)
        {
            health -= damage;
            if (health <= 0)
            {
                health = 0; // Ensure health doesn't go negative
            }
        }
    }

    public void DealDamage(MonsterScript opponent)
    {
        opponent.TakeDamage(damage);
    }

    public bool IsDead() {
        return health == 0;
    }

    public float GetHealth()
    {
        return health;
    }

    public string GetName() {
        return playerName;
    }

    public float GetDamage() {
        return damage;
    }
}
