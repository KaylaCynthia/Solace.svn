using UnityEngine;

[RequireComponent(typeof(ChannelHandler))]
public class ChannelNotifs : MonoBehaviour
{
    // PLaced in each channel, if a new message is sent in that channel, the bool will be set to true. BubbleNotif will have a list of channels
    // and will check if any of them have new messages, if so, it will set the bubble to active.
    public bool hasNewMessage;
    ChannelHandler channelHandler;

    void Awake()
    {
        channelHandler = GetComponent<ChannelHandler>();
    }

    void OnEnable()
    {
        ChannelEventManager.OnNewMessageReceived += OnNewMessageReceived;
        ChannelEventManager.OnChannelClickedCallback += OnChatAreaViewed;
    }

    void OnDisable()
    {
        ChannelEventManager.OnNewMessageReceived -= OnNewMessageReceived;
        ChannelEventManager.UpdateBubbleNotification -= UpdateBubbleNotification;
        ChannelEventManager.OnChannelClickedCallback -= OnChatAreaViewed;
    }

    void OnNewMessageReceived(string channelName)
    {
        channelHandler.CheckWaitingForResponse();
        
        if(channelHandler.isWaitingForResponse)
        {
            // If the channel is waiting for a response, always show the notification
            hasNewMessage = true;
            return;
        }

        if(channelName == channelHandler.channelName)
        {
            if(ChannelEventManager.GetCurrentChannelHandler() == channelHandler)
            {
                // If the current channel is the one that received a new message, we don't want to show a notification
                hasNewMessage = false;
            }
            else
            {
                // If the channel that received a new message is not the current channel, we want to show a notification
                hasNewMessage = true;
            }
        }
    }

    void UpdateBubbleNotification()
    {
        // Force check if the channel is waiting for a response, if so, we want to show the notification
        if(channelHandler.isWaitingForResponse)
        {
            hasNewMessage = true;
        }
    }

    void OnChatAreaViewed(ChannelHandler channelHandler)
    {
        if (channelHandler.channelName == this.channelHandler.channelName)
        {
            if (!channelHandler.isWaitingForResponse)
            {
                // If channel is not active, we can remove the notification
                hasNewMessage = false;
                ChannelEventManager.UpdateBubbleNotification?.Invoke();
            }
        }
    }
}
