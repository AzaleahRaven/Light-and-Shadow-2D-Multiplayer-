using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerScore : MonoBehaviourPun
{
    public const string ScoreKey = "score";
    public const int DiamondGoal = 3; // Need 3 diamonds to win

    private bool gameEnded = false;

    private void Start()
    {
        if (photonView.IsMine)
        {
            Hashtable props = new Hashtable { { ScoreKey, 0 } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
    }

    public void AddScore(int amount)
    {
        if (!photonView.IsMine || gameEnded) return;

        int currentScore = GetMyScore();
        int newScore = currentScore + amount;

        Hashtable props = new Hashtable { { ScoreKey, newScore } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        Debug.Log($"[PlayerScore] Score updated: {newScore}/{DiamondGoal}");

        // Check if this player collected all diamonds
        if (newScore >= DiamondGoal)
        {
            photonView.RPC(nameof(PlayerFinishedRPC), RpcTarget.All,
                          PhotonNetwork.LocalPlayer.ActorNumber);
        }
    }

    public int GetMyScore()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(ScoreKey, out object value))
            return (int)value;
        return 0;
    }

    [PunRPC]
    private void PlayerFinishedRPC(int actorNumber)
    {
        gameEnded = true;

        if (ScoreUIManager.Instance == null) return;

        // Find the player name
        string playerName = actorNumber == 1 ? "Sunling" : "Moonling";

        if (PhotonNetwork.LocalPlayer.ActorNumber == actorNumber)
            ScoreUIManager.Instance.ShowResult($"{playerName} collected all diamonds!\nYOU WIN! 🎉");
        else
            ScoreUIManager.Instance.ShowResult($"{playerName} collected all diamonds!\nKeep going!");
    }
}