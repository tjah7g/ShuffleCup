using UnityEngine;
using TMPro;

public class LeaderboardEntryUI : MonoBehaviour
{
    public TMP_Text rankText;
    public TMP_Text playerNameText;
    public TMP_Text scoreText;
    public TMP_Text levelText;

    public void SetData(int rank, PlayerScore playerScore)
    {
        if (rankText != null)
            rankText.text = $"#{rank}";

        if (playerNameText != null)
            playerNameText.text = playerScore.playerName; // lowercase

        if (scoreText != null)
            scoreText.text = playerScore.score.ToString(); // lowercase

        if (levelText != null)
            levelText.text = $"Lv.{playerScore.level}"; // lowercase

        // Highlight top 3
        if (rank <= 3)
        {
            Color highlightColor = rank == 1 ? new Color(1f, 0.84f, 0f) :
                                   rank == 2 ? new Color(0.75f, 0.75f, 0.75f) :
                                   new Color(0.8f, 0.5f, 0.2f);

            if (rankText != null)
                rankText.color = highlightColor;
        }
    }
}