using System.Collections;
using UnityEngine;

public class MonsterSprite : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    public float damageDuration = 0.33f;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    // Call this function to trigger the damage effect
    public void ChangeSpriteColor(IndicatorType type)
    {
        StartCoroutine(DamageEffectCoroutine(type));
    }

    public Color GetColor(IndicatorType type)
    {
        return type switch
        {
            IndicatorType.BaseDamage => Color.red,
            IndicatorType.NotEffectiveDamage => Color.red,
            IndicatorType.SuperEffectiveDamage => Color.red,
            IndicatorType.WeakenedDamage => Color.red,
            IndicatorType.Heal => Color.green,
            IndicatorType.Buff => Color.yellow,
            IndicatorType.Debuff => Color.magenta,
            _ => Color.white
        };
    }

    private IEnumerator DamageEffectCoroutine(IndicatorType type)
    {
        Color color = GetColor(type);
        spriteRenderer.color = color;

        yield return new WaitForSeconds(damageDuration);

        float timeElapsed = 0f;
        while (timeElapsed < damageDuration)
        {
            spriteRenderer.color = Color.Lerp(color, originalColor, timeElapsed / damageDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // bring back to original color
        spriteRenderer.color = originalColor;
    }
}
