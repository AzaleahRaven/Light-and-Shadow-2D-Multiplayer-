using Photon.Pun;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class PlayerController : MonoBehaviourPun
{
    private PlayerMovement movement;

    public PlayerMovement Movement => movement;

    private void Awake()
    {   
        var rb = GetComponent<Rigidbody2D>();
        var anim = GetComponentInChildren<Animator>();
        var stats = GetComponent<PlayerStats>();
        var input = GetComponent<PlayerInputHandler>();

        movement = new PlayerMovement(rb, anim, transform, stats, input);
    }

    private void Start()
    {
        Debug.Log($"[PlayerController] on {gameObject.name} | IsMine: {photonView.IsMine} | Owner: {photonView.Owner.NickName}");

        if (photonView.IsMine) photonView.RPC(nameof(SyncNameRPC), RpcTarget.AllBuffered, photonView.Owner.NickName);

        CinemachineVirtualCameraBase vcam = GetComponentInChildren<CinemachineVirtualCameraBase>();
        if (vcam != null) vcam.gameObject.SetActive(photonView.IsMine);
    }

    private void Update()
    {
        if (!photonView.IsMine) return;
        movement.HandleMovement();
    }

    [PunRPC]
    private void SyncNameRPC(string nickname)
    {
        gameObject.name = nickname;
    }

    private void OnDrawGizmosSelected()
    {
        if (movement != null) movement.DrawGroundCheckGizmo();
    }
}