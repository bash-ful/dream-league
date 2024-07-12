using UnityEngine;
using TMPro;
using System.Collections;


public enum IndicatorType
{
    BaseDamage,
    WeakenedDamage,
    BoostedDamage,
    Heal
}
public class FloatingIndicator : MonoBehaviour
{
    public GameObject indicatorTextPrefab; // Assign your TMP_Text prefab in the Inspector
    public Canvas canvas;

    public void ShowIndicator(string text, IndicatorType type, Vector3 worldPosition)
    {
        GameObject instance = Instantiate(indicatorTextPrefab, canvas.transform); // Instantiate as child of Canvas
        instance.transform.position = worldPosition; // Set initial world position

        TMP_Text textComponent = instance.GetComponent<TMP_Text>();

        // Set the text to display the damage amount
        textComponent.text = text;

        // Start animation coroutine
        StartCoroutine(AnimateIndicator(instance, type));
    }

    private Color GetColor(IndicatorType type)
    {
        switch (type)
        {
            case IndicatorType.BaseDamage:
                return Color.white;
            case IndicatorType.Heal:
                return Color.green;
            case IndicatorType.WeakenedDamage:
                return Color.blue;
            case IndicatorType.BoostedDamage:
                return Color.red;
            default:
                return Color.white;
        }
    }

    private IEnumerator AnimateIndicator(GameObject instance, IndicatorType type)
    {
        TMP_Text textComponent = instance.GetComponent<TMP_Text>();

        // Optional: Fade in animation (you can customize this)
        float fadeInDuration = 0.25f;
        float fadeOutDuration = 0.5f;
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
