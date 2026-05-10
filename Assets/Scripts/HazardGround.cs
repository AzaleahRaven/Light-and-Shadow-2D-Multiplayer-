using UnityEngine;
using Photon.Pun;

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

        // Get PlayerControllerPun directly
        PlayerControllerPun player = obj.GetComponent<PlayerControllerPun>();
        if (player == null) return;

        // Only the owner kills themselves
        PhotonView pv = obj.GetComponent<PhotonView>();
        if (pv == null || !pv.IsMine) return;

        bool isSunling = obj.CompareTag("Sunling");
        bool isMoonling = obj.CompareTag("Moonling");

        bool shouldDie = false;

        switch (groundType.groundType)
        {
            case GroundType.Type.Poison:
                shouldDie = true;
                break;
            case GroundType.Type.Sunling:
                // Yellow ground kills Moonling only
                if (isMoonling) shouldDie = true;
                break;
            case GroundType.Type.Moonling:
                // Blue ground kills Sunling only
                if (isSunling) shouldDie = true;
                break;
            case GroundType.Type.Normal:
                shouldDie = false;
                break;
        }

        if (shouldDie)
        {
            // Call DieRPC directly on the component
            player.DieRPC();
            pv.RPC("DieRPC", RpcTarget.Others);
            Debug.Log($"[HazardGround] {obj.name} touched {groundType.groundType} ground and died!");
        }
    }
}