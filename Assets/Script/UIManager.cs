using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Text")]
    public TMP_Text scoreText;
    public TMP_Text healthText;
    public TMP_Text levelText;

    [Header("Game Over")]
    public GameObject gameOverPanel;
    public TMP_Text finalScoreText;
    public Button restartButton;
    public Button submitScoreButton;
    public TMP_InputField playerNameInput;

    [Header("Leaderboard")]
    public LeaderboardManager leaderboardManager;
    public Button showLeaderboardButton;

    [Header("References")]
    public PlayerData playerData;

    void Start()
    {
        if (restartButton != null)
            restartButton.onClick.AddListener(() => GameManager.Instance.RestartGame());

        if (submitScoreButton != null)
            submitScoreButton.onClick.AddListener(SubmitScore);

        if (showLeaderboardButton != null)
            showLeaderboardButton.onClick.AddListener(() => leaderboardManager?.ShowLeaderboard());
    }

    void SubmitScore()
    {
        string playerName = playerNameInput != null ? playerNameInput.text : "Anonymous";

        if (string.IsNullOrEmpty(playerName))
        {
            playerName = "Anonymous";
        }

        if (submitScoreButton != null)
            submitScoreButton.interactable = false;

        StartCoroutine(APIManager.Instance.SubmitScore(
            playerName,
            playerData.score,
            playerData.currentLevel,
            (success, message) =>
            {
                if (success)
                {
                    Debug.Log("Score submitted successfully!");
                    if (finalScoreText != null)
                        finalScoreText.text += "\n\n? Score Submitted!";
                }
                else
                {
                    Debug.LogError($"Failed to submit score: {message}");
                }

                if (submitScoreButton != null)
                    submitScoreButton.interactable = true;
            }
        ));
    }

    public void UpdateAllUI()
    {
        UpdateScore();
        UpdateHealth();
        UpdateLevel();
    }

    public void UpdateScore()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + playerData.score;
    }

    public void UpdateHealth()
    {
        if (healthText != null)
            healthText.text = "Health: " + playerData.health;
    }

    public void UpdateLevel()
    {
        if (levelText != null)
            levelText.text = "Level: " + playerData.currentLevel + " | Cups: " + playerData.currentCups;
    }

    public void FlashScore(bool correct)
    {
        StartCoroutine(FlashText(scoreText, correct ? Color.green : Color.red));
    }

    public void FlashHealth(bool correct)
    {
        StartCoroutine(FlashText(healthText, correct ? Color.green : Color.red));
    }

    IEnumerator FlashText(TMP_Text text, Color flashColor)
    {
        if (text == null) yield break;

        Color originalColor = text.color;
        text.color = flashColor;
        yield return new WaitForSeconds(0.5f);
        text.color = originalColor;
    }

    public void ShowLevelUp()
    {
        StartCoroutine(LevelUpAnimation());
    }

    IEnumerator LevelUpAnimation()
    {
        if (levelText == null) yield break;

        Color original = levelText.color;
        float originalSize = levelText.fontSize;

        for (int i = 0; i < 3; i++)
        {
            levelText.color = Color.yellow;
            levelText.fontSize = originalSize * 1.3f;
            yield return new WaitForSeconds(0.15f);

            levelText.color = original;
            levelText.fontSize = originalSize;
            yield return new WaitForSeconds(0.15f);
        }
    }

    public void ShowGameOver(int finalScore, int finalLevel)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            if (finalScoreText != null)
                finalScoreText.text = $"Game Over!\n\nFinal Score: {finalScore}\nLevel Reached: {finalLevel}";
        }
    }

    public void HideGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }
}