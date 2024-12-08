public enum EffectType
{
    HealPlayerPercentageOfMaxHP,
    HealEnemyPercentageOfMaxHP,
    DamagePlayerPercentageOfMaxHP,
    DamageEnemyPercentageOfMaxHP,
    ModifySelfDamageTakenModifier,
    ModifySelfDamageDealtModifier,

    ModifyEnemyDamageTakenModifier,

    ModifyEnemyDamageDealtModifier,

    ReflectDamage,
    CheatDeath,
    ModifyTimerBySeconds,
    Stun,
    PlayerVampirism,
    AllIn
}


[System.Serializable]
public class Effect
{
    public string type;
    public float value;
    public int duration;
    
}

[System.Serializable]
public class ActiveEffect
{
    public EffectType type;
    public float value;
    public int remainingDuration;
    public string source;
}