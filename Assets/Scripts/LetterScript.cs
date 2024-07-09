using UnityEngine;
using UnityEngine.UI;

public class LetterScript : MonoBehaviour
{
    private Text InputtedAnswerText;
    public AnswerScript answerScript;


    public void Start() {
        InputtedAnswerText = answerScript.GetInputtedAnswerText();
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
        string buttonPressedLetter = buttonPressed.GetComponentInChildren<Text>().text;
        answerScript.AppendLetter(InputtedAnswerText, buttonPressedLetter);
        buttonPressed.interactable = false;

    }
}
