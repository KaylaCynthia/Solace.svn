using UnityEngine;

public class ChatListManager : MonoBehaviour
{
    [SerializeField] private ChannelHandler dmFirstChannel;
    [SerializeField] private ChannelHandler serverFirstChannel;
    Transform chatListParent;

    void Awake()
    {
        AddChannelHandlersToList();
    }

    void AddChannelHandlersToList()
    {
        ChannelEventManager.RegisterChannelHandler(ChannelType.DMs, dmFirstChannel);
        ChannelEventManager.RegisterChannelHandler(ChannelType.Server, serverFirstChannel);

        chatListParent = dmFirstChannel.transform.parent;
    }

    void HandleNewChannelClicked(string channelName)
    {
        foreach (Transform child in chatListParent)
        {
            ChannelHandler channelHandler = child.GetComponent<ChannelHandler>();
            if (channelHandler != null)
            {
                CanvasGroup canvasGroup = child.GetComponent<CanvasGroup>();
                if (channelHandler.channelName == channelName)
                {
                    canvasGroup.alpha = 1f; // Make the channel fully visible
                    canvasGroup.blocksRaycasts = true; // Enable interaction
                    canvasGroup.interactable = true; // Enable interaction
                }
                else
                {
                    canvasGroup.alpha = 0f; // Hide other channels
                    canvasGroup.blocksRaycasts = false; // Disable interaction
                    canvasGroup.interactable = false; // Disable interaction
                }
            }
        }
    }

    public void OpenDMs()
    {
        Debug.Log("Opening DMs");
        ChannelEventManager.OnChannelTypeClicked?.Invoke(ChannelType.DMs);
    }

    public void OpenServer()
    {
        Debug.Log("Opening Server");
        ChannelEventManager.OnChannelTypeClicked?.Invoke(ChannelType.Server);
    }

    void OnEnable()
    {
        ChannelEventManager.OnNewChannelClicked += HandleNewChannelClicked;
    }

    void OnDisable()
    {
        ChannelEventManager.OnNewChannelClicked -= HandleNewChannelClicked;
    }


}
