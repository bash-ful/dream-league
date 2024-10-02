using TMPro;
using System;
using UnityEngine;
using System.Linq;
[System.Serializable]
public class StageRewards
{
    public int itemID;
    public int specialCoins;
    public int dreamCoins;
}
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
    public StageRewards rewards;
}
public class QAManager : MonoBehaviour
{
    private int nextIndex = 0;
    private string hiddenAnswer;
    private string answer, question;
    private int qaIndex, qaCount;
    private QAList qaList;
    public TMP_Text QuestionText, AnswerText;
    private string lastQuestion;
    public TextAsset qaJson;
    public int ItemID
    {
        get { return qaList.rewards.itemID; }
    }
    public int DreamCoins
    {
        get { return qaList.rewards.dreamCoins; }
    }
    public int SpecialCurrency
    {
        get { return qaList.rewards.specialCoins; }
    }

    public void Init()
    {
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
        QA qa;
        System.Random random = new();
        do
        {
            qa = qaList.qaList[random.Next(qaList.qaList.Count())];
        } while (qa.question == lastQuestion);


        answer = qa.answer;
        question = qa.question;

        lastQuestion = qa.question;

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

    public void ClearQAText()
    {
        AnswerText.text = "";
        QuestionText.text = "";
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
