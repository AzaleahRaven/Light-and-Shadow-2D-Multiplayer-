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
            return;
        }

        if (playerCamera != null)
            playerCamera.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (!photonView.IsMine || !alive)
            return;

        moveInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") && isGrounded)
            jumpPressed = true;

        HandleFlip();
        HandleAnimations();
        HandleActions();
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine || !alive)
            return;

        CheckGround();
        Run();
        Jump();
    }

    private void Run()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y); // UPDATE
    }

    private void Jump()
    {
        if (!jumpPressed) return;

        rb.linearVelocity = new Vector2(rb.linearVelocity .x, 0f); // UPDATE
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        anim.SetBool("isJump", true);
        jumpPressed = false;
    }

    private void CheckGround()
    {
        if (groundCheck == null) return;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded && rb.linearVelocity .y <= 0.05f)
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

    private void HandleActions()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) anim.SetTrigger("attack");
        if (Input.GetKeyDown(KeyCode.Alpha2)) anim.SetTrigger("hurt");
        if (Input.GetKeyDown(KeyCode.Alpha3)) { anim.SetTrigger("die"); alive = false; rb.linearVelocity  = Vector2.zero; }
        if (Input.GetKeyDown(KeyCode.Alpha0)) { anim.SetTrigger("idle"); alive = true; }
    }

    [PunRPC] //NEW
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
