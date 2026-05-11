using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class WinManager : MonoBehaviourPunCallbacks
{
    public static WinManager Instance;

    [Header("Win UI")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private TextMeshProUGUI winText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;

    private bool sunlingEntered = false;
    private bool moonlingEntered = false;
    private bool gameEnded = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (winPanel != null)
            winPanel.SetActive(false);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitToLobby);
    }

    // Called by Door.cs when player enters
    public void OnPlayerEnteredDoor(Door.DoorType doorType)
    {
        if (gameEnded) return;

        if (doorType == Door.DoorType.Sunling)
        {
            sunlingEntered = true;
            Debug.Log("[WinManager] Sunling entered door!");
        }
        else
        {
            moonlingEntered = true;
            Debug.Log("[WinManager] Moonling entered door!");
        }

        // Sync with all players via RPC
        PhotonView pv = GetComponent<PhotonView>();
        if (pv != null)
            pv.RPC("SyncDoorEnteredRPC", RpcTarget.All,
                   sunlingEntered, moonlingEntered);
        else
            CheckWinCondition();
    }

    [PunRPC]
    private void SyncDoorEnteredRPC(bool sunling, bool moonling)
    {
        sunlingEntered = sunling;
        moonlingEntered = moonling;
        CheckWinCondition();
    }

    private void CheckWinCondition()
    {
        if (gameEnded) return;

        if (sunlingEntered && moonlingEntered)
        {
            // Both entered = FULL WIN!
            gameEnded = true;
            ShowWin("🎉 BOTH PLAYERS WIN! 🎉\nLevel Complete!");
        }
        else if (sunlingEntered)
        {
            ShowWin("Sunling reached the door!\nWaiting for Moonling...");
        }
        else if (moonlingEntered)
        {
            ShowWin("Moonling reached the door!\nWaiting for Sunling...");
        }
    }

    private void ShowWin(string message)
    {
        if (winPanel != null)
            winPanel.SetActive(true);

        if (winText != null)
            winText.text = message;

        Debug.Log("[WinManager] " + message);
    }

    private void RestartGame()
    {
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel("GamePlay");
    }

    private void QuitToLobby()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("Lobby");
    }
}
