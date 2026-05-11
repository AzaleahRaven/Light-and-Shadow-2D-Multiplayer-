using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class LoseManager : MonoBehaviour
{
    public static LoseManager Instance;

    [Header("Lose UI")]
    [SerializeField] private GameObject losePanel;
    [SerializeField] private TextMeshProUGUI loseText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (losePanel != null)
            losePanel.SetActive(false);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitToLobby);
    }

    public void OnPlayerDied(string playerName)
    {
        if (losePanel != null)
            losePanel.SetActive(true);

        if (loseText != null)
            loseText.text = playerName + " died!\nGame Over!";
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
