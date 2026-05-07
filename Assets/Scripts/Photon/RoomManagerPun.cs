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
        if (PhotonNetwork.InRoom)
        {
            SpawnPlayer();
            ShowRoomCode();
            SetupMasterButton();
            UpdateReadyText();
        }
    }

    public override void OnJoinedRoom()
    {
        SpawnPlayer();
        ShowRoomCode();
        SetupMasterButton();
        UpdateReadyText();

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
        string prefabId;
        Transform spawnPoint;
        string nickname;

        if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
        {
            prefabId = "Sunling";
            spawnPoint = sunlingSpawnPoint;
            nickname = "Player 1 - Sunling";
        }
        else
        {
            prefabId = "Moonling";
            spawnPoint = moonlingSpawnPoint;
            nickname = "Player 2 - Moonling";
        }

        PhotonNetwork.LocalPlayer.NickName = nickname;
        PhotonNetwork.Instantiate(prefabId, spawnPoint.position, spawnPoint.rotation);
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
        masterButtonText.text = "Ready";
        masterButton.onClick.RemoveAllListeners();
        masterButton.onClick.AddListener(OnReadyClicked);
    }

    private void OnReadyClicked()
    {
        bool currentReady = false;

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("isReady", out object val))
            currentReady = (bool)val;

        bool newReady = !currentReady;

        Hashtable props = new Hashtable { { "isReady", newReady } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

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
        readyText.text = ready + "/2";

        if (ready >= 2 && PhotonNetwork.IsMasterClient)
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