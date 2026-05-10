using UnityEngine;
using Photon.Pun;

// Attach this to ALL ground objects
// It checks who touches it and kills accordingly
public class HazardGround : MonoBehaviour
{
    private GroundType groundType;

    private void Awake()
    {
        groundType = GetComponent<GroundType>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckAndKill(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckAndKill(collision.gameObject);
    }

    private void CheckAndKill(GameObject obj)
    {
        if (groundType == null) return;

        // Get the PlayerControllerPun on the object
        PlayerControllerPun player = obj.GetComponent<PlayerControllerPun>();
        if (player == null) return;

        // Only the master client handles death to avoid double kills
        if (!PhotonNetwork.IsMasterClient) return;

        bool isSunling = obj.CompareTag("Sunling");
        bool isMoonling = obj.CompareTag("Moonling");

        switch (groundType.groundType)
        {
            case GroundType.Type.Poison:
                // Both die on poison
                KillPlayer(obj);
                break;

            case GroundType.Type.Sunling:
                // Only Moonling dies on yellow ground
                if (isMoonling) KillPlayer(obj);
                break;

            case GroundType.Type.Moonling:
                // Only Sunling dies on blue ground
                if (isSunling) KillPlayer(obj);
                break;

            case GroundType.Type.Normal:
                // Nobody dies on normal ground
                break;
        }
    }

    private void KillPlayer(GameObject player)
    {
        PhotonView pv = player.GetComponent<PhotonView>();
        if (pv != null)
        {
            pv.RPC("DieRPC", RpcTarget.All);
        }
    }
}