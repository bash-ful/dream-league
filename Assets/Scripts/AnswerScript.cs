using System.Linq;
using TMPro;
using UnityEngine;

[System.Serializable]
public class QA
{
    public string answer;
    public string question;
}

[System.Serializable]
public class QAList
{
    public QA[] qaList;
}
public class AnswerScript : MonoBehaviour
{
    private int nextIndex = 0;
    private string hiddenAnswer;
    private string answer, question;
    private int qaIndex, qaCount;

    public void ChangeQA(QAList questionsAndAnswers)
    {
        qaCount = questionsAndAnswers.qaList.Count();
        qaIndex = Random.Range(0, qaCount);
        answer = questionsAndAnswers.qaList[qaIndex].answer;
        question = questionsAndAnswers.qaList[qaIndex].question;

        ResetQuestionText();
        ResetAnswerText();
    }

    private void ResetQuestionText()
    {
        GameObject.Find("QuestionText").GetComponent<TMP_Text>().text = question;
    }

    public void ResetAnswerText()
    {
        nextIndex = 0;
        hiddenAnswer = new string('_', answer.Length);
        GetInputtedAnswerText().text = hiddenAnswer;
    }

    public void AppendLetter(TMP_Text AnswerText, string letter)
    {
        if (nextIndex < hiddenAnswer.Length)
        {
            int realIndex = nextIndex;
            hiddenAnswer = hiddenAnswer.Remove(realIndex, 1).Insert(realIndex, letter);
            nextIndex++;
            AnswerText.text = hiddenAnswer;
        }
    }

    public TMP_Text GetInputtedAnswerText()
    {
        return GameObject.Find("AnswerText").GetComponent<TMP_Text>();
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
