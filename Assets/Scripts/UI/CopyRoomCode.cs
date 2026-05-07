using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class CopyRoomCode : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI roomCodeText;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (roomCodeText != null)
        {
            GUIUtility.systemCopyBuffer = roomCodeText.text;
            Debug.Log("[CopyRoomCode] Copied room code: " + roomCodeText.text);
        }
    }
}