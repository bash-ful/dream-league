using UnityEngine;
using TMPro;
using System.Collections;


public enum IndicatorType
{
    BaseDamage,
    WeakenedDamage,
    BoostedDamage,
    SuperEffectiveDamage,
    NotEffectiveDamage,
    Heal
}
public class FloatingIndicator : MonoBehaviour
{
    public GameObject indicatorTextPrefab;
    public Canvas canvas;

    public void ShowIndicator(string text, IndicatorType type, Vector3 worldPosition)
    {
        GameObject instance = Instantiate(indicatorTextPrefab, canvas.transform);
        instance.transform.position = worldPosition;

        TMP_Text textComponent = instance.GetComponent<TMP_Text>();

        textComponent.text = text;

        StartCoroutine(AnimateIndicator(instance, type));
    }

    private Color GetColor(IndicatorType type)
    {
        return type switch
        {
            IndicatorType.BaseDamage => Color.white,
            IndicatorType.Heal => Color.green,
            IndicatorType.WeakenedDamage => Color.blue,
            IndicatorType.BoostedDamage => Color.red,
            IndicatorType.SuperEffectiveDamage => Color.yellow,
            IndicatorType.NotEffectiveDamage => Color.gray,
            _ => Color.white,
        };
    }

    private IEnumerator AnimateIndicator(GameObject instance, IndicatorType type)
    {
        TMP_Text textComponent = instance.GetComponent<TMP_Text>();

        // Optional: Fade in animation (you can customize this)
        float fadeInDuration = 0.2f;
        float fadeOutDuration = 0.4f;
        float moveUpSpeed = 750f;

        Color textColor = GetColor(type);
        textColor.a = 0f;
        textComponent.color = textColor;

        float timer = 0f;
        while (timer < fadeInDuration)
        {
            timer += Time.deltaTime;
            textColor.a = Mathf.Lerp(0f, 1f, timer / fadeInDuration);
            textComponent.color = textColor;
            instance.transform.Translate(Vector3.up * moveUpSpeed * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(fadeOutDuration);

        // Fade out animation
        timer = 0f;
        while (timer < fadeOutDuration)
        {
            timer += Time.deltaTime;
            textColor.a = Mathf.Lerp(1f, 0f, timer / fadeOutDuration);
            textComponent.color = textColor;
            yield return null;
        }

        // Destroy the instance when animation is finished
        Destroy(instance);
    }
}
