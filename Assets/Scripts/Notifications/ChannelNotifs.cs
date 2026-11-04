using UnityEngine;

public class ChannelNotifs : MonoBehaviour
{
    // PLaced in each channel, if a new message is sent in that channel, the bool will be set to true. BubbleNotif will have a list of channels
    // and will check if any of them have new messages, if so, it will set the bubble to active.
    public bool hasNewMessage;

    void OnEnable()
    {
        ChatNotificationEvents.OnNewMessageInInactiveArea += OnNewMessageInInactiveArea;
        ChatNotificationEvents.OnChatAreaViewed += OnChatAreaViewed;
    }

    void OnDisable()
    {
        ChatNotificationEvents.OnNewMessageInInactiveArea -= OnNewMessageInInactiveArea;
        ChatNotificationEvents.OnChatAreaViewed -= OnChatAreaViewed;
    }

    void OnNewMessageInInactiveArea(string chatAreaName)
    {
        if (chatAreaName == gameObject.name)
        {
            hasNewMessage = true;
            ChatNotificationEvents.TriggerUpdateBubbleNotification();
        }
    }

    void OnChatAreaViewed(string chatAreaName)
    {
        if (chatAreaName == gameObject.name)
        {
            hasNewMessage = false;
            ChatNotificationEvents.TriggerUpdateBubbleNotification();
        }
    }
}
