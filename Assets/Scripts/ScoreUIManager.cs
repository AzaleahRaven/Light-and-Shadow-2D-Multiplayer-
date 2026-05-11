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

    // NO result panel here - that is handled by WinManager only!

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateScoreUI();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        UpdateScoreUI();
    }

    // ONLY updates score display - NO WIN CHECK
    public void UpdateScoreUI()
    {
        int sunScore = 0;
        int moonScore = 0;

        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue(
                PlayerScore.SunlingScoreKey, out object s))
                sunScore = (int)s;

            if (player.CustomProperties.TryGetValue(
                PlayerScore.MoonlingScoreKey, out object m))
                moonScore = (int)m;
        }

        if (player1ScoreText != null)
            player1ScoreText.text = $"Sunling: {sunScore}/{PlayerScore.DiamondGoal}";

        if (player2ScoreText != null)
            player2ScoreText.text = $"Moonling: {moonScore}/{PlayerScore.DiamondGoal}";
    }

    // REMOVED ShowResult - WinManager handles all win UI now
}