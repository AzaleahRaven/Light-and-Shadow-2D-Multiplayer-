using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using TMPro;

public class ScoreUIManager : MonoBehaviourPunCallbacks
{
    public static ScoreUIManager Instance;

    [Header("Score Text")]
    public TextMeshProUGUI player1ScoreText; // Sunling
    public TextMeshProUGUI player2ScoreText; // Moonling

    [Header("Result UI")]
    public GameObject resultPanel;
    public TextMeshProUGUI resultText;
    public Button restartButton;
    public Button quitButton;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (resultPanel != null)
            resultPanel.SetActive(false);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitToLobby);

        UpdateScoreUI();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        UpdateScoreUI();
    }

    public void ShowResult(string message)
    {
        if (resultPanel != null)
            resultPanel.SetActive(true);
        if (resultText != null)
            resultText.text = message;
    }

    public void UpdateScoreUI()
    {
        // Get scores from local player properties
        int sunScore = GetScore(PlayerScore.SunlingScoreKey);
        int moonScore = GetScore(PlayerScore.MoonlingScoreKey);

        if (player1ScoreText != null)
            player1ScoreText.text = $"Sunling: {sunScore}/{PlayerScore.DiamondGoal}";

        if (player2ScoreText != null)
            player2ScoreText.text = $"Moonling: {moonScore}/{PlayerScore.DiamondGoal}";
    }

    private int GetScore(string key)
    {
        // Check all players for the score
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue(key, out object val))
                return (int)val;
        }
        return 0;
    }

    private void RestartGame()
    {
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel("GamePlay");
    }

    private void QuitToLobby()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("Lobby");
    }
}