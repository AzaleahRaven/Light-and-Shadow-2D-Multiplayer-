using Photon.Pun;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private PhotonView photonView;

    public int maxHealth;
    public int currentHealth;

    public int maxStamina;
    public int currentStamina;

    public int speed;
    public int jumpForce;

    [Header("Ground Check Settings")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();

    }

    private void Start()
    {
        if (!photonView.IsMine) return;

        currentHealth = maxHealth;
        currentStamina = maxStamina;
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

        if (currentHealth < 0)
        {
            currentHealth = 0;
            // TODO: Death logic here
        }
    }

}
