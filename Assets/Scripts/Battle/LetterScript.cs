using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LetterScript : MonoBehaviour
{
    private TMP_Text InputtedAnswerText;
    public QAManager qaManager;


    public void Start() {
        InputtedAnswerText = qaManager.AnswerText;
    }
    public void EnableAllButtons()
    {
        for (int i = 0; i < 18; i++)
        {
            GameObject.Find(i.ToString()).GetComponent<Button>().interactable = true;
        }
    }

    public void DisableAllButtons() {
        for (int i = 0; i < 18; i++)
        {
            GameObject.Find(i.ToString()).GetComponent<Button>().interactable = false;
        }
    }

    public void LetterPress(int ButtonIndex)
    {
        Button buttonPressed = GameObject.Find(ButtonIndex.ToString()).GetComponent<Button>();
        string buttonPressedLetter = buttonPressed.GetComponentInChildren<TMP_Text>().text;
        qaManager.AppendLetter(InputtedAnswerText, buttonPressedLetter);
        buttonPressed.interactable = false;

    }
}
