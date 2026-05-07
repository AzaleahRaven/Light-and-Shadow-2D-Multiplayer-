using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerScore : MonoBehaviourPun
{
    public const string ScoreKey = "score";
    public const int WinScore = 10;

    private bool gameEnded = false;

    private void Start()
    {
        if (photonView.IsMine)
        {
            Hashtable props = new Hashtable
            {
                { ScoreKey, 0 }
            };

            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
    }

    public void AddScore(int amount)
    {
        if (!photonView.IsMine || gameEnded) return;

        int currentScore = GetMyScore();
        int newScore = currentScore + amount;

        Hashtable props = new Hashtable
        {
            { ScoreKey, newScore }
        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        if (newScore >= WinScore)
        {
            photonView.RPC(nameof(ShowWinLoseRPC), RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
        }
    }

    private int GetMyScore()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(ScoreKey, out object value))
            return (int)value;

        return 0;
    }

    [PunRPC]
    private void ShowWinLoseRPC(int winnerActorNumber)
    {
        gameEnded = true;

        if (ScoreUIManager.Instance == null) return;

        if (PhotonNetwork.LocalPlayer.ActorNumber == winnerActorNumber)
            ScoreUIManager.Instance.ShowResult("YOU WIN");
        else
            ScoreUIManager.Instance.ShowResult("YOU LOSE");
    }
}