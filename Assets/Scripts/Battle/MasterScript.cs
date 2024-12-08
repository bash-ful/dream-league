using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MasterScript : MonoBehaviour
{

    public int stageLevel;
    private bool isPaused = false;
    private bool isPlayerTurn = true;
    private bool isSomeCoroutineRunning = false;
    public DialogueScript dialogueScript;
    public MonsterScript player, enemy;
    public SceneScript sceneScript;
    public List<ActiveEffect> playerActiveEffects = new();
    public List<ActiveEffect> enemyActiveEffects = new();


    public GameObject playerBuffsPanel, enemyBuffsPanel;
    public GameObject MovesPanel;
    public GameObject EnemyMovePanel;
    public TMP_Text EnemyMoveText;
    public List<Image> playerBuffImages, enemyBuffImages;
    private float totalElapsedTime;

    public GameObject onWinSound, onLoseSound, bgm;
    public AudioSource hit, dmg;

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
        if (DataSaver.Instance.dts.maxLevelCleared < stageLevel)
        {
            dialogueScript.BeginDialogue();
        }
        StartCoroutine(BattleCoroutine());

    }

    IEnumerator BattleCoroutine()
    {
        while (true)
        {
            if (isGameOver)
            {
                yield return null;
                continue;
            }

            if (isPlayerTurn)
            {
                MovesPanel.SetActive(true);
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
                MovesPanel.SetActive(false);
                StartCoroutine(EnemyMove());
            }


            if (player.IsDead() || enemy.IsDead())
            {
                isGameOver = true;
                if (player.IsDead())
                {
                    StartCoroutine(OnPlayerLose());
                }
                else
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
        ApplyPlayerEffects(id, true);
    }

    public void UpdateActiveEffects(Effect effect, EffectType effectType, string source, List<ActiveEffect> activeEffects)
    {
        int existingEffectIndex = activeEffects.FindIndex(eff => eff.source == source);
        if (existingEffectIndex >= 0)
        {
            activeEffects[existingEffectIndex].remainingDuration = effect.duration;
        }
        else
        {
            activeEffects.Add(new ActiveEffect { type = effectType, value = effect.value, remainingDuration = effect.duration, source = source });
        }
    }

    public void ApplyPlayerEffects(int id, bool isItem)
    {
        List<Effect> effects;
        string source;
        if (isItem)
        {
            Item item = ItemManager.Instance.GetItemFromID(id);
            effects = item.effects;
            source = item.name;
        }
        else
        {
            Move move = MoveManager.Instance.GetMoveFromID(id);
            effects = move.effects;
            source = move.name;
        }
        foreach (var effect in effects)
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
                    player.TakeDamage(player.GetMaxHP() * effect.value / 100, IndicatorType.BaseDamage);
                    break;
                case EffectType.DamageEnemyPercentageOfMaxHP:
                    enemy.TakeDamage(enemy.GetMaxHP() * effect.value / 100, IndicatorType.BaseDamage);
                    break;
                case EffectType.ModifySelfDamageTakenModifier:
                    player.DamageTakenModifier = Mathf.Clamp(player.DamageTakenModifier + effect.value, 0, 999);
                    UpdateActiveEffects(effect, eff, source, playerActiveEffects);
                    break;
                case EffectType.ModifySelfDamageDealtModifier:
                    player.DamageDealtModifier = Mathf.Clamp(player.DamageDealtModifier + effect.value, 0, 999);
                    UpdateActiveEffects(effect, eff, source, enemyActiveEffects);
                    break;
                case EffectType.ModifyEnemyDamageTakenModifier:
                    enemy.DamageTakenModifier = Mathf.Clamp(enemy.DamageTakenModifier + effect.value, 0, 999);
                    UpdateActiveEffects(effect, eff, source, enemyActiveEffects);
                    break;
                case EffectType.ModifyEnemyDamageDealtModifier:
                    enemy.DamageDealtModifier = Mathf.Clamp(enemy.DamageDealtModifier + effect.value, 0, 999);
                    UpdateActiveEffects(effect, eff, source, enemyActiveEffects);
                    break;
                case EffectType.ReflectDamage:
                    // This actually adds to the damage reflect modifier
                    player.DamageReflectModifier += effect.value;
                    UpdateActiveEffects(effect, eff, source, playerActiveEffects);
                    break;
                case EffectType.CheatDeath:
                    player.CheatDeathCount = effect.duration; // Set the number of times it can occur
                    player.CheatDeathPercentage = effect.value; // Set the percentage for revival
                    UpdateActiveEffects(effect, eff, source, playerActiveEffects);
                    break;
                case EffectType.Stun:
                    enemy.StunDuration = effect.duration;
                    UpdateActiveEffects(effect, eff, source, enemyActiveEffects);
                    break;
                case EffectType.PlayerVampirism:
                    enemy.TakeDamage(enemy.GetMaxHP() * effect.value / 100, IndicatorType.BaseDamage);
                    player.Heal(enemy.GetMaxHP() * effect.value / 100);
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

    public void ApplyEnemyEffects(int id, bool isItem)
    {
        List<Effect> effects;
        string source;
        if (isItem)
        {
            Item item = ItemManager.Instance.GetItemFromID(id);
            effects = item.effects;
            source = item.name;
        }
        else
        {
            Move move = MoveManager.Instance.GetMoveFromID(id);
            effects = move.effects;
            source = move.name;
        }
        foreach (var effect in effects)
        {
            Enum.TryParse(effect.type, out EffectType eff);
            switch (eff)
            {
                case EffectType.HealPlayerPercentageOfMaxHP:
                    enemy.Heal(enemy.GetMaxHP() * effect.value / 100);
                    break;
                case EffectType.HealEnemyPercentageOfMaxHP:
                    player.Heal(player.GetMaxHP() * effect.value / 100);
                    break;
                case EffectType.DamagePlayerPercentageOfMaxHP:
                    enemy.TakeDamage(player.GetMaxHP() * effect.value / 100, IndicatorType.BaseDamage);
                    break;
                case EffectType.DamageEnemyPercentageOfMaxHP:
                    player.TakeDamage(enemy.GetMaxHP() * effect.value / 100, IndicatorType.BaseDamage);
                    break;
                case EffectType.ModifySelfDamageTakenModifier:
                    enemy.DamageTakenModifier = Mathf.Clamp(enemy.DamageTakenModifier + effect.value, 0, 999);
                    UpdateActiveEffects(effect, eff, source, enemyActiveEffects);
                    break;
                case EffectType.ModifySelfDamageDealtModifier:
                    enemy.DamageDealtModifier = Mathf.Clamp(enemy.DamageDealtModifier + effect.value, 0, 999);
                    UpdateActiveEffects(effect, eff, source, playerActiveEffects);
                    break;
                case EffectType.ModifyEnemyDamageTakenModifier:
                    player.DamageTakenModifier = Mathf.Clamp(player.DamageTakenModifier + effect.value, 0, 999);
                    UpdateActiveEffects(effect, eff, source, playerActiveEffects);
                    break;
                case EffectType.ModifyEnemyDamageDealtModifier:
                    player.DamageDealtModifier = Mathf.Clamp(player.DamageDealtModifier + effect.value, 0, 999);
                    UpdateActiveEffects(effect, eff, source, playerActiveEffects);
                    break;
                case EffectType.ReflectDamage:
                    // This actually adds to the damage reflect modifier
                    enemy.DamageReflectModifier += effect.value;
                    UpdateActiveEffects(effect, eff, source, enemyActiveEffects);
                    break;
                case EffectType.CheatDeath:
                    enemy.CheatDeathCount = effect.duration; // Set the number of times it can occur
                    enemy.CheatDeathPercentage = effect.value; // Set the percentage for revival
                    UpdateActiveEffects(effect, eff, source, enemyActiveEffects);
                    break;
                case EffectType.Stun:
                    player.StunDuration = effect.duration;
                    UpdateActiveEffects(effect, eff, source, playerActiveEffects);
                    break;
                case EffectType.PlayerVampirism:
                    player.TakeDamage(player.GetMaxHP() * effect.value / 100, IndicatorType.BaseDamage);
                    enemy.Heal(player.GetMaxHP() * effect.value / 100);
                    break;
                case EffectType.AllIn:
                    enemy.AllInExtraDamage = player.GetMaxHP() * 0.1f;
                    enemy.AllInExtraDamageTaken = player.GetMaxHP() * 0.1f;
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

    private void ApplyPlayerActiveEffects()
    {
        float selfTotalDamageDealtModifier = 1;
        float selfTotalDamageTakenModifier = 1;
        float selfReflectDamageModifier = 0;

        for (int i = playerActiveEffects.Count - 1; i >= 0; i--)
        {
            var effect = playerActiveEffects[i];
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
                    selfTotalDamageTakenModifier += effect.value;
                    break;
                case EffectType.ModifyEnemyDamageDealtModifier:
                    selfTotalDamageDealtModifier += effect.value;
                    break;
                case EffectType.ReflectDamage:
                    selfReflectDamageModifier += effect.value;
                    break;
                case EffectType.CheatDeath:
                    effect.remainingDuration = player.CheatDeathCount;
                    player.CheatDeathPercentage = effect.value;
                    break;
                case EffectType.Stun:
                    player.StunDuration = effect.remainingDuration;
                    break;

            }

            if (effect.remainingDuration == 0)
            {
                playerActiveEffects.RemoveAt(i);
            }
        }

        player.DamageTakenModifier = Mathf.Max(0, selfTotalDamageTakenModifier);
        player.DamageDealtModifier = Mathf.Max(0, selfTotalDamageDealtModifier);
        player.DamageReflectModifier = Mathf.Max(0, selfReflectDamageModifier);
    }

    private void ApplyEnemyActiveEffects()
    {
        float selfTotalDamageDealtModifier = 1;
        float selfTotalDamageTakenModifier = 1;
        float selfReflectDamageModifier = 0;

        for (int i = enemyActiveEffects.Count - 1; i >= 0; i--)
        {
            var effect = enemyActiveEffects[i];
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
                    selfTotalDamageTakenModifier += effect.value;
                    break;
                case EffectType.ModifyEnemyDamageDealtModifier:
                    selfTotalDamageDealtModifier += effect.value;
                    break;
                case EffectType.ReflectDamage:
                    selfReflectDamageModifier += effect.value;
                    break;
                case EffectType.CheatDeath:
                    effect.remainingDuration = enemy.CheatDeathCount;
                    enemy.CheatDeathPercentage = effect.value;
                    break;
                case EffectType.Stun:
                    enemy.StunDuration = effect.remainingDuration;
                    break;

            }

            if (effect.remainingDuration == 0)
            {
                enemyActiveEffects.RemoveAt(i);
            }
        }

        enemy.DamageTakenModifier = Mathf.Max(0, selfTotalDamageTakenModifier);
        enemy.DamageDealtModifier = Mathf.Max(0, selfTotalDamageDealtModifier);
        enemy.DamageReflectModifier = Mathf.Max(0, selfReflectDamageModifier);
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
        DataSaver.Instance.UpdateMaxLevelCleared(stageLevel);
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
        if (move.type == "attack")
        {
            player.DealDamage(enemy, move.baseDamage, move.elementType);
            hit.Play();

        }
        ApplyPlayerEffects(move.id, false);
        isPlayerTurn = false;
        isSomeCoroutineRunning = true;
    }

    private IEnumerator EnemyMove()
    {
        if (isSomeCoroutineRunning)
        {
            isSomeCoroutineRunning = false;
            yield return new WaitForSeconds(1);
            ApplyEnemyActiveEffects();
            int randomNumber = UnityEngine.Random.Range(0, 4);
            Move move = MoveManager.Instance.GetMoveFromID(enemy.GetMoveID(randomNumber));
            EnemyMovePanel.SetActive(true);
            EnemyMoveText.text = $"Enemy uses {move.name}!";
            yield return new WaitForSeconds(1);
            if (move.type == "attack")
            {
                enemy.DealDamage(player, move.baseDamage, move.elementType);
                hit.Play();
                dmg.Play();
            }
            ApplyEnemyEffects(move.id, false);
            yield return new WaitForSeconds(1);
            EnemyMovePanel.SetActive(false);
            isPlayerTurn = true;
            ApplyPlayerActiveEffects();
        }
    }


    private void LoadBuffIcons()
    {
        for (int i = playerActiveEffects.Count - 1; i >= 0; i--)
        {
            var effect = playerActiveEffects[i];

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
                        AddToPlayerBuffIcons(Resources.Load<Sprite>("UI Scenes/Shop/Card/Item Effects Icon/Stronger Damage"));
                    }
                    else
                    {
                        AddToPlayerBuffIcons(Resources.Load<Sprite>("UI Scenes/Shop/Card/Item Effects Icon/Weaker Damage"));
                    }
                    break;
                case EffectType.ReflectDamage:
                    AddToPlayerBuffIcons(Resources.Load<Sprite>("UI Scenes/Shop/Card/Item Effects Icon/Reflect Damage"));
                    break;
                case EffectType.CheatDeath:
                    AddToPlayerBuffIcons(Resources.Load<Sprite>("UI Scenes/Shop/Card/Item Effects Icon/Instant Health"));
                    break;
                case EffectType.Stun:
                    AddToPlayerBuffIcons(Resources.Load<Sprite>("UI Scenes/Shop/Card/Item Effects Icon/Stun"));
                    break;
            }
        }

        for (int i = enemyActiveEffects.Count - 1; i >= 0; i--)
        {
            var effect = enemyActiveEffects[i];

            switch (effect.type)
            {
                case EffectType.ModifySelfDamageTakenModifier:
                    if (effect.value > 1)
                    {
                        AddToEnemyBuffIcons(Resources.Load<Sprite>("UI Scenes/Shop/Card/Item Effects Icon/Stronger Damage"));
                    }
                    else
                    {
                        AddToEnemyBuffIcons(Resources.Load<Sprite>("UI Scenes/Shop/Card/Item Effects Icon/Weaker Damage"));
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
                    AddToEnemyBuffIcons(Resources.Load<Sprite>("UI Scenes/Shop/Card/Item Effects Icon/Reflect Damage"));
                    break;
                case EffectType.CheatDeath:
                    AddToEnemyBuffIcons(Resources.Load<Sprite>("UI Scenes/Shop/Card/Item Effects Icon/Instant Health"));
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
