using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Text")]
    public TMP_Text usernameText;
    public TMP_Text scoreText;
    public TMP_Text healthText;
    public TMP_Text levelText;

    [Header("Game Over")]
    public GameObject gameOverPanel;
    public TMP_Text finalScoreText;
    public TMP_Text playerRankText;
    public TMP_Text submitStatusText;
    public Button restartButton;
    public Button viewLeaderboardButton;

    [Header("Leaderboard")]
    public LeaderboardManager leaderboardManager;

    [Header("References")]
    public PlayerData playerData;

    void Start()
    {
        if (restartButton != null)
            restartButton.onClick.AddListener(() => GameManager.Instance.RestartGame());

        if (viewLeaderboardButton != null)
            viewLeaderboardButton.onClick.AddListener(() => leaderboardManager?.ShowLeaderboard());
    }

    public void UpdateAllUI()
    {
        UpdateUsername();
        UpdateScore();
        UpdateHealth();
        UpdateLevel();
    }

    public void UpdateUsername()
    {
        if (usernameText != null && playerData != null)
            usernameText.text = playerData.username;
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

    public void ShowGameOver(int finalScore, int finalLevel, bool scoreSubmitted)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            if (finalScoreText != null)
            {
                finalScoreText.text = $"Game Over!\n\n" +
                                     $"Username: {playerData.username}\n" +
                                     $"Final Score: {finalScore}\n" +
                                     $"Level Reached: {finalLevel}";
            }

            if (submitStatusText != null)
            {
                if (scoreSubmitted)
                {
                    submitStatusText.text = "? Score submitted!";
                    submitStatusText.color = Color.green;

                    // Get player rank setelah submit
                    StartCoroutine(APIManager.Instance.GetPlayerRank(
                        playerData.username,
                        (rank, score, error) =>
                        {
                            if (error == null && playerRankText != null)
                            {
                                playerRankText.text = $"Your Rank: #{rank}";
                                playerRankText.color = Color.yellow;
                            }
                        }
                    ));
                }
                else
                {
                    submitStatusText.text = "? Failed to submit score";
                    submitStatusText.color = Color.red;
                }
            }
        }
    }

    public void HideGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }
}