using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class LeaderboardEntry
{
    public string playerName;
    public int score;
    public int level;
    public string timestamp;
}

[System.Serializable]
public class LeaderboardResponse
{
    public List<LeaderboardEntry> leaderboard;
}

[System.Serializable]
public class SubmitScoreData
{
    public string playerName;
    public int score;
    public int level;
}

public class APIManager : MonoBehaviour
{
    public static APIManager Instance { get; private set; }

    [Header("API Settings")]
    public string apiBaseURL = "https://your-api.com/api"; // Ganti dengan URL API Anda
    public int requestTimeout = 10;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Submit score to leaderboard
    public IEnumerator SubmitScore(string playerName, int score, int level, Action<bool, string> callback)
    {
        SubmitScoreData data = new SubmitScoreData
        {
            playerName = playerName,
            score = score,
            level = level
        };

        string jsonData = JsonUtility.ToJson(data);

        using (UnityWebRequest request = UnityWebRequest.Post($"{apiBaseURL}/leaderboard", jsonData, "application/json"))
        {
            request.timeout = requestTimeout;

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                callback?.Invoke(true, "Score submitted successfully!");
            }
            else
            {
                callback?.Invoke(false, $"Error: {request.error}");
            }
        }
    }

    // Get leaderboard (Top N players)
    public IEnumerator GetLeaderboard(int topN, Action<List<LeaderboardEntry>, string> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Get($"{apiBaseURL}/leaderboard?top={topN}"))
        {
            request.timeout = requestTimeout;

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    string jsonResponse = request.downloadHandler.text;
                    LeaderboardResponse response = JsonUtility.FromJson<LeaderboardResponse>(jsonResponse);
                    callback?.Invoke(response.leaderboard, null);
                }
                catch (Exception e)
                {
                    callback?.Invoke(null, $"Parse error: {e.Message}");
                }
            }
            else
            {
                callback?.Invoke(null, $"Error: {request.error}");
            }
        }
    }

    // Get player rank
    public IEnumerator GetPlayerRank(string playerName, Action<int, string> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Get($"{apiBaseURL}/leaderboard/rank?name={UnityWebRequest.EscapeURL(playerName)}"))
        {
            request.timeout = requestTimeout;

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    string jsonResponse = request.downloadHandler.text;
                    int rank = JsonUtility.FromJson<RankResponse>(jsonResponse).rank;
                    callback?.Invoke(rank, null);
                }
                catch (Exception e)
                {
                    callback?.Invoke(-1, $"Parse error: {e.Message}");
                }
            }
            else
            {
                callback?.Invoke(-1, $"Error: {request.error}");
            }
        }
    }

    [System.Serializable]
    private class RankResponse
    {
        public int rank;
    }
}