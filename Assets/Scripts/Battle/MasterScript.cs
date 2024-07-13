using System;
using System.Collections;
using System.Collections.Generic;
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
    public ItemManager itemManager;
    public List<ActiveEffect> activeEffects = new();


    void Start()
    {
        GameObject.Find("GameWin").GetComponent<Image>().enabled = false;
        GameObject.Find("GameLose").GetComponent<Image>().enabled = false;
        ResetAnswerTime(BASE_ANSWER_TIME);

        dialogueScript.BeginDialogue();

        player.MonsterInit();
        enemy.MonsterInit();

        qaList = JsonUtility.FromJson<QAList>(qaJson.text);
        answerScript.ChangeQA(qaList);
        ResetButtonLetters();
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

    private void ResetButtonLetters()
    {
        stringGenerator.ApplyGeneratedStringToButtons(answerScript.getAnswer());
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
        ApplyActiveEffects();
        isSomeCoroutineRunning = false;
    }

    private void GiveRewards()
    {
        GameObject.Find("DataSaverRealtime").GetComponent<DataSaver>().dts.dreamCoinAmount += 50;
        GameObject.Find("DataSaverRealtime").GetComponent<DataSaver>().SaveDataFn();
    }

    public void UseItem(int itemID)
    {
        Item item = itemManager.GetItemFromID(itemID);
        foreach (var effect in item.effects)
        {
            Enum.TryParse(effect.type, out EffectType eff);
            switch (eff)
            {
                case EffectType.HealPercentageOfMaxHP:
                    player.Heal(effect.value);
                    break;
                case EffectType.DamagePercentageOfMaxHP:
                    enemy.TakeDamage(enemy.GetMaxHP() * effect.value);
                    break;
                case EffectType.IncreaseSelfDamageTakenPercentage:
                    player.DamageTakenModifier *= effect.value;
                    activeEffects.Add(new ActiveEffect { type = eff, value = effect.value, remainingDuration = effect.effectDuration });
                    break;
                case EffectType.IncreaseSelfDamageDealtPercentage:
                    player.DamageDealtModifier *= effect.value;
                    activeEffects.Add(new ActiveEffect { type = eff, value = effect.value, remainingDuration = effect.effectDuration });
                    break;
            }
        }
    }

    private void ApplyActiveEffects()
    {
        float selfTotalDamageDealtModifier = 1;
        float selfTotalDamageTakenModifier = 1;
        float enemyTotalDamageDealtModifier = 1;
        float enemyTotalDamageTakenModifier = 1;

        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            var effect = activeEffects[i];
            effect.remainingDuration--;

            switch (effect.type)
            {
                case EffectType.IncreaseSelfDamageTakenPercentage:
                    selfTotalDamageTakenModifier *= effect.value;
                    break;
                case EffectType.IncreaseSelfDamageDealtPercentage:
                    selfTotalDamageDealtModifier *= effect.value;
                    break;
            }

            if (effect.remainingDuration <= 0)
            {
                activeEffects.RemoveAt(i);
            }
        }

        player.DamageTakenModifier = selfTotalDamageTakenModifier;
        player.DamageDealtModifier = selfTotalDamageDealtModifier;
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
        ApplyActiveEffects();
        isSomeCoroutineRunning = false;
    }
}
