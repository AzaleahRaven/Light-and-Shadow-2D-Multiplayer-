using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PhotonView photonView;
    private PlayerStats stats;
    private PlayerInput playerInput;

    public float MoveInput { get; private set; }
    public bool JumpPressed { get; private set; }

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        stats = GetComponent<PlayerStats>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        if (playerInput != null && !photonView.IsMine) playerInput.enabled = false;

        Debug.Log($"[PlayerInputHandler] on {gameObject.name} | IsMine: {photonView.IsMine} | PlayerInput enabled: {(playerInput != null ? playerInput.enabled.ToString() : "NULL")}");
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!photonView.IsMine) return;
        MoveInput = context.ReadValue<Vector2>().x;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!photonView.IsMine) return;
        if (context.performed) JumpPressed = true;
    }

    public void ResetJump() => JumpPressed = false;

    private void OnDrawGizmosSelected()
    {
        if (stats != null && stats.groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(stats.groundCheck.position, stats.groundCheck.position + Vector3.down * stats.groundCheckRadius);
        }
    }
}