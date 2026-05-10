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
    public TextMeshProUGUI player1ScoreText;
    public TextMeshProUGUI player2ScoreText;

    [Header("Diamond Count")]
    public TextMeshProUGUI player1DiamondsText;
    public TextMeshProUGUI player2DiamondsText;

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

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdateScoreUI();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
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
        Player[] players = PhotonNetwork.PlayerList;

        if (players.Length > 0)
        {
            int score1 = GetPlayerScore(players[0]);
            if (player1ScoreText != null)
                player1ScoreText.text = "Sunling: " + score1;
            if (player1DiamondsText != null)
                player1DiamondsText.text = $"💎 {score1}/{PlayerScore.DiamondGoal}";
        }

        if (players.Length > 1)
        {
            int score2 = GetPlayerScore(players[1]);
            if (player2ScoreText != null)
                player2ScoreText.text = "Moonling: " + score2;
            if (player2DiamondsText != null)
                player2DiamondsText.text = $"💎 {score2}/{PlayerScore.DiamondGoal}";
        }
    }

    private int GetPlayerScore(Player player)
    {
        if (player.CustomProperties.TryGetValue(PlayerScore.ScoreKey, out object value))
            return (int)value;
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