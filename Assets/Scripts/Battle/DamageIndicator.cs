using UnityEngine;
using TMPro;
using System.Collections;

public class DamageIndicator : MonoBehaviour
{
    public GameObject damageTextPrefab; // Assign your TMP_Text prefab in the Inspector
    public Canvas canvas;

    public void ShowDamageIndicator(float damageAmount, Vector3 worldPosition)
    {
        GameObject instance = Instantiate(damageTextPrefab, canvas.transform); // Instantiate as child of Canvas
        instance.transform.position = worldPosition; // Set initial world position

        TMP_Text textComponent = instance.GetComponent<TMP_Text>();

        // Set the text to display the damage amount
        textComponent.text = damageAmount.ToString();

        // Start animation coroutine
        StartCoroutine(AnimateDamageIndicator(instance));
    }

    private IEnumerator AnimateDamageIndicator(GameObject instance)
    {
        TMP_Text textComponent = instance.GetComponent<TMP_Text>();

        // Optional: Fade in animation (you can customize this)
        float fadeInDuration = 0.3f;
        float fadeOutDuration = 0.2f;
        float moveUpSpeed = 600f;

        Color textColor = textComponent.color;
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
