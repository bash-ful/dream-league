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
    public QAManager qaManager;
    public LetterScript letterScript;
    public StringGenerator stringGenerator;
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

        qaManager.Init();
        ResetQuestionnaire();

        itemManager.Init();
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

        string answer = qaManager.Answer;
        string inputtedAnswer = qaManager.InputtedAnswerText.text;
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
        stringGenerator.ApplyGeneratedStringToButtons(qaManager.Answer);
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

    private void GiveRewards()
    {
        GameObject.Find("DataSaverRealtime").GetComponent<DataSaver>().dts.dreamCoinAmount += 50;
        GameObject.Find("DataSaverRealtime").GetComponent<DataSaver>().SaveDataFn();
    }

    public void UseItem(int itemID)
    {
        Item item = itemManager.GetItemFromID(itemID);
        print($"using item {item.name}");
        foreach (var effect in item.effects)
        {
            Enum.TryParse(effect.type, out EffectType eff);
            switch (eff)
            {
                case EffectType.HealPercentageOfMaxHP:
                    player.Heal(effect.value);
                    break;
                case EffectType.DamagePercentageOfMaxHP:
                    enemy.TakeDamage(enemy.GetMaxHP() * effect.value / 100);
                    break;
                case EffectType.ModifySelfDamageTakenModifier:
                    player.DamageTakenModifier *= effect.value;
                    activeEffects.Add(new ActiveEffect { type = eff, value = effect.value, remainingDuration = effect.effectDuration, keepStacking = effect.keepStacking });
                    break;
                case EffectType.ModifySelfDamageDealtModifier:
                    player.DamageDealtModifier *= effect.value;
                    activeEffects.Add(new ActiveEffect { type = eff, value = effect.value, remainingDuration = effect.effectDuration, keepStacking = effect.keepStacking });
                    break;
                case EffectType.ModifyEnemyDamageTakenModifier:
                    enemy.DamageTakenModifier *= effect.value;
                    activeEffects.Add(new ActiveEffect { type = eff, value = effect.value, remainingDuration = effect.effectDuration, keepStacking = effect.keepStacking });
                    break;
                case EffectType.ModifyEnemyDamageDealtModifier:
                    enemy.DamageDealtModifier *= effect.value;
                    activeEffects.Add(new ActiveEffect { type = eff, value = effect.value, remainingDuration = effect.effectDuration, keepStacking = effect.keepStacking });
                    break;
                case EffectType.ReflectDamage:
                    // This actually adds to the damage reflect modifier
                    player.DamageReflectModifier = effect.value;
                    activeEffects.Add(new ActiveEffect { type = eff, value = effect.value, remainingDuration = effect.effectDuration, keepStacking = effect.keepStacking });
                    break;
                case EffectType.CheatDeath:
                    player.CheatDeathCount = effect.effectDuration; // Set the number of times it can occur
                    player.CheatDeathPercentage = effect.value; // Set the percentage for revival
                    activeEffects.Add(new ActiveEffect { type = eff, value = effect.value, remainingDuration = effect.effectDuration, keepStacking = effect.keepStacking });
                    break;
            }
        }
    }

    private void ApplyActiveEffects()
    {
        float selfTotalDamageDealtModifier = 1;
        float selfTotalDamageTakenModifier = 1;
        float selfReflectDamageModifier = 0;
        float enemyTotalDamageDealtModifier = 1;
        float enemyTotalDamageTakenModifier = 1;
        float enemyReflectDamageModifier = 0;

        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            var effect = activeEffects[i];
            effect.remainingDuration--;

            switch (effect.type)
            {
                case EffectType.ModifySelfDamageTakenModifier:
                    selfTotalDamageTakenModifier *= effect.value;
                    break;
                case EffectType.ModifySelfDamageDealtModifier:
                    selfTotalDamageDealtModifier *= effect.value;
                    break;
                case EffectType.ModifyEnemyDamageTakenModifier:
                    enemyTotalDamageTakenModifier *= effect.value;
                    break;
                case EffectType.ModifyEnemyDamageDealtModifier:
                    if (effect.keepStacking)
                    {
                        enemyTotalDamageDealtModifier *= (float)Math.Pow(effect.value, effect.remainingDuration * -1);

                    }
                    else
                    {
                        enemyTotalDamageDealtModifier *= effect.value;
                    }
                    break;
                case EffectType.ReflectDamage:
                    selfReflectDamageModifier += effect.value;
                    break;
                case EffectType.CheatDeath:
                    effect.remainingDuration = player.CheatDeathCount;
                    player.CheatDeathPercentage = effect.value;
                    break;
            }

            if (effect.remainingDuration == 0)
            {
                activeEffects.RemoveAt(i);
            }
        }

        player.DamageTakenModifier = selfTotalDamageTakenModifier;
        player.DamageDealtModifier = selfTotalDamageDealtModifier;
        enemy.DamageTakenModifier = enemyTotalDamageTakenModifier;
        enemy.DamageDealtModifier = enemyTotalDamageDealtModifier;
        player.DamageReflectModifier = selfReflectDamageModifier;
    }
    public void ResetQuestionnaire()
    {
        letterScript.EnableAllButtons();
        qaManager.ChangeQA();
        qaManager.ResetAnswerText();
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
