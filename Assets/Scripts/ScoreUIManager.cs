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

    [Header("Result UI")]
    public GameObject resultPanel;
    public TextMeshProUGUI resultText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (resultPanel != null)
            resultPanel.SetActive(false);

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

        int score1 = 0;
        int score2 = 0;

        if (players.Length > 0)
            score1 = GetPlayerScore(players[0]);

        if (players.Length > 1)
            score2 = GetPlayerScore(players[1]);

        if (player1ScoreText != null)
            player1ScoreText.text = "Player 1 Score: " + score1;

        if (player2ScoreText != null)
            player2ScoreText.text = "Player 2 Score: " + score2;
    }

    private int GetPlayerScore(Player player)
    {
        if (player.CustomProperties.TryGetValue(PlayerScore.ScoreKey, out object value))
            return (int)value;

        return 0;
    }
}