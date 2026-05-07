using Photon.Pun;
using UnityEngine;

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

    // Separate keys per player
    private KeyCode leftKey;
    private KeyCode rightKey;
    private KeyCode jumpKey;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerScore = GetComponent<PlayerScore>();
    }

    private void Start()
    {
        if (!photonView.IsMine)
        {
            if (playerCamera != null)
                playerCamera.gameObject.SetActive(false);
            rb.bodyType = RigidbodyType2D.Kinematic;
            return;
        }

        if (playerCamera != null)
            playerCamera.gameObject.SetActive(true);

        // Actor 1 = Sunling = WASD
        // Actor 2 = Moonling = Arrow Keys
        if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
        {
            leftKey = KeyCode.A;
            rightKey = KeyCode.D;
            jumpKey = KeyCode.W;
        }
        else
        {
            leftKey = KeyCode.LeftArrow;
            rightKey = KeyCode.RightArrow;
            jumpKey = KeyCode.UpArrow;
        }
    }

    private void Update()
    {
        if (!photonView.IsMine || !alive) return;

        moveInput = 0f;
        if (Input.GetKey(leftKey)) moveInput = -1f;
        if (Input.GetKey(rightKey)) moveInput = 1f;

        if (Input.GetKeyDown(jumpKey) && isGrounded)
            jumpPressed = true;

        HandleFlip();
        HandleAnimations();
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine || !alive) return;

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