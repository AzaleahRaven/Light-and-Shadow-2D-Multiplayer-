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
        Debug.Log($"[PlayerController] on {gameObject.name} | IsMine: {photonView.IsMine}");

        if (photonView.IsMine)
            photonView.RPC(nameof(SyncNameRPC), RpcTarget.AllBuffered, photonView.Owner.NickName);

        // Camera only active for owned character
        // In single player, only Sunling (actor 1) gets the camera
        CinemachineVirtualCameraBase vcam = GetComponentInChildren<CinemachineVirtualCameraBase>();
        if (vcam != null)
        {
            bool isSinglePlayer = PhotonNetwork.CurrentRoom != null &&
                                  PhotonNetwork.CurrentRoom.PlayerCount == 1;

            if (isSinglePlayer)
                vcam.gameObject.SetActive(photonView.Owner.ActorNumber == 1);
            else
                vcam.gameObject.SetActive(photonView.IsMine);
        }
    }

    private void Update()
    {
        bool isSinglePlayer = PhotonNetwork.CurrentRoom != null &&
                              PhotonNetwork.CurrentRoom.PlayerCount == 1;

        // In single player: master client controls both
        // In multiplayer: only control your own character
        bool canControl = photonView.IsMine ||
                         (isSinglePlayer && PhotonNetwork.IsMasterClient);

        if (!canControl) return;

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