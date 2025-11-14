// ============================================
// LoginManager.cs - Handles username input before game starts
// ============================================
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject loginPanel;
    public TMP_InputField usernameInput;
    public Button playButton;
    public TMP_Text errorText;

    [Header("Settings")]
    public int minUsernameLength = 3;
    public int maxUsernameLength = 20;

    private string currentUsername;

    void Start()
    {
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayButtonClicked);

        if (usernameInput != null)
            usernameInput.characterLimit = maxUsernameLength;

        ShowLogin();
    }

    public void ShowLogin()
    {
        if (loginPanel != null)
            loginPanel.SetActive(true);

        if (errorText != null)
            errorText.text = "";

        // Load saved username if exists
        if (PlayerPrefs.HasKey("Username"))
        {
            string savedUsername = PlayerPrefs.GetString("Username");
            if (usernameInput != null)
                usernameInput.text = savedUsername;
        }
    }

    void OnPlayButtonClicked()
    {
        string username = usernameInput != null ? usernameInput.text.Trim() : "";

        // Validasi username
        if (string.IsNullOrEmpty(username))
        {
            ShowError("Username tidak boleh kosong!");
            return;
        }

        if (username.Length < minUsernameLength)
        {
            ShowError($"Username minimal {minUsernameLength} karakter!");
            return;
        }

        if (username.Length > maxUsernameLength)
        {
            ShowError($"Username maksimal {maxUsernameLength} karakter!");
            return;
        }

        // Simpan username
        currentUsername = username;
        PlayerPrefs.SetString("Username", username);
        PlayerPrefs.Save();

        Debug.Log($"Username set: {currentUsername}");

        // Mulai game
        StartGame();
    }

    void StartGame()
    {
        if (loginPanel != null)
            loginPanel.SetActive(false);

        // Notify GameManager to start
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetPlayerUsername(currentUsername);
            GameManager.Instance.InitializeGame();
            Debug.Log("Game started!");
        }
        else
        {
            Debug.LogError("GameManager.Instance is NULL! Pastikan GameManager ada di scene.");
        }
    }

    void ShowError(string message)
    {
        if (errorText != null)
        {
            errorText.text = message;
            errorText.color = Color.red;
        }

        Debug.LogWarning(message);
    }

    public string GetCurrentUsername()
    {
        return currentUsername;
    }
}