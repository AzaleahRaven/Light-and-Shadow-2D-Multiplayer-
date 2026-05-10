using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using ExitGames.Client.Photon;
using TMPro;
using UnityEngine.UI;

public class RoomManagerPun : MonoBehaviourPunCallbacks
{
    [Header("Player Prefabs")]
    [SerializeField] private GameObject sunlingPrefab;
    [SerializeField] private GameObject moonlingPrefab;

    [Header("Spawn Points")]
    [SerializeField] private Transform sunlingSpawnPoint;
    [SerializeField] private Transform moonlingSpawnPoint;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI roomCodeText;
    [SerializeField] private TextMeshProUGUI readyText;
    [SerializeField] private Button masterButton;
    [SerializeField] private TextMeshProUGUI masterButtonText;

    [Header("Panels")]
    [SerializeField] private GameObject startupPanel;
    [SerializeField] private GameObject playerHUD;

    private bool hasSpawned = false;

    private void Awake()
    {
        if (PhotonNetwork.PrefabPool is CustomPrefabPool pool)
        {
            pool.RegisterPrefab("Sunling", sunlingPrefab);
            pool.RegisterPrefab("Moonling", moonlingPrefab);
        }

        if (startupPanel != null) startupPanel.SetActive(true);
        if (playerHUD != null) playerHUD.SetActive(false);
    }

    private void Start()
    {
        if (PhotonNetwork.InRoom && !hasSpawned)
        {
            SpawnPlayer();
            ShowRoomCode();
            SetupMasterButton();
            UpdateReadyText();
        }
    }

    public override void OnJoinedRoom()
    {
        if (!hasSpawned)
        {
            SpawnPlayer();
            ShowRoomCode();
            SetupMasterButton();
            UpdateReadyText();
        }

        if (startupPanel != null) startupPanel.SetActive(true);
        if (playerHUD != null) playerHUD.SetActive(false);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        UpdateReadyText();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdateReadyText();
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("gameStarted"))
            StartGame();
    }

    private void SpawnPlayer()
    {
        hasSpawned = true;

        // Always spawn Sunling for Actor 1 (host)
        GameObject sunling = PhotonNetwork.Instantiate(
            "Sunling",
            sunlingSpawnPoint.position,
            sunlingSpawnPoint.rotation
        );

        // Set Sunling to use WASD
        var sunlingInput = sunling.GetComponent<PlayerInputHandler>();
        if (sunlingInput != null)
            sunlingInput.controlScheme = PlayerInputHandler.ControlScheme.WASD;

        PhotonNetwork.LocalPlayer.NickName = "Player 1 - Sunling";

        // If single player (only 1 player in room), also spawn Moonling locally
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1 && PhotonNetwork.IsMasterClient)
        {
            SpawnMoonlingForSinglePlayer();
        }

        Debug.Log($"[RoomManager] Spawned Sunling. Room players: {PhotonNetwork.CurrentRoom.PlayerCount}");
    }

    private void SpawnMoonlingForSinglePlayer()
    {
        // Spawn Moonling controlled by same player
        GameObject moonling = PhotonNetwork.Instantiate(
            "Moonling",
            moonlingSpawnPoint.position,
            moonlingSpawnPoint.rotation
        );

        // Set Moonling to use Arrow Keys
        var moonlingInput = moonling.GetComponent<PlayerInputHandler>();
        if (moonlingInput != null)
            moonlingInput.controlScheme = PlayerInputHandler.ControlScheme.ArrowKeys;

        Debug.Log("[RoomManager] Single player mode: Spawned Moonling with Arrow Keys");
    }

    private void ShowRoomCode()
    {
        if (roomCodeText != null && PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("roomCode"))
        {
            string code = (string)PhotonNetwork.CurrentRoom.CustomProperties["roomCode"];
            roomCodeText.text = code;
        }
    }

    private void SetupMasterButton()
    {
        if (masterButtonText != null) masterButtonText.text = "Ready";
        if (masterButton != null)
        {
            masterButton.onClick.RemoveAllListeners();
            masterButton.onClick.AddListener(OnReadyClicked);
        }
    }

    private void OnReadyClicked()
    {
        bool currentReady = false;

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("isReady", out object val))
            currentReady = (bool)val;

        bool newReady = !currentReady;

        Hashtable props = new Hashtable { { "isReady", newReady } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        if (masterButtonText != null)
            masterButtonText.text = newReady ? "Unready" : "Ready";

        // Single player: start immediately when ready
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1 && newReady)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            Hashtable roomProps = new Hashtable { { "gameStarted", true } };
            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);
        }
    }

    private int GetReadyCount()
    {
        int count = 0;
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue("isReady", out object val) && (bool)val)
                count++;
        }
        return count;
    }

    private void UpdateReadyText()
    {
        if (readyText == null) return;

        int ready = GetReadyCount();
        int total = PhotonNetwork.CurrentRoom.PlayerCount;
        readyText.text = ready + "/" + total;

        // Start when all players ready (works for 1 or 2 players)
        if (ready >= total && total > 0 && PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            Hashtable roomProps = new Hashtable { { "gameStarted", true } };
            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);
        }
    }

    private void StartGame()
    {
        if (startupPanel != null) startupPanel.SetActive(false);
        if (playerHUD != null) playerHUD.SetActive(true);
    }
}