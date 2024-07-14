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
public class QAManager : MonoBehaviour
{
    private int nextIndex = 0;
    private string hiddenAnswer;
    private string answer, question;
    private int qaIndex, qaCount;
    private QAList qaList;
    public TMP_Text QuestionText, AnswerText;

    public void Init()
    {
        TextAsset qaJson = Resources.Load<TextAsset>("Json/Stages/Stage1");
        if (qaJson != null)
        {
            qaList = JsonUtility.FromJson<QAList>(qaJson.text);
        }
        else
        {
            Debug.LogError("error loading qaJson");
        }
    }
    public void ChangeQA()
    {
        qaCount = qaList.qaList.Count();
        qaIndex = Random.Range(0, qaCount);
        answer = qaList.qaList[qaIndex].answer;
        question = qaList.qaList[qaIndex].question;

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
        AnswerText.text = hiddenAnswer;
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

    public TMP_Text InputtedAnswerText
    {
        get { return AnswerText; }
    }

    public string Answer
    {
        get { return answer; }
    }

    public string Question
    {
        get { return question; }
    }
}
