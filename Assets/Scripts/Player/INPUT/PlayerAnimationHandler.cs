using Photon.Pun;
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
    private Animator anim;
    private PlayerInputHandler input;
    private PlayerController playerController;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        input = GetComponent<PlayerInputHandler>();
        playerController = GetComponent<PlayerController>();
    }

    private void Start()
    {
        PhotonView pv = GetComponent<PhotonView>();
        Debug.Log($"[PlayerAnimationHandler] on {gameObject.name} | IsMine: {(pv != null ? pv.IsMine.ToString() : "NO PHOTONVIEW")} | Animator: {(anim != null ? anim.gameObject.name : "NULL")}");
    }

    private void Update()
    {
        bool isWalking = Mathf.Abs(input.MoveInput) > 0.1f;
        anim.SetBool("isWalking", isWalking);
    }
}