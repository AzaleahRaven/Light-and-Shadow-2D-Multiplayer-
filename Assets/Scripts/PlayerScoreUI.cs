using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI p1ScoreText;
    [SerializeField] private TextMeshProUGUI p2ScoreText;

    private void Update()
    {
        if (PhotonNetwork.PlayerList.Length > 0)
        {
            int score1 = GetScore(PhotonNetwork.PlayerList[0]);
            if (p1ScoreText != null)
                p1ScoreText.text = "Player 1\nScore: " + score1;
        }

        if (PhotonNetwork.PlayerList.Length > 1)
        {
            int score2 = GetScore(PhotonNetwork.PlayerList[1]);
            if (p2ScoreText != null)
                p2ScoreText.text = "Player 2\nScore: " + score2;
        }
    }

    private int GetScore(Photon.Realtime.Player player)
    {
        if (player.CustomProperties.TryGetValue(PlayerScore.ScoreKey, out object val))
            return (int)val;
        return 0;
    }
}