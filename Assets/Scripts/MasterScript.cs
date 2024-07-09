using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class MasterScript : MonoBehaviour
{
    private const float BASE_ANSWER_TIME = 11.0f;
    private bool isSomeCoroutineRunning = false;

    public AnswerScript answerScript;
    public LetterScript letterScript;
    public StringGenerator stringGenerator;
    public TextAsset qaJson;
    private QuestionsAndAnswers questionsAndAnswers;
    public MonsterScript player, enemy;
    public SceneScript sceneScript;
    public TimerUI timerUI;

    private float answerTime;


    void Start()
    {
        GameObject.Find("GameWin").GetComponent<Image>().enabled = false;
        GameObject.Find("GameLose").GetComponent<Image>().enabled = false;
        ResetAnswerTime(BASE_ANSWER_TIME);
        questionsAndAnswers = JsonUtility.FromJson<QuestionsAndAnswers>(qaJson.text);
        answerScript.ChangeQA(questionsAndAnswers);
        ResetButtonLetters();
    }

    private void ResetButtonLetters()
    {
        stringGenerator.ApplyGeneratedStringToButtons(answerScript.getAnswer());
    }

    void Update()
    {
        if (isSomeCoroutineRunning)
        {
            return;
        }
        answerTime -= Time.deltaTime;
        timerUI.SetDisplayedTimeText(answerTime);

        if (answerTime <= 0.0f)
        {
            StartCoroutine(OnTimerEnd());
            return;
        }

        string answer = answerScript.getAnswer();
        string inputtedAnswer = answerScript.GetInputtedAnswerText().text;
        inputtedAnswer = Regex.Replace(inputtedAnswer, @"\s+", "");
        if (answer.Equals(inputtedAnswer))
        {
            StartCoroutine(OnCorrectAnswer());
        }

    }

    private void HideUI() {
        GameObject.Find("Letters").SetActive(false);
        GameObject.Find("QA").SetActive(false);
        GameObject.Find("Timer").SetActive(false);
    }

    public IEnumerator OnCorrectAnswer()
    {
        isSomeCoroutineRunning = true;
        yield return new WaitForSeconds(1);
        player.DealDamage(enemy);
        Debug.Log($"correct! {player.GetName()} deals {player.GetDamage()} damage!");
        if (enemy.IsDead())
        {
            Debug.Log("you are winrar!");
            HideUI();
            GameObject.Find("GameWin").GetComponent<Image>().enabled = true;
            yield return ReturnToMainMenu();
            yield break;
        }
        ResetQuestionnaire();
        ResetAnswerTime(BASE_ANSWER_TIME);
        isSomeCoroutineRunning = false;
    }

    public void ResetQuestionnaire()
    {
        answerScript.ResetAnswerText();
        letterScript.EnableAllButtons();
        answerScript.ChangeQA(questionsAndAnswers);
        ResetButtonLetters();
    }

    public IEnumerator ReturnToMainMenu()
    {
        yield return new WaitForSeconds(2);
        sceneScript.MoveScene(0);
    }

    public void ResetAnswerTime(float seconds)
    {
        answerTime = seconds;
    }

    private IEnumerator OnTimerEnd()
    {
        isSomeCoroutineRunning = true;
        Debug.Log($"ran out of time! Enemy deals {enemy.GetDamage()} damage!");
        enemy.DealDamage(player);
        if (player.IsDead())
        {
            Debug.Log("You lose!!1");
            GameObject.Find("GameLose").GetComponent<Image>().enabled = true;
            yield return ReturnToMainMenu();
            yield break;
        }
        ResetQuestionnaire();
        ResetAnswerTime(BASE_ANSWER_TIME);
        isSomeCoroutineRunning = false;
    }
}
