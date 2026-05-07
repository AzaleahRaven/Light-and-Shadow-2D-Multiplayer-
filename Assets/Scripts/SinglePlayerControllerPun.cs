using Photon.Pun;
using UnityEngine;

namespace ClearSky
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(PhotonView))]
    public class SimplePlayerControllerPun : MonoBehaviourPun
    {
        [Header("Movement")]
        public float movePower = 6f;
        public float jumpPower = 12f;

        [Header("Ground Check")]
        public Transform groundCheck;
        public float groundCheckRadius = 0.2f;
        public LayerMask groundLayer;

        [Header("Optional")]
        public Camera playerCamera;

        private Rigidbody2D rb;
        private Animator anim;

        private float horizontalInput;
        private int direction = 1;
        private bool jumpPressed = false;
        private bool alive = true;
        private bool isGrounded = false;

        public int Score { get; private set; }

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();

            if (!photonView.IsMine)
            {
                if (playerCamera != null)
                    playerCamera.gameObject.SetActive(false);
            }
            else
            {
                if (playerCamera != null)
                    playerCamera.gameObject.SetActive(true);
            }
        }

        private void Update()
        {
            if (!photonView.IsMine)
                return;

            Restart();

            if (!alive)
                return;

            horizontalInput = Input.GetAxisRaw("Horizontal");

            if ((Input.GetButtonDown("Jump") || Input.GetAxisRaw("Vertical") > 0) && isGrounded)
            {
                jumpPressed = true;
            }

            Attack();
            Hurt();
            Die();
            UpdateAnimationState();
            FlipCharacter();
        }

        private void FixedUpdate()
        {
            if (!photonView.IsMine)
                return;

            CheckGround();

            Run();
            Jump();
        }

        private void Run()
        {
            rb.linearVelocity = new Vector2(horizontalInput * movePower, rb.linearVelocity.y);
        }

        private void Jump()
        {
            if (!jumpPressed)
                return;

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);

            anim.SetBool("isJump", true);
            jumpPressed = false;
        }

        private void CheckGround()
        {
            if (groundCheck == null)
                return;

            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

            if (isGrounded && rb.linearVelocity.y <= 0.05f)
            {
                anim.SetBool("isJump", false);
            }
        }

        private void FlipCharacter()
        {
            if (horizontalInput < 0)
            {
                direction = -1;
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (horizontalInput > 0)
            {
                direction = 1;
                transform.localScale = new Vector3(1, 1, 1);
            }
        }

        private void UpdateAnimationState()
        {
            bool isRunning = Mathf.Abs(horizontalInput) > 0.1f && !anim.GetBool("isJump");
            anim.SetBool("isRun", isRunning);
        }

        private void Attack()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                anim.SetTrigger("attack");
            }
        }

        private void Hurt()
        {
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                anim.SetTrigger("hurt");

                if (direction == 1)
                    rb.AddForce(new Vector2(-5f, 1f), ForceMode2D.Impulse);
                else
                    rb.AddForce(new Vector2(5f, 1f), ForceMode2D.Impulse);
            }
        }

        private void Die()
        {
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                anim.SetTrigger("die");
                alive = false;
                rb.linearVelocity = Vector2.zero;
            }
        }

        private void Restart()
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                anim.SetTrigger("idle");
                anim.SetBool("isRun", false);
                anim.SetBool("isJump", false);
                alive = true;
            }
        }

        [PunRPC]
        public void AddScoreRPC(int amount)
        {
            Score += amount;
            Debug.Log($"{photonView.Owner.NickName} Score: {Score}");
        }


        private void OnDrawGizmosSelected()
        {
            if (groundCheck == null)
                return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}