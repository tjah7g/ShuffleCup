using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Managers")]
    public UIManager uiManager;
    public CupManager cupManager;
    public PlayerData playerData;

    [Header("Game Settings")]
    public int correctGuessesForNewCup = 5;
    public int maxCups = 7;

    private int correctStreakCount = 0;
    private bool isPlaying = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        InitializeGame();
    }

    public void InitializeGame()
    {
        playerData.ResetData();
        correctStreakCount = 0;
        isPlaying = false;

        uiManager.UpdateAllUI();
        uiManager.HideGameOver();

        cupManager.SpawnCups(playerData.currentCups);
        StartCoroutine(StartNewRound());
    }

    IEnumerator StartNewRound()
    {
        isPlaying = false;
        cupManager.DisableAllCupButtons();

        yield return StartCoroutine(cupManager.RevealPhase());
        yield return StartCoroutine(cupManager.MixPhase());

        cupManager.EnableAllCupButtons();
        isPlaying = true;
    }

    public void OnCupClicked(Cup clickedCup)
    {
        if (!isPlaying) return;

        isPlaying = false;
        cupManager.DisableAllCupButtons();
        StartCoroutine(HandleGuess(clickedCup));
    }

    IEnumerator HandleGuess(Cup clickedCup)
    {
        bool isCorrect = cupManager.RevealCup(clickedCup);
        yield return new WaitForSeconds(1f);

        if (isCorrect)
        {
            HandleCorrectGuess();
        }
        else
        {
            HandleWrongGuess();
        }

        yield return new WaitForSeconds(0.5f);

        if (playerData.health > 0)
        {
            cupManager.ResetCups();
            StartCoroutine(StartNewRound());
        }
    }

    void HandleCorrectGuess()
    {
        playerData.AddScore(10 * playerData.currentLevel);
        correctStreakCount++;

        uiManager.FlashScore(true);
        uiManager.UpdateAllUI();

        if (correctStreakCount >= correctGuessesForNewCup && playerData.currentCups < maxCups)
        {
            correctStreakCount = 0;
            playerData.LevelUp();
            cupManager.SpawnCups(playerData.currentCups);
            uiManager.ShowLevelUp();
        }
    }

    void HandleWrongGuess()
    {
        playerData.LoseHealth();
        correctStreakCount = 0;

        uiManager.FlashHealth(false);
        uiManager.UpdateAllUI();

        if (playerData.health <= 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        isPlaying = false;
        uiManager.ShowGameOver(playerData.score, playerData.currentLevel);
    }

    public void RestartGame()
    {
        InitializeGame();
    }
}
