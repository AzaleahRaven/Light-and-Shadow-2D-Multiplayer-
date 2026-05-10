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
        bool isSinglePlayer = PhotonNetwork.CurrentRoom.PlayerCount == 1;

        if (isSinglePlayer && PhotonNetwork.IsMasterClient)
        {
            // Single player: spawn BOTH characters
            // Spawn Sunling with WASD
            GameObject sunling = PhotonNetwork.Instantiate(
                "Sunling",
                sunlingSpawnPoint.position,
                sunlingSpawnPoint.rotation
            );
            var sunlingInput = sunling.GetComponent<PlayerInputHandler>();
            if (sunlingInput != null)
                sunlingInput.SetControlScheme(PlayerInputHandler.ControlScheme.WASD);

            // Spawn Moonling with Arrow Keys
            GameObject moonling = PhotonNetwork.Instantiate(
                "Moonling",
                moonlingSpawnPoint.position,
                moonlingSpawnPoint.rotation
            );
            var moonlingInput = moonling.GetComponent<PlayerInputHandler>();
            if (moonlingInput != null)
                moonlingInput.SetControlScheme(PlayerInputHandler.ControlScheme.ArrowKeys);

            PhotonNetwork.LocalPlayer.NickName = "Player 1";
            Debug.Log("[RoomManager] Single player: Spawned both Sunling (WASD) and Moonling (Arrows)");
        }
        else
        {
            // Multiplayer: each player spawns their own character
            if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
            {
                GameObject sunling = PhotonNetwork.Instantiate(
                    "Sunling",
                    sunlingSpawnPoint.position,
                    sunlingSpawnPoint.rotation
                );
                var sunlingInput = sunling.GetComponent<PlayerInputHandler>();
                if (sunlingInput != null)
                    sunlingInput.SetControlScheme(PlayerInputHandler.ControlScheme.WASD);

                PhotonNetwork.LocalPlayer.NickName = "Player 1 - Sunling";
            }
            else
            {
                GameObject moonling = PhotonNetwork.Instantiate(
                    "Moonling",
                    moonlingSpawnPoint.position,
                    moonlingSpawnPoint.rotation
                );
                var moonlingInput = moonling.GetComponent<PlayerInputHandler>();
                if (moonlingInput != null)
                    moonlingInput.SetControlScheme(PlayerInputHandler.ControlScheme.ArrowKeys);

                PhotonNetwork.LocalPlayer.NickName = "Player 2 - Moonling";
            }
        }
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

        // Start when all players ready
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