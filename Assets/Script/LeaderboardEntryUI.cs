using UnityEngine;
using TMPro;

public class LeaderboardEntryUI : MonoBehaviour
{
    public TMP_Text rankText;
    public TMP_Text nameText;
    public TMP_Text scoreText;
    public TMP_Text levelText;

    public void SetData(int rank, string playerName, int score, int level)
    {
        if (rankText != null)
            rankText.text = $"#{rank}";

        if (nameText != null)
            nameText.text = playerName;

        if (scoreText != null)
            scoreText.text = $"Score: {score}";

        if (levelText != null)
            levelText.text = $"Lv.{level}";
    }
}