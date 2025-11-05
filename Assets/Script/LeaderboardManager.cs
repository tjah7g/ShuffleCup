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

        StartCoroutine(APIManager.Instance.GetLeaderboard(topPlayersToShow, (entries, error) =>
        {
            if (error != null)
            {
                UpdateStatus($"Failed to load: {error}");
                return;
            }

            DisplayLeaderboard(entries);
            UpdateStatus("");
        }));
    }

    void DisplayLeaderboard(List<LeaderboardEntry> entries)
    {
        // Clear existing entries
        foreach (Transform child in leaderboardContent)
        {
            Destroy(child.gameObject);
        }

        // Create new entries
        if (entries != null && entries.Count > 0)
        {
            for (int i = 0; i < entries.Count; i++)
            {
                GameObject entryObj = Instantiate(leaderboardEntryPrefab, leaderboardContent);
                LeaderboardEntryUI entryUI = entryObj.GetComponent<LeaderboardEntryUI>();

                if (entryUI != null)
                {
                    entryUI.SetData(i + 1, entries[i].playerName, entries[i].score, entries[i].level);
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