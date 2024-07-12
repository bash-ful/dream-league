using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MasterScript : MonoBehaviour
{
    private const float BASE_ANSWER_TIME = 60;
    private bool isSomeCoroutineRunning = false;
    public DialogueScript dialogueScript;
    public AnswerScript answerScript;
    public LetterScript letterScript;
    public StringGenerator stringGenerator;
    public TextAsset qaJson;
    private QAList qaList;
    public MonsterScript player, enemy;
    public SceneScript sceneScript;
    public TimerUI timerUI;

    private float answerTime;
    private MonsterList monsterList;
    private ItemList itemList;
    public TextAsset monstersJson;
    public TextAsset itemsJson;

    private float damageTakenReduction, damageDealtReduction;


    void Start()
    {
        GameObject.Find("GameWin").GetComponent<Image>().enabled = false;
        GameObject.Find("GameLose").GetComponent<Image>().enabled = false;
        ResetAnswerTime(BASE_ANSWER_TIME);

        dialogueScript.BeginDialogue();

        monsterList = JsonUtility.FromJson<MonsterList>(monstersJson.text);
        player.MonsterInit(monsterList);
        enemy.MonsterInit(monsterList);

        qaList = JsonUtility.FromJson<QAList>(qaJson.text);
        answerScript.ChangeQA(qaList);
        ResetButtonLetters();

        itemList = JsonUtility.FromJson<ItemList>(itemsJson.text);
        Debug.Log("Items loaded successfully");
        Debug.Log("Number of items: " + itemList.itemList.Count);
        foreach (var item in itemList.itemList)
        {
            Debug.Log("Item name: " + item.name);
            foreach (var effect in item.effects)
            {
                Debug.Log("Effect type: " + effect.type + ", value: " + effect.value);
            }
        }


    }

    private void ResetButtonLetters()
    {
        stringGenerator.ApplyGeneratedStringToButtons(answerScript.getAnswer());
    }

    void Update()
    {
        if (isSomeCoroutineRunning || dialogueScript.dialogueActive)
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
        if (inputtedAnswer.Contains("_"))
        {
            return;
        }
        if (answer.Equals(inputtedAnswer))
        {
            print("correct answer!");
            StartCoroutine(OnCorrectAnswer());
        }
        else
        {
            StartCoroutine(OnTimerEnd());
        }

    }

    private void HideUI()
    {
        GameObject.Find("Letters").SetActive(false);
        GameObject.Find("QA").SetActive(false);
        GameObject.Find("Timer").SetActive(false);
        GameObject.Find("Player").SetActive(false);
        GameObject.Find("Enemy").SetActive(false);
        GameObject.Find("Return").SetActive(false);
    }

    public IEnumerator OnCorrectAnswer()
    {
        isSomeCoroutineRunning = true;
        player.DealDamage(enemy);
        if (enemy.IsDead())
        {
            HideUI();
            GameObject.Find("GameWin").GetComponent<Image>().enabled = true;
            GiveRewards();
            yield return ReturnToMainMenu();
            yield break;
        }
        ResetQuestionnaire();
        ResetAnswerTime(BASE_ANSWER_TIME);
        isSomeCoroutineRunning = false;
    }

    private void GiveRewards()
    {
        GameObject.Find("DataSaverRealtime").GetComponent<DataSaver>().dts.dreamCoinAmount += 50;
        GameObject.Find("DataSaverRealtime").GetComponent<DataSaver>().SaveDataFn();
    }

    public void UseItem(int itemID)
    {
        Item item = itemList.itemList[itemID];
        foreach (var effect in item.effects)
        {
            print(effect.type);
            switch (effect.type)
            {
                case "HealPercentageOfMaxHP":
                    player.Heal(effect.value);
                    break;
                case "DamagePercentageOfMaxHP":
                    enemy.TakeDamage(enemy.maxHP * effect.value);
                    break;
                case "ModifyDamageTakenPercentage":
                    player.DamageTakenModifier = effect.value;
                    break;
                case "ModifyDamageDealtPercentage":
                    player.DamageDealtModifier = effect.value;
                    break;
            }
        }
    }

    public void ResetQuestionnaire()
    {
        answerScript.ResetAnswerText();
        letterScript.EnableAllButtons();
        answerScript.ChangeQA(qaList);
        ResetButtonLetters();
    }

    public IEnumerator ReturnToMainMenu()
    {
        yield return new WaitForSeconds(2);
        sceneScript.MoveScene(1);
    }

    public void ResetAnswerTime(float seconds)
    {
        answerTime = seconds;
    }

    private IEnumerator OnTimerEnd()
    {
        isSomeCoroutineRunning = true;
        enemy.DealDamage(player);
        if (player.IsDead())
        {
            GameObject.Find("GameLose").GetComponent<Image>().enabled = true;
            yield return ReturnToMainMenu();
            yield break;
        }
        ResetQuestionnaire();
        ResetAnswerTime(BASE_ANSWER_TIME);
        isSomeCoroutineRunning = false;
    }
}
