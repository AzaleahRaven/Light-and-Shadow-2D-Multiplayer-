using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PlayerScore))]
public class PlayerControllerPun : MonoBehaviourPun
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float jumpForce = 12f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Optional")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    private Animator anim;
    private PlayerScore playerScore;

    private float moveInput;
    private bool jumpPressed;
    private bool isGrounded;
    private bool alive = true;

    // Control type set on prefab
    public enum ControlScheme { WASD, ArrowKeys }
    [SerializeField] public ControlScheme controlScheme = ControlScheme.WASD;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerScore = GetComponent<PlayerScore>();
    }

    private void Start()
    {
        bool isSinglePlayer = PhotonNetwork.CurrentRoom != null &&
                              PhotonNetwork.CurrentRoom.PlayerCount == 1;

        if (!isSinglePlayer && !photonView.IsMine)
        {
            // Multiplayer: disable camera and physics for remote players
            if (playerCamera != null)
                playerCamera.gameObject.SetActive(false);
            rb.bodyType = RigidbodyType2D.Kinematic;
            return;
        }

        if (playerCamera != null)
            playerCamera.gameObject.SetActive(photonView.IsMine);

        Debug.Log($"[PlayerControllerPun] {gameObject.name} | Scheme: {controlScheme}");
    }

    private void Update()
    {
        if (!alive) return;
        if (Keyboard.current == null) return;

        bool isSinglePlayer = PhotonNetwork.CurrentRoom != null &&
                              PhotonNetwork.CurrentRoom.PlayerCount == 1;

        bool canControl = isSinglePlayer ?
                         PhotonNetwork.IsMasterClient :
                         photonView.IsMine;

        if (!canControl) return;

        // Read input based on control scheme
        moveInput = 0f;

        if (controlScheme == ControlScheme.WASD)
        {
            if (Keyboard.current.aKey.isPressed) moveInput = -1f;
            if (Keyboard.current.dKey.isPressed) moveInput = 1f;
            if (Keyboard.current.wKey.wasPressedThisFrame && isGrounded)
                jumpPressed = true;
        }
        else // ArrowKeys
        {
            if (Keyboard.current.leftArrowKey.isPressed) moveInput = -1f;
            if (Keyboard.current.rightArrowKey.isPressed) moveInput = 1f;
            if (Keyboard.current.upArrowKey.wasPressedThisFrame && isGrounded)
                jumpPressed = true;
        }

        HandleFlip();
        HandleAnimations();
    }

    private void FixedUpdate()
    {
        if (!alive) return;

        bool isSinglePlayer = PhotonNetwork.CurrentRoom != null &&
                              PhotonNetwork.CurrentRoom.PlayerCount == 1;

        bool canControl = isSinglePlayer ?
                         PhotonNetwork.IsMasterClient :
                         photonView.IsMine;

        if (!canControl) return;

        CheckGround();
        Run();
        Jump();
    }

    private void Run()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    private void Jump()
    {
        if (!jumpPressed) return;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        anim.SetBool("isJump", true);
        jumpPressed = false;
    }

    private void CheckGround()
    {
        if (groundCheck == null) return;
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded && rb.linearVelocity.y <= 0.05f)
            anim.SetBool("isJump", false);
    }

    private void HandleFlip()
    {
        if (spriteRenderer == null) return;
        if (moveInput > 0) spriteRenderer.flipX = false;
        else if (moveInput < 0) spriteRenderer.flipX = true;
    }

    private void HandleAnimations()
    {
        bool isRunning = Mathf.Abs(moveInput) > 0.1f && !anim.GetBool("isJump");
        anim.SetBool("isRun", isRunning);
    }

    [PunRPC]
    public void AddScoreRPC(int amount)
    {
        playerScore.AddScore(amount);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}