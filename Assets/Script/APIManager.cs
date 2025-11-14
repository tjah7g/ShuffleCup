using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// Model sesuai dengan PlayerScore di backend
[System.Serializable]
public class PlayerScore
{
    public int id;              // Tambah ini (lowercase)
    public string playerName;   // Ganti dari PlayerName ke playerName
    public int score;           // Ganti dari Score ke score
    public int level;           // Ganti dari Level ke level
    public string createdAt;    // Ganti dari CreatedAt ke createdAt
}
// Response wrapper dari backend
[System.Serializable]
public class PlayerScoreList
{
    public List<PlayerScore> scores;
}

public class APIManager : MonoBehaviour
{
    public static APIManager Instance { get; private set; }

    [Header("API Settings")]
    [Tooltip("URL Backend API Anda (tanpa /api)")]
    public string apiBaseURL = "https://localhost:7267"; // Default ASP.NET Core HTTPS port
    public int requestTimeout = 10;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Submit score menggunakan POST /api/scores
    public IEnumerator SubmitScore(string playerName, int score, int level, Action<bool, string> callback)
    {
        // GANTI seluruh isi method dengan ini:

        // Format JSON manual (lebih reliable)
        string jsonData = $"{{\"playerName\":\"{playerName}\",\"score\":{score},\"level\":{level}}}";

        Debug.Log($"Submitting score: {jsonData}");

        using (UnityWebRequest request = new UnityWebRequest($"{apiBaseURL}/api/scores", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = requestTimeout;
            request.certificateHandler = new AcceptAllCertificates();

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Score submitted successfully!");
                Debug.Log($"Response: {request.downloadHandler.text}"); // Lihat response
                callback?.Invoke(true, "Score berhasil disimpan!");
            }
            else
            {
                Debug.LogError($"Error submitting score: {request.error}");
                Debug.LogError($"Response: {request.downloadHandler.text}"); // Lihat error detail
                callback?.Invoke(false, $"Error: {request.error}");
            }
        }
    }

    // Get leaderboard menggunakan GET /api/scores/top/{n}
    public IEnumerator GetLeaderboard(int topN, Action<List<PlayerScore>, string> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Get($"{apiBaseURL}/api/scores/top/{topN}"))
        {
            request.timeout = requestTimeout;
            request.certificateHandler = new AcceptAllCertificates();

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    string responseText = request.downloadHandler.text;
                    Debug.Log($"Leaderboard raw response: {responseText}"); // CEK INI DI CONSOLE

                    // Backend return array langsung, bukan object wrapper
                    // Tambah wrapper manual
                    string wrappedJson = "{\"scores\":" + responseText + "}";
                    Debug.Log($"Wrapped JSON: {wrappedJson}");

                    PlayerScoreList scoreList = JsonUtility.FromJson<PlayerScoreList>(wrappedJson);

                    if (scoreList != null && scoreList.scores != null)
                    {
                        Debug.Log($"Parsed {scoreList.scores.Count} scores");
                        callback?.Invoke(scoreList.scores, null);
                    }
                    else
                    {
                        Debug.LogError("Score list is null after parsing");
                        callback?.Invoke(null, "Failed to parse leaderboard");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Parse error: {e.Message}");
                    Debug.LogError($"Stack trace: {e.StackTrace}");
                    callback?.Invoke(null, $"Parse error: {e.Message}");
                }
            }
            else
            {
                Debug.LogError($"Error getting leaderboard: {request.error}");
                callback?.Invoke(null, $"Error: {request.error}");
            }
        }
    }

    // Get player rank menggunakan GET /api/scores/rank/{playerName}
    public IEnumerator GetPlayerRank(string playerName, Action<int, int, string> callback)
    {
        string encodedName = UnityWebRequest.EscapeURL(playerName);

        using (UnityWebRequest request = UnityWebRequest.Get($"{apiBaseURL}/api/scores/rank/{encodedName}"))
        {
            request.timeout = requestTimeout;
            request.certificateHandler = new AcceptAllCertificates();

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    string responseText = request.downloadHandler.text;
                    Debug.Log($"Rank response: {responseText}");

                    RankResponse response = JsonUtility.FromJson<RankResponse>(responseText);
                    callback?.Invoke(response.rank, response.score, null);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Parse error: {e.Message}");
                    callback?.Invoke(-1, 0, $"Parse error: {e.Message}");
                }
            }
            else
            {
                Debug.LogError($"Error getting rank: {request.error}");
                callback?.Invoke(-1, 0, $"Error: {request.error}");
            }
        }
    }

    [System.Serializable]
    private class RankResponse
    {
        public string player;
        public int score;
        public int rank;
    }
}

// Helper class untuk accept self-signed certificates (HANYA UNTUK DEVELOPMENT)
public class AcceptAllCertificates : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true; // JANGAN gunakan di production!
    }
}