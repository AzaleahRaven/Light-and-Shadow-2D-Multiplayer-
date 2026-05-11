using UnityEngine;
using Photon.Pun;
using TMPro;

public class Door : MonoBehaviour
{
    public enum DoorType { Sunling, Moonling }

    [Header("Door Settings")]
    [SerializeField] public DoorType doorType = DoorType.Sunling;
    [SerializeField] private int requiredDiamonds = 3;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer doorSprite;
    [SerializeField] private Color lockedColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    [SerializeField] private Color unlockedColor = Color.white;

    [Header("UI")]
    [SerializeField] private GameObject lockedIndicator; // "LOCKED" text above door
    [SerializeField] private GameObject unlockedIndicator; // "ENTER" text above door

    private bool isEntered = false;

    private void Start()
    {
        // Start as locked (grey)
        UpdateDoorVisual(false);
    }

    private void Update()
    {
        // Check if door should be unlocked
        bool unlocked = CheckUnlocked();
        UpdateDoorVisual(unlocked);
    }

    private bool CheckUnlocked()
    {
        // Get score from Photon properties
        string scoreKey = doorType == DoorType.Sunling ?
                         PlayerScore.SunlingScoreKey :
                         PlayerScore.MoonlingScoreKey;

        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue(scoreKey, out object val))
            {
                if ((int)val >= requiredDiamonds)
                    return true;
            }
        }
        return false;
    }

    private void UpdateDoorVisual(bool unlocked)
    {
        if (doorSprite == null) return;

        if (unlocked)
        {
            // Bright color = unlocked
            doorSprite.color = doorType == DoorType.Sunling ?
                new Color(1f, 0.85f, 0f) : // Yellow
                new Color(0.2f, 0.5f, 1f);  // Blue

            if (lockedIndicator != null) lockedIndicator.SetActive(false);
            if (unlockedIndicator != null) unlockedIndicator.SetActive(true);
        }
        else
        {
            // Grey = locked
            doorSprite.color = lockedColor;

            if (lockedIndicator != null) lockedIndicator.SetActive(true);
            if (unlockedIndicator != null) unlockedIndicator.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isEntered) return;

        // Check correct player
        bool isSunling = collision.CompareTag("Sunling");
        bool isMoonling = collision.CompareTag("Moonling");

        bool correctPlayer = (doorType == DoorType.Sunling && isSunling) ||
                            (doorType == DoorType.Moonling && isMoonling);

        if (!correctPlayer) return;

        // Check PhotonView ownership
        PhotonView pv = collision.GetComponent<PhotonView>();
        if (pv == null || !pv.IsMine) return;

        // Check if diamonds collected
        if (!CheckUnlocked())
        {
            Debug.Log($"[Door] {collision.name} tried to enter but needs {requiredDiamonds} diamonds!");
            return;
        }

        // Enter the door!
        isEntered = true;
        Debug.Log($"[Door] {collision.name} entered the door!");

        // Notify WinManager
        WinManager.Instance?.OnPlayerEnteredDoor(doorType);
    }
}
