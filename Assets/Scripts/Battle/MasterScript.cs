using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MasterScript : MonoBehaviour
{
    private const float BASE_ANSWER_TIME = 60;
    private bool isPaused = false;
    private bool isPlayerTurn = true;
    private bool isSomeCoroutineRunning = false;
    public DialogueScript dialogueScript;
    public MonsterScript player, enemy;
    public SceneScript sceneScript;
    public TimerUI timerUI;
    public List<ActiveEffect> activeEffects = new();

    public GameObject playerBuffsPanel, enemyBuffsPanel;
    public GameObject MovesPanel;
    public GameObject EnemyMovePanel;
    public TMP_Text EnemyMoveText;
    public List<Image> playerBuffImages, enemyBuffImages;
    private float totalElapsedTime;

    public GameObject onWinSound, onLoseSound, bgm;

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
        player.MonsterInit();
        enemy.MonsterInit();

        UpdateBuffIcons();
        // dialogueScript.BeginDialogue();
        StartCoroutine(BattleCoroutine());
    }

    IEnumerator BattleCoroutine()
    {
        while (true)
        {

            if (isGameOver)
            {
                yield break; // Exit the coroutine if game over
            }

            if (isPlayerTurn)
            {
                Button[] moves = MovesPanel.GetComponentsInChildren<Button>();
                foreach (Button move in moves)
                {
                    move.interactable = true;
                }
            }
            else
            {
                Button[] moves = MovesPanel.GetComponentsInChildren<Button>();
                foreach (Button move in moves)
                {
                    move.interactable = false;
                }
                StartCoroutine(EnemyMove());
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

            yield return null;
        }
    }

    private void HideUI()
    {
        GameObject.Find("Moves").SetActive(false);
        GameObject.Find("Player").SetActive(false);
        GameObject.Find("Enemy").SetActive(false);
        GameObject.Find("Return").SetActive(false);
    }

    private void GiveRewards()
    {
        DataSaver.Instance.AddDreamCoins(500);
        DataSaver.Instance.AddSpecialCurrency(500);
        DataSaver.Instance.AddItemToInventory(2);
    }

    public void UseItem(int id)
    {
        ApplyEffects(id, true, player, enemy);
    }
    public void ApplyEffects(int id, bool isItem, MonsterScript user, MonsterScript target)
    {
        List<Effect> effects;
        if (isItem)
        {
            effects = ItemManager.Instance.GetItemFromID(id).effects;
        }
        else
        {
            effects = MoveManager.Instance.GetMoveFromID(id).effects;
        }
        foreach (var effect in effects)
        {
            Enum.TryParse(effect.type, out EffectType eff);
            switch (eff)
            {
                case EffectType.HealPlayerPercentageOfMaxHP:
                    user.Heal(user.GetMaxHP() * effect.value / 100);
                    break;
                case EffectType.HealEnemyPercentageOfMaxHP:
                    target.Heal(target.GetMaxHP() * effect.value / 100);
                    break;
                case EffectType.DamagePlayerPercentageOfMaxHP:
                    user.TakeDamage(user.GetMaxHP() * effect.value / 100);
                    break;
                case EffectType.DamageEnemyPercentageOfMaxHP:
                    target.TakeDamage(target.GetMaxHP() * effect.value / 100);
                    break;
                case EffectType.ModifySelfDamageTakenModifier:
                    user.DamageTakenModifier = Mathf.Clamp(user.DamageTakenModifier + effect.value, 0, 999);
                    activeEffects.Add(new ActiveEffect { type = eff, value = effect.value, remainingDuration = effect.effectDuration, keepStacking = effect.keepStacking });
                    break;
                case EffectType.ModifySelfDamageDealtModifier:
                    user.DamageDealtModifier = Mathf.Clamp(user.DamageDealtModifier + effect.value, 0, 999);
                    activeEffects.Add(new ActiveEffect { type = eff, value = effect.value, remainingDuration = effect.effectDuration, keepStacking = effect.keepStacking });
                    break;
                case EffectType.ModifyEnemyDamageTakenModifier:
                    target.DamageTakenModifier = Mathf.Clamp(target.DamageTakenModifier + effect.value, 0, 999);
                    activeEffects.Add(new ActiveEffect { type = eff, value = effect.value, remainingDuration = effect.effectDuration, keepStacking = effect.keepStacking });
                    break;
                case EffectType.ModifyEnemyDamageDealtModifier:
                    target.DamageDealtModifier = Mathf.Clamp(target.DamageDealtModifier + effect.value, 0, 999);
                    activeEffects.Add(new ActiveEffect { type = eff, value = effect.value, remainingDuration = effect.effectDuration, keepStacking = effect.keepStacking });
                    break;
                case EffectType.ReflectDamage:
                    // This actually adds to the damage reflect modifier
                    user.DamageReflectModifier += effect.value;
                    activeEffects.Add(new ActiveEffect { type = eff, value = effect.value, remainingDuration = effect.effectDuration, keepStacking = effect.keepStacking });
                    break;
                case EffectType.CheatDeath:
                    user.CheatDeathCount = effect.effectDuration; // Set the number of times it can occur
                    user.CheatDeathPercentage = effect.value; // Set the percentage for revival
                    activeEffects.Add(new ActiveEffect { type = eff, value = effect.value, remainingDuration = effect.effectDuration, keepStacking = effect.keepStacking });
                    break;
                case EffectType.Stun:
                    target.StunDuration = effect.effectDuration;
                    activeEffects.Add(new ActiveEffect { type = eff, value = effect.value, remainingDuration = effect.effectDuration, keepStacking = effect.keepStacking });
                    break;
                case EffectType.PlayerVampirism:
                    target.TakeDamage(target.GetMaxHP() * effect.value / 100);
                    user.Heal(target.GetMaxHP() * effect.value / 100);
                    break;
                case EffectType.AllIn:
                    user.AllInExtraDamage = target.GetMaxHP() * 0.1f;
                    user.AllInExtraDamageTaken = target.GetMaxHP() * 0.1f;
                    break;
            }
        }

        if (target.IsDead())
        {
            isGameOver = true;
            StartCoroutine(OnPlayerWin());
        }
        else if (user.IsDead())
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

    public IEnumerator ReturnToMainMenu()
    {
        yield return new WaitForSeconds(2);
        sceneScript.MoveScene(1);
    }

    private IEnumerator OnPlayerWin()
    {
        HideUI();
        bgm.GetComponent<AudioSource>().Stop();
        onWinSound.GetComponent<AudioSource>().Play();
        GameObject.Find("GameWin").GetComponent<Image>().enabled = true;
        dreamCoinText.text = "500";
        specialCurrencyText.text = "500";
        int seconds = (int)totalElapsedTime % 60;
        int minutes = (int)totalElapsedTime / 60;
        timeElapsedText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        itemImage.sprite = ItemManager.Instance.GetItemSprite(ItemManager.Instance.GetItemFromID(2));
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

    public void PlayerMove(int movesetIndex)
    {
        Move move = MoveManager.Instance.GetMoveFromID(player.GetMoveID(movesetIndex));
        Debug.Log($"Player uses {move.name}!");
        if (move.type == "attack")
        {
            player.DealDamage(enemy, move.baseDamage);
        }
        ApplyEffects(move.id, false, player, enemy);
        isPlayerTurn = false;
        isSomeCoroutineRunning = true;
    }

    private IEnumerator EnemyMove()
    {
        if (isSomeCoroutineRunning)
        {
            isSomeCoroutineRunning = false;
            yield return new WaitForSeconds(1);
            int randomNumber = UnityEngine.Random.Range(0, 4);
            Move move = MoveManager.Instance.GetMoveFromID(enemy.GetMoveID(randomNumber));
            EnemyMovePanel.SetActive(true);
            EnemyMoveText.text = $"Enemy uses {move.name}!";
            yield return new WaitForSeconds(1);
            if (move.type == "attack")
            {
                enemy.DealDamage(player, move.baseDamage);
            }
            ApplyEffects(move.id, false, enemy, player);
            yield return new WaitForSeconds(1);
            EnemyMovePanel.SetActive(false);
            isPlayerTurn = true;
            ApplyActiveEffects();
        }
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
