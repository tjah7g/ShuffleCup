using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LeaderboardManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject leaderboardPanel;
    public Transform leaderboardContent;
    public GameObject leaderboardEntryPrefab;
    public Button closeButton;
    public Button refreshButton;
    public TMP_Text statusText;
    public TMP_Text titleText;

    [Header("Settings")]
    public int topPlayersToShow = 10;

    void Start()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(HideLeaderboard);

        if (refreshButton != null)
            refreshButton.onClick.AddListener(LoadLeaderboard);
    }

    public void ShowLeaderboard()
    {
        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(true);

        LoadLeaderboard();
    }

    public void HideLeaderboard()
    {
        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(false);
    }

    public void LoadLeaderboard()
    {
        UpdateStatus("Loading leaderboard...");

        StartCoroutine(APIManager.Instance.GetLeaderboard(topPlayersToShow, (scores, error) =>
        {
            if (error != null)
            {
                UpdateStatus($"Failed to load: {error}");
                return;
            }

            DisplayLeaderboard(scores);
            UpdateStatus("");
        }));
    }

    void DisplayLeaderboard(List<PlayerScore> scores)
    {
        Debug.Log($"DisplayLeaderboard called with {scores?.Count ?? 0} scores");

        // Clear existing
        foreach (Transform child in leaderboardContent)
        {
            Destroy(child.gameObject);
        }

        if (scores != null && scores.Count > 0)
        {
            for (int i = 0; i < scores.Count; i++)
            {
                Debug.Log($"Creating entry {i + 1}: {scores[i].playerName} - {scores[i].score}");

                GameObject entryObj = Instantiate(leaderboardEntryPrefab, leaderboardContent);
                LeaderboardEntryUI entryUI = entryObj.GetComponent<LeaderboardEntryUI>();

                if (entryUI != null)
                {
                    entryUI.SetData(i + 1, scores[i]);
                }
                else
                {
                    Debug.LogError("LeaderboardEntryUI component not found on prefab!");
                }
            }
        }
        else
        {
            UpdateStatus("No leaderboard data available");
        }
    }

    void UpdateStatus(string message)
    {
        if (statusText != null)
            statusText.text = message;
    }
}
