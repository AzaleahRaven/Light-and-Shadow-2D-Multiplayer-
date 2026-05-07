using UnityEngine;

public class PlayerMovement
{
    private readonly Rigidbody2D rb;
    private readonly Animator anim;
    private readonly Transform transform;
    private readonly PlayerStats stats;
    private readonly PlayerInputHandler input;

    private bool isGrounded;
    private bool hasJumped;

    public bool IsGrounded => isGrounded;

    public PlayerMovement(Rigidbody2D rb, Animator anim, Transform transform, PlayerStats stats, PlayerInputHandler input)
    {
        this.rb = rb;
        this.anim = anim;
        this.transform = transform;
        this.stats = stats;
        this.input = input;
    }

    public void HandleMovement()
    {
        CheckGround();
        Run();
        Jump();
        UpdateAnimation();
        UpdateFlip();
    }

    private void Run()
    {
        rb.linearVelocity = new Vector2(input.MoveInput * stats.speed, rb.linearVelocity.y);
    }

    private void Jump()
    {
        if (input.JumpPressed && isGrounded && !hasJumped)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, stats.jumpForce);
            anim.SetTrigger("isJumping");
            hasJumped = true;
            Debug.Log("[Jump] Jump executed once — isJumping trigger set");
        }

        input.ResetJump();
    }

    private void CheckGround()
    {
        if (stats.groundCheck == null) return;

        RaycastHit2D hit = Physics2D.Raycast( stats.groundCheck.position, Vector2.down, stats.groundCheckRadius, stats.groundLayer);

        bool wasGrounded = isGrounded;
        isGrounded = hit.collider != null;

        if (isGrounded && !wasGrounded)
        {
            hasJumped = false;
            Debug.Log("[Ground] Player landed — hasJumped reset");
        }

        anim.SetBool("isGrounded", isGrounded);
    }

    private void UpdateAnimation()
    {
        anim.SetBool("isWalking", Mathf.Abs(input.MoveInput) > 0.1f);
    }

    private void UpdateFlip()
    {
        if (input.MoveInput < 0) transform.localScale = new Vector3(-1, 1, 1);
        else if (input.MoveInput > 0) transform.localScale = new Vector3(1, 1, 1);
    }

    public void DrawGroundCheckGizmo()
    {
        if (stats != null && stats.groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(stats.groundCheck.position, Vector3.down * stats.groundCheckRadius);
        }
    }
}