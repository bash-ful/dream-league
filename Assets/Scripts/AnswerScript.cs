using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class AnswerScript : MonoBehaviour
{
    private int nextIndex = 0;
    private string hiddenAnswer;
    private string answer, question;
    private int questionIndex;
    private int listMaxIndex;

    public void ChangeQA(QuestionsAndAnswers questionsAndAnswers)
    {
        listMaxIndex = questionsAndAnswers.qaList.Count() - 1;
        questionIndex = Random.Range(0, listMaxIndex);
        answer = questionsAndAnswers.qaList[questionIndex].answer;
        question = questionsAndAnswers.qaList[questionIndex].question;

        ResetQuestionText();
        ResetAnswerText();
    }

    private void ResetQuestionText()
    {
        GameObject.Find("QuestionText").GetComponent<Text>().text = question;
    }

    public void ResetAnswerText()
    {
        nextIndex = 0;
        // Create a string of underscores with spaces in between
        hiddenAnswer = string.Join(" ", new string('_', answer.Length).ToCharArray());
        GetInputtedAnswerText().text = hiddenAnswer;
    }

    // Method to append a letter to the text
    public void AppendLetter(Text AnswerText, string letter)
    {
        if (nextIndex < hiddenAnswer.Length)
        {
            // Find the index of the next underscore to replace, considering spaces
            int realIndex = nextIndex * 2;
            hiddenAnswer = hiddenAnswer.Remove(realIndex, 1).Insert(realIndex, letter);
            nextIndex++;
            AnswerText.text = hiddenAnswer;
        }
    }

    public Text GetInputtedAnswerText()
    {
        return GameObject.Find("AnswerText").GetComponent<Text>();
    }

    public string getAnswer()
    {
        return answer;
    }

    public string getQuestion()
    {
        return question;
    }
}
