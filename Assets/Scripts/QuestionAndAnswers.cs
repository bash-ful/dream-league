[System.Serializable]
public class QuestionAndAnswer
{
    public string answer;
    public string question;
}

[System.Serializable]
public class QuestionsAndAnswers
{
    public QuestionAndAnswer[] qaList;
}
