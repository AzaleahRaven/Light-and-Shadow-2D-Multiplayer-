using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerScore : MonoBehaviourPun
{
    public const string SunlingScoreKey = "sunlingScore";
    public const string MoonlingScoreKey = "moonlingScore";
    public const string ScoreKey = "score";
    public const int DiamondGoal = 3;

    private bool isSunling = false;

    private void Start()
    {
        isSunling = gameObject.CompareTag("Sunling");

        if (photonView.IsMine)
        {
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
        int sunScore = GetScore(SunlingScoreKey);
        int moonScore = GetScore(MoonlingScoreKey);

        if (isSunling)
            sunScore += amount;
        else
            moonScore += amount;

        Hashtable props = new Hashtable
        {
            { SunlingScoreKey, sunScore },
            { MoonlingScoreKey, moonScore },
            { ScoreKey, sunScore + moonScore }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        Debug.Log($"[Score] Sunling: {sunScore} | Moonling: {moonScore}");

        // ONLY update UI - NO WIN CHECK - WIN IS ONLY FROM DOOR
        if (ScoreUIManager.Instance != null)
            ScoreUIManager.Instance.UpdateScoreUI();
    }

    public int GetScore(string key)
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(key, out object val))
            return (int)val;
        return 0;
    }
}