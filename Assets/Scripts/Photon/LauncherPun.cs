using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LauncherPun : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI joinStatusText;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private TMP_InputField joinCodeInput;
    [SerializeField] private GameObject joinPanel;

    private string currentRoomCode;
    private bool isConnecting;
    private bool isHosting;

    private void Awake()
    {
        PhotonNetwork.PrefabPool = new CustomPrefabPool();
    }

    private void Start()
    {
        joinPanel.SetActive(false);

        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = "1.0";

        if (hostButton != null) hostButton.onClick.AddListener(HostRoom);
        if (joinButton != null) joinButton.onClick.AddListener(() => JoinRoom(joinCodeInput.text));

        SetButtonsInteractable(true);
        UpdateStatus("Ready");
        UpdateJoinStatus("");
    }

    public void HostRoom()
    {
        SetButtonsInteractable(false);

        if (!PhotonNetwork.IsConnected)
        {
            isConnecting = true;
            isHosting = true;
            UpdateStatus("Connecting...");
            PhotonNetwork.ConnectUsingSettings();
            return;
        }

        CreateRoom();
    }

    private void CreateRoom()
    {
        currentRoomCode = GenerateRoomCode(6);

        RoomOptions options = new RoomOptions { MaxPlayers = 2, IsOpen = true, IsVisible = true };

        ExitGames.Client.Photon.Hashtable roomProps = new ExitGames.Client.Photon.Hashtable();
        roomProps["roomCode"] = currentRoomCode;
        options.CustomRoomProperties = roomProps;
        options.CustomRoomPropertiesForLobby = new string[] { "roomCode" };

        PhotonNetwork.CreateRoom(currentRoomCode, options);
        UpdateStatus("Creating room with code: " + currentRoomCode);
    }

    private string GenerateRoomCode(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        char[] code = new char[length];
        for (int i = 0; i < length; i++)
            code[i] = chars[Random.Range(0, chars.Length)];
        return new string(code);
    }

    public void JoinRoom(string roomCode)
    {
        if (!PhotonNetwork.IsConnected)
        {
            if (string.IsNullOrEmpty(roomCode))
            {
                UpdateStatus("Enter a valid room code.");
                return;
            }

            SetButtonsInteractable(false);
            isConnecting = true;
            isHosting = false;
            UpdateJoinStatus("Connecting to Server");
            PhotonNetwork.ConnectUsingSettings();
            return;
        }

        if (string.IsNullOrEmpty(roomCode))
        {
            UpdateStatus("Enter a valid room code.");
            return;
        }

        SetButtonsInteractable(false);
        UpdateJoinStatus("Connecting to Server");
        PhotonNetwork.JoinRoom(roomCode);
    }

    public override void OnConnectedToMaster()
    {
        UpdateStatus("Connected to Master");

        if (isConnecting && isHosting)
        {
            CreateRoom();
        }
        else if (isConnecting && !isHosting)
        {
            UpdateJoinStatus("Server Found, Joining Room");
            string roomCode = joinCodeInput != null ? joinCodeInput.text : "";
            PhotonNetwork.JoinRoom(roomCode);
        }
    }

    public override void OnJoinedRoom()
    {
        UpdateStatus("Joined room: " + PhotonNetwork.CurrentRoom.Name);
        UpdateJoinStatus("");
        // DO NOT load scene here - RoomManagerPun handles it via ready system
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        UpdateJoinStatus("Incorrect Code");
        SetButtonsInteractable(true);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        UpdateStatus("Room creation failed: " + message);
        SetButtonsInteractable(true);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        UpdateStatus("Disconnected: " + cause);
        UpdateJoinStatus("");
        SetButtonsInteractable(true);
    }

    private void SetButtonsInteractable(bool state)
    {
        if (hostButton != null) hostButton.interactable = state;
        if (joinButton != null) joinButton.interactable = state;
    }

    private void UpdateStatus(string message)
    {
        Debug.Log(message);
        if (statusText != null)
            statusText.text = message;
    }

    private void UpdateJoinStatus(string message)
    {
        if (joinStatusText != null)
            joinStatusText.text = message;
    }
}