using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CoinPickup : MonoBehaviourPun
{
    public int scoreValue = 1;
    private bool collected = false;

    private void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;

        PlayerScore playerScore = other.GetComponent<PlayerScore>();
        if (playerScore == null) return;

        if (!playerScore.photonView.IsMine) return;

        photonView.RPC(nameof(CollectCoinRPC), RpcTarget.AllBuffered, playerScore.photonView.ViewID);
    }

    [PunRPC]
    private void CollectCoinRPC(int playerViewId)
    {
        if (collected) return;
        collected = true;

        PhotonView playerView = PhotonView.Find(playerViewId);
        if (playerView != null)
        {
            PlayerScore playerScore = playerView.GetComponent<PlayerScore>();
            if (playerScore != null && playerScore.photonView.IsMine)
            {
                playerScore.AddScore(scoreValue);
            }
        }

        gameObject.SetActive(false);
    }
}