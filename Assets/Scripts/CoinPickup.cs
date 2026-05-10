using UnityEngine;
using Photon.Pun;

// Rename this to DiamondPickup or keep as CoinPickup
// Attach to diamond/coin objects in scene
public class CoinPickup : MonoBehaviour
{
    public enum DiamondType { Sunling, Moonling, Both }

    [SerializeField] public DiamondType diamondType = DiamondType.Both;
    [SerializeField] private int scoreValue = 1;

    private bool collected = false;

    private void Start()
    {
        // Auto color based on type
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) return;

        switch (diamondType)
        {
            case DiamondType.Sunling:
                sr.color = new Color(1f, 0.85f, 0f); // Yellow
                break;
            case DiamondType.Moonling:
                sr.color = new Color(0.2f, 0.5f, 1f); // Blue
                break;
            case DiamondType.Both:
                sr.color = Color.white;
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collected) return;

        bool isSunling = collision.CompareTag("Sunling");
        bool isMoonling = collision.CompareTag("Moonling");

        bool canCollect = false;
        switch (diamondType)
        {
            case DiamondType.Sunling:
                canCollect = isSunling;
                break;
            case DiamondType.Moonling:
                canCollect = isMoonling;
                break;
            case DiamondType.Both:
                canCollect = isSunling || isMoonling;
                break;
        }

        if (!canCollect) return;

        PhotonView pv = collision.GetComponent<PhotonView>();
        if (pv == null || !pv.IsMine) return;

        collected = true;

        // Add score
        PlayerScore playerScore = collision.GetComponent<PlayerScore>();
        if (playerScore != null)
            playerScore.AddScore(scoreValue);

        // Update UI
        if (ScoreUIManager.Instance != null)
            ScoreUIManager.Instance.UpdateScoreUI();

        Debug.Log($"[CoinPickup] {collision.name} collected diamond!");

        // Destroy for all
        PhotonView diamondPV = GetComponent<PhotonView>();
        if (diamondPV != null)
            PhotonNetwork.Destroy(diamondPV);
        else
            Destroy(gameObject);
    }
}