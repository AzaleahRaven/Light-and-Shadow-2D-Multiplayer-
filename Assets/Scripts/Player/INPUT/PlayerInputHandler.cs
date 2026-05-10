using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PhotonView photonView;
    private PlayerStats stats;

    public float MoveInput { get; private set; }
    public bool JumpPressed { get; private set; }

    // Which control scheme to use
    public enum ControlScheme { WASD, ArrowKeys }
    [HideInInspector] public ControlScheme controlScheme;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        stats = GetComponent<PlayerStats>();
    }

    private void Update()
    {
        // In multiplayer: only control if this is your own character
        // In single player: host controls BOTH characters
        bool isSinglePlayer = PhotonNetwork.CurrentRoom != null &&
                              PhotonNetwork.CurrentRoom.PlayerCount == 1;

        bool canControl = photonView.IsMine ||
                         (isSinglePlayer && PhotonNetwork.IsMasterClient);

        if (!canControl) return;

        ReadInput();
    }

    private void ReadInput()
    {
        if (controlScheme == ControlScheme.WASD)
        {
            // Sunling - WASD using New Input System
            float move = 0f;
            if (Keyboard.current.aKey.isPressed) move = -1f;
            if (Keyboard.current.dKey.isPressed) move = 1f;
            MoveInput = move;

            if (Keyboard.current.wKey.wasPressedThisFrame) JumpPressed = true;
        }
        else
        {
            // Moonling - Arrow Keys using New Input System
            float move = 0f;
            if (Keyboard.current.leftArrowKey.isPressed) move = -1f;
            if (Keyboard.current.rightArrowKey.isPressed) move = 1f;
            MoveInput = move;

            if (Keyboard.current.upArrowKey.wasPressedThisFrame) JumpPressed = true;
        }
    }

    public void ResetJump() => JumpPressed = false;

    private void OnDrawGizmosSelected()
    {
        if (stats != null && stats.groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(stats.groundCheck.position,
                          stats.groundCheck.position + Vector3.down * stats.groundCheckRadius);
        }
    }
}