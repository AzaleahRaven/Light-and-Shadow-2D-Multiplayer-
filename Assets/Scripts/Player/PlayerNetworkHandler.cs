using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class PlayerNetworkHandler : MonoBehaviourPun
{
    private void Start()
    {
        if (!photonView.IsMine)
        {
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null) cam.gameObject.SetActive(false);
        }
    }

    [PunRPC]
    public void AddScoreRPC(int amount)
    {
        PlayerScore score = GetComponent<PlayerScore>();
        if (score != null) score.AddScore(amount);
    }
}
