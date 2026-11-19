using UnityEngine;
using UnityEngine.UI;

public class ServerInviteScript : MonoBehaviour
{
    [SerializeField] private Button acceptButton;

    void OnEnable()
    {
        if (acceptButton != null)
        {
            acceptButton.onClick.AddListener(OnAcceptButtonClicked);
        }
        else
        {
            Debug.LogError("Accept button is not assigned in the inspector.");
        }
    }

    void OnAcceptButtonClicked()
    {
        // Trigger the event to unlock the server
        ChatListManager.UnlockServer?.Invoke();
        ChannelEventManager.OnChannelTypeClicked?.Invoke(ChannelType.Server);
    }
}
