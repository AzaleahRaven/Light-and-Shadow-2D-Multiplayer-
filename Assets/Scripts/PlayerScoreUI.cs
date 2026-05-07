using ClearSky;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    private SimplePlayerControllerPun localPlayer;

    private void Update()
    {
        if (localPlayer == null)
        {
            FindLocalPlayer();
            return;
        }

        scoreText.text = "Score: " + localPlayer.Score;
    }

    private void FindLocalPlayer()
    {
        SimplePlayerControllerPun[] players = FindObjectsOfType<SimplePlayerControllerPun>();
        foreach (var player in players)
        {
            if (player.photonView.IsMine)
            {
                localPlayer = player;
                break;
            }
        }
    }
}