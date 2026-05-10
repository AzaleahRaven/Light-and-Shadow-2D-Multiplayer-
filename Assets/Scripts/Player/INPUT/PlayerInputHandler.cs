using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviourPun
{
    private PlayerStats stats;

    public float MoveInput { get; private set; }
    public bool JumpPressed { get; private set; }

    public enum ControlScheme { WASD, ArrowKeys }

    [SerializeField] private ControlScheme controlScheme = ControlScheme.WASD;

    private bool isSinglePlayerMode = false;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
    }

    private void Start()
    {
        if (PhotonNetwork.CurrentRoom != null)
            isSinglePlayerMode = PhotonNetwork.CurrentRoom.PlayerCount == 1;

        Debug.Log($"[InputHandler] {gameObject.name} | IsMine: {photonView.IsMine} | Scheme: {controlScheme} | SinglePlayer: {isSinglePlayerMode}");
    }

    // Called by RoomManagerPun after spawning
    public void SetControlScheme(ControlScheme scheme)
    {
        controlScheme = scheme;
        Debug.Log($"[InputHandler] {gameObject.name} control scheme set to: {scheme}");
    }

    private void Update()
    {
        if (PhotonNetwork.CurrentRoom != null)
            isSinglePlayerMode = PhotonNetwork.CurrentRoom.PlayerCount == 1;

        bool canControl = false;

        if (isSinglePlayerMode && PhotonNetwork.IsMasterClient)
            canControl = true; // Single player controls all
        else
            canControl = photonView.IsMine; // Multiplayer: own character only

        if (!canControl) return;

        ReadInput();
    }

    private void ReadInput()
    {
        if (Keyboard.current == null) return;

        if (controlScheme == ControlScheme.WASD)
        {
            // Sunling - WASD
            float move = 0f;
            if (Keyboard.current.aKey.isPressed) move = -1f;
            if (Keyboard.current.dKey.isPressed) move = 1f;
            MoveInput = move;
            if (Keyboard.current.wKey.wasPressedThisFrame) JumpPressed = true;
        }
        else
        {
            // Moonling - Arrow Keys
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