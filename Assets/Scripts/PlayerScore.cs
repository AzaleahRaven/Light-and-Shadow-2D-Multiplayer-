using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerScore : MonoBehaviourPun
{
    public const string SunlingScoreKey = "sunlingScore";
    public const string MoonlingScoreKey = "moonlingScore";
    public const string ScoreKey = "score"; // kept for compatibility
    public const int DiamondGoal = 3;

    private bool gameEnded = false;
    private bool isSunling = false;

    private void Start()
    {
        // Check which character this is
        isSunling = gameObject.CompareTag("Sunling");

        if (photonView.IsMine)
        {
            // Reset scores at start
            Hashtable props = new Hashtable
            {
                { SunlingScoreKey, 0 },
                { MoonlingScoreKey, 0 },
                { ScoreKey, 0 }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
    }

    public void AddScore(int amount)
    {
        if (gameEnded) return;

        // Get current scores
        int sunScore = GetScore(SunlingScoreKey);
        int moonScore = GetScore(MoonlingScoreKey);

        // Add to correct character score
        if (isSunling)
            sunScore += amount;
        else
            moonScore += amount;

        // Update Photon properties
        Hashtable props = new Hashtable
        {
            { SunlingScoreKey, sunScore },
            { MoonlingScoreKey, moonScore },
            { ScoreKey, sunScore + moonScore }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        Debug.Log($"[Score] Sunling: {sunScore} | Moonling: {moonScore}");

        // Update UI immediately
        if (ScoreUIManager.Instance != null)
            ScoreUIManager.Instance.UpdateScoreUI();

        // Check win
        if (isSunling && sunScore >= DiamondGoal)
        {
            gameEnded = true;
            photonView.RPC(nameof(ShowResultRPC), RpcTarget.All, "Sunling", true);
        }
        else if (!isSunling && moonScore >= DiamondGoal)
        {
            gameEnded = true;
            photonView.RPC(nameof(ShowResultRPC), RpcTarget.All, "Moonling", true);
        }
    }

    private int GetScore(string key)
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(key, out object val))
            return (int)val;
        return 0;
    }

    [PunRPC]
    private void ShowResultRPC(string winner, bool isWin)
    {
        gameEnded = true;
        if (ScoreUIManager.Instance != null)
            ScoreUIManager.Instance.ShowResult($"{winner} collected all diamonds!\n🎉 YOU WIN! 🎉");
    }
}