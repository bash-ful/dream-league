using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MasterScript : MonoBehaviour
{
    private const float BASE_ANSWER_TIME = 60;
    private bool isSomeCoroutineRunning = false;
    private bool isPaused = false;
    public DialogueScript dialogueScript;
    public QAManager qaManager;
    public LetterScript letterScript;
    public StringGenerator stringGenerator;
    public MonsterScript player, enemy;
    public SceneScript sceneScript;
    public TimerUI timerUI;

    private float answerTime;
    public List<ActiveEffect> activeEffects = new();

    public GameObject playerBuffsPanel, enemyBuffsPanel;
    public List<Image> playerBuffImages, enemyBuffImages;
    private float totalElapsedTime;

    public GameObject onCorrectSound, onIncorrectSound, onWinSound, onLoseSound, bgm;

    #region MOVETHIS
    public GameObject winPanel, losePanel;
    public TMP_Text timeElapsedText, dreamCoinText, specialCurrencyText;
    public Image itemImage;
    private bool isGameOver = false;

    #endregion

    void Start()
    {
        playerBuffImages = new(playerBuffsPanel.GetComponentsInChildren<Image>());
        enemyBuffImages = new(enemyBuffsPanel.GetComponentsInChildren<Image>());
        GameObject.Find("GameWin").GetComponent<Image>().enabled = false;
        GameObject.Find("GameLose").GetComponent<Image>().enabled = false;
        ResetAnswerTime(BASE_ANSWER_TIME);
        player.MonsterInit();
        enemy.MonsterInit();

        qaManager.Init();
        ResetQuestionnaire();
        UpdateBuffIcons();
        dialogueScript.BeginDialogue();
        StartCoroutine(TimerCoroutine());
    }

    IEnumerator TimerCoroutine()
    {
        while (true)
        {

            if (isGameOver)
            {
                yield break; // Exit the coroutine if game over
            }

            if (player.IsDead() || enemy.IsDead())
            {
                isGameOver = true;
                if (player.IsDead())
                {
                    StartCoroutine(OnPlayerLose());
                }
                else if (enemy.IsDead())
                {
                    StartCoroutine(OnPlayerWin());
                }
                yield break; // Exit the coroutine if either player or enemy is dead
            }

            if (isSomeCoroutineRunning || isPaused || dialogueScript.dialogueActive)
            {
                yield return null;
                continue;
            }

            answerTime -= Time.deltaTime;
            totalElapsedTime += Time.deltaTime;
            timerUI.SetDisplayedTimeText(answerTime);

            if (answerTime <= 0.0f)
            {
                yield return StartCoroutine(OnTimerEnd());
                continue;
            }

            string answer = qaManager.Answer;
            string inputtedAnswer = qaManager.InputtedAnswerText.text;
            if (inputtedAnswer.Contains("_"))
            {
                yield return null;
                continue;
            }

            if (answer.Equals(inputtedAnswer))
            {
                print("correct answer!");
                yield return StartCoroutine(OnCorrectAnswer());
            }
            else
            {
                yield return StartCoroutine(OnTimerEnd());
            }

            yield return null;
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
        DataSaver.Instance.AddDreamCoins(qaManager.DreamCoins);
        DataSaver.Instance.AddSpecialCurrency(qaManager.SpecialCurrency);
        DataSaver.Instance.AddItemToInventory(qaManager.ItemID);
    }

    public void UseItem(int itemID)
    {
        Item item = ItemManager.Instance.GetItemFromID(itemID);
        print($"using item {item.name}");
        foreach (var effect in item.effects)
        {
            Enum.TryParse(effect.type, out EffectType eff);
            switch (eff)
            {
                case EffectType.HealPlayerPercentageOfMaxHP:
                    player.Heal(player.GetMaxHP() * effect.value / 100);
                    break;
                case EffectType.HealEnemyPercentageOfMaxHP:
                    enemy.Heal(enemy.GetMaxHP() * effect.value / 100);
                    break;
                case EffectType.DamagePlayerPercentageOfMaxHP:
                    player.TakeDamage(player.GetMaxHP() * effect.value / 100);
                    break;
                case EffectType.DamageEnemyPercentageOfMaxHP:
                    enemy.TakeDamage(enemy.GetMaxHP() * effect.value / 100);
                    break;
                case EffectType.ModifySelfDamageTakenModifier:
                    player.DamageTakenModifier = Mathf.Clamp(player.DamageTakenModifier + effect.value, 0, 999);
                    activeEffects.Add(new ActiveEffect { type = eff, value = effect.value, remainingDuration = effect.effectDuration, keepStacking = effect.keepStacking });
                    break;
                case EffectType.ModifySelfDamageDealtModifier:
                    player.DamageDealtModifier = Mathf.Clamp(player.DamageDealtModifier + effect.value, 0, 999);
                    activeEffects.Add(new ActiveEffect { type = eff, value = effect.value, remainingDuration = effect.effectDuration, keepStacking = effect.keepStacking });
                    break;
                case EffectType.ModifyEnemyDamageTakenModifier:
                    enemy.DamageTakenModifier = Mathf.Clamp(enemy.DamageTakenModifier + effect.value, 0, 999);
                    activeEffects.Add(new ActiveEffect { type = eff, value = effect.value, remainingDuration = effect.effectDuration, keepStacking = effect.keepStacking });
                    break;
                case EffectType.ModifyEnemyDamageDealtModifier:
                    enemy.DamageDealtModifier = Mathf.Clamp(enemy.DamageDealtModifier + effect.value, 0, 999);
                    activeEffects.Add(new ActiveEffect { type = eff, value = effect.value, remainingDuration = effect.effectDuration, keepStacking = effect.keepStacking });
                    break;
                case EffectType.ReflectDamage:
                    // This actually adds to the damage reflect modifier
                    player.DamageReflectModifier += effect.value;
                    activeEffects.Add(new ActiveEffect { type = eff, value = effect.value, remainingDuration = effect.effectDuration, keepStacking = effect.keepStacking });
                    break;
                case EffectType.CheatDeath:
                    player.CheatDeathCount = effect.effectDuration; // Set the number of times it can occur
                    player.CheatDeathPercentage = effect.value; // Set the percentage for revival
                    activeEffects.Add(new ActiveEffect { type = eff, value = effect.value, remainingDuration = effect.effectDuration, keepStacking = effect.keepStacking });
                    break;
                case EffectType.Stun:
                    enemy.StunDuration = effect.effectDuration;
                    activeEffects.Add(new ActiveEffect { type = eff, value = effect.value, remainingDuration = effect.effectDuration, keepStacking = effect.keepStacking });
                    break;
                case EffectType.PlayerVampirism:
                    enemy.TakeDamage(enemy.GetMaxHP() * effect.value / 100);
                    player.Heal(enemy.GetMaxHP() * effect.value / 100);
                    break;
                case EffectType.ModifyTimerBySeconds:
                    answerTime += effect.value;
                    break;
                case EffectType.AllIn:
                    player.AllInExtraDamage = enemy.GetMaxHP() * 0.1f;
                    player.AllInExtraDamageTaken = enemy.GetMaxHP() * 0.1f;
                    break;
            }
        }

        if (enemy.IsDead())
        {
            isGameOver = true;
            StartCoroutine(OnPlayerWin());
        }
        else if (player.IsDead())
        {
            isGameOver = true;
            StartCoroutine(OnPlayerLose());
        }
        ResetBuffIcons();
        LoadBuffIcons();
    }

    private void ApplyActiveEffects()
    {
        float selfTotalDamageDealtModifier = 1;
        float selfTotalDamageTakenModifier = 1;
        float selfReflectDamageModifier = 0;
        float enemyTotalDamageDealtModifier = 1;
        float enemyTotalDamageTakenModifier = 1;
        float enemyReflectDamageModifier = 0;
        float enemyStunDuration = 0;
        List<EffectType> playerBuffList = new();
        List<EffectType> enemyBuffList = new();

        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            var effect = activeEffects[i];
            effect.remainingDuration--;

            switch (effect.type)
            {
                case EffectType.ModifySelfDamageTakenModifier:
                    selfTotalDamageTakenModifier += effect.value;
                    break;
                case EffectType.ModifySelfDamageDealtModifier:
                    selfTotalDamageDealtModifier += effect.value;
                    break;
                case EffectType.ModifyEnemyDamageTakenModifier:
                    enemyTotalDamageTakenModifier += effect.value;
                    break;
                case EffectType.ModifyEnemyDamageDealtModifier:
                    enemyTotalDamageDealtModifier += effect.value;
                    break;
                case EffectType.ReflectDamage:
                    selfReflectDamageModifier += effect.value;
                    break;
                case EffectType.CheatDeath:
                    effect.remainingDuration = player.CheatDeathCount;
                    player.CheatDeathPercentage = effect.value;
                    break;
                case EffectType.Stun:
                    enemy.StunDuration = effect.remainingDuration;
                    break;

            }

            if (effect.remainingDuration == 0)
            {
                activeEffects.RemoveAt(i);
            }
        }

        player.DamageTakenModifier = Mathf.Max(0, selfTotalDamageTakenModifier);
        player.DamageDealtModifier = Mathf.Max(0, selfTotalDamageDealtModifier);
        enemy.DamageTakenModifier = Mathf.Max(0, enemyTotalDamageTakenModifier);
        enemy.DamageDealtModifier = Mathf.Max(0, enemyTotalDamageDealtModifier);
        player.DamageReflectModifier = Mathf.Max(0, selfReflectDamageModifier);
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

    private IEnumerator OnPlayerWin()
    {
        HideUI();
        bgm.GetComponent<AudioSource>().Stop();
        onWinSound.GetComponent<AudioSource>().Play();
        GameObject.Find("GameWin").GetComponent<Image>().enabled = true;
        dreamCoinText.text = qaManager.DreamCoins.ToString();
        int seconds = (int)totalElapsedTime % 60;
        int minutes = (int)totalElapsedTime / 60;
        timeElapsedText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        specialCurrencyText.text = qaManager.SpecialCurrency.ToString();
        itemImage.sprite = ItemManager.Instance.GetItemSprite(ItemManager.Instance.GetItemFromID(qaManager.ItemID));
        GiveRewards();
        yield return new WaitForSeconds(1);
        winPanel.SetActive(true);
    }

    private IEnumerator OnPlayerLose()
    {
        bgm.GetComponent<AudioSource>().Stop();
        onLoseSound.GetComponent<AudioSource>().Play();
        GameObject.Find("GameLose").GetComponent<Image>().enabled = true;
        yield return new WaitForSeconds(1);
        losePanel.SetActive(true);
    }

    private IEnumerator OnCorrectAnswer()
    {
        isSomeCoroutineRunning = true;
        onCorrectSound.GetComponent<AudioSource>().Play();
        player.DealDamage(enemy);
        if (enemy.IsDead())
        {
            StartCoroutine(OnPlayerWin());
            yield break;
        }
        else if (player.IsDead())
        {
            StartCoroutine(OnPlayerLose());
            yield break;
        }
        ResetQuestionnaire();
        ResetAnswerTime(BASE_ANSWER_TIME);
        ApplyActiveEffects();
        if (enemy.IsDead())
        {
            StartCoroutine(OnPlayerWin());
            yield break;
        }
        else if (player.IsDead())
        {
            StartCoroutine(OnPlayerLose());
            yield break;
        }
        ResetBuffIcons();
        LoadBuffIcons();
        isSomeCoroutineRunning = false;
    }
    private IEnumerator OnTimerEnd()
    {
        isPaused = true; // Pause the timer and questionnaire generation
        onIncorrectSound.GetComponent<AudioSource>().Play();

        isSomeCoroutineRunning = true;
        OnTurnEnd();
        yield return new WaitForSeconds(1);
        enemy.DealDamage(player);
        if (player.IsDead())
        {
            OnPlayerLose();
            yield break;
        }
        else if (enemy.IsDead())
        {
            OnPlayerWin();
            yield break;
        }
        yield return new WaitForSeconds(1);
        ResetQuestionnaire();
        ResetAnswerTime(BASE_ANSWER_TIME);
        ApplyActiveEffects();
        if (enemy.IsDead())
        {
            StartCoroutine(OnPlayerWin());
            yield break;
        }
        else if (player.IsDead())
        {
            StartCoroutine(OnPlayerLose());
            yield break;
        }
        ResetBuffIcons();
        LoadBuffIcons();
        isSomeCoroutineRunning = false;
        isPaused = false; // Resume the timer and questionnaire generation
    }

    private void OnTurnEnd()
    {
        letterScript.DisableAllButtons();
        stringGenerator.ClearAllButtonLetters();
        qaManager.ClearQAText();
    }

    private void LoadBuffIcons()
    {
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            var effect = activeEffects[i];

            switch (effect.type)
            {
                case EffectType.ModifySelfDamageTakenModifier:
                    if (effect.value > 1)
                    {
                        AddToPlayerBuffIcons(Resources.Load<Sprite>("UI Scenes/Shop/Card/Item Effects Icon/Stronger Damage"));
                    }
                    else
                    {
                        AddToPlayerBuffIcons(Resources.Load<Sprite>("UI Scenes/Shop/Card/Item Effects Icon/Weaker Damage"));
                    }
                    break;
                case EffectType.ModifySelfDamageDealtModifier:

                    break;
                case EffectType.ModifyEnemyDamageTakenModifier:

                    break;
                case EffectType.ModifyEnemyDamageDealtModifier:
                    if (effect.value > 1)
                    {
                        AddToEnemyBuffIcons(Resources.Load<Sprite>("UI Scenes/Shop/Card/Item Effects Icon/Stronger Damage"));
                    }
                    else
                    {
                        AddToEnemyBuffIcons(Resources.Load<Sprite>("UI Scenes/Shop/Card/Item Effects Icon/Weaker Damage"));
                    }
                    break;
                case EffectType.ReflectDamage:
                    AddToPlayerBuffIcons(Resources.Load<Sprite>("UI Scenes/Shop/Card/Item Effects Icon/Reflect Damage"));
                    break;
                case EffectType.CheatDeath:
                    AddToPlayerBuffIcons(Resources.Load<Sprite>("UI Scenes/Shop/Card/Item Effects Icon/Instant Health"));
                    break;
                case EffectType.Stun:
                    AddToEnemyBuffIcons(Resources.Load<Sprite>("UI Scenes/Shop/Card/Item Effects Icon/Stun"));
                    break;
            }
        }

    }

    private void AddToPlayerBuffIcons(Sprite sprite)
    {
        for (int i = 0; i < 9; i++)
        {
            if (playerBuffImages[i].sprite == null)
            {
                playerBuffImages[i].sprite = sprite;
                ImageTransparencyScript.UpdateImageTransparency(playerBuffImages[i]);
                return;
            }
        }
    }
    private void AddToEnemyBuffIcons(Sprite sprite)
    {
        for (int i = 0; i < 9; i++)
        {
            if (enemyBuffImages[i].sprite == null)
            {
                enemyBuffImages[i].sprite = sprite;
                ImageTransparencyScript.UpdateImageTransparency(enemyBuffImages[i]);
                return;
            }
        }
    }


    private void UpdateBuffIcons()
    {
        for (int i = 0; i < 9; i++)
        {
            ImageTransparencyScript.UpdateImageTransparency(playerBuffImages[i]);
            ImageTransparencyScript.UpdateImageTransparency(enemyBuffImages[i]);
        }
    }

    private void ResetBuffIcons()
    {
        for (int i = 0; i < 9; i++)
        {
            playerBuffImages[i].sprite = null;
            enemyBuffImages[i].sprite = null;
        }
        UpdateBuffIcons();
    }


}
