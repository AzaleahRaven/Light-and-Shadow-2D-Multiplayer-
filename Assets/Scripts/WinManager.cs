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
        // Hide win panel at start
        if (winPanel != null) winPanel.SetActive(false);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitToLobby);
    }

    // Called by Door.cs when player enters door
    public void OnPlayerEnteredDoor(Door.DoorType doorType)
    {
        if (gameEnded) return;

        if (doorType == Door.DoorType.Sunling)
            sunlingEntered = true;
        else
            moonlingEntered = true;

        Debug.Log($"[WinManager] Sunling: {sunlingEntered} | Moonling: {moonlingEntered}");

        // Sync with all players via RPC
        PhotonView pv = GetComponent<PhotonView>();
        if (pv != null)
            pv.RPC("SyncDoorRPC", RpcTarget.All, sunlingEntered, moonlingEntered);
        else
            CheckWin();
    }

    [PunRPC]
    private void SyncDoorRPC(bool sunling, bool moonling)
    {
        sunlingEntered = sunling;
        moonlingEntered = moonling;
        CheckWin();
    }

    private void CheckWin()
    {
        if (gameEnded) return;

        // ONLY show win when BOTH players entered their doors
        if (sunlingEntered && moonlingEntered)
        {
            gameEnded = true;

            if (winPanel != null) winPanel.SetActive(true);
            if (winText != null) winText.text = "COMPLETE!";

            Debug.Log("[WinManager] COMPLETE! Both players entered doors!");
        }
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
