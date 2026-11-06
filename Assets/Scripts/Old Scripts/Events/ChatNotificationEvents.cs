using System;
using UnityEngine;

public static class ChatNotificationEvents
{
    public static event Action<string> OnNewMessageInInactiveArea;
    public static event Action<string> OnChatAreaViewed;
    public static event Action SetAreaBubbleNotification;

    public static void TriggerNewMessage(string chatAreaName)
    {
        Debug.Log($"Triggering new message event for chat area: {chatAreaName}");
        OnNewMessageInInactiveArea?.Invoke(chatAreaName);
    }

    public static void TriggerChatAreaViewed(string chatAreaName)
    {
        OnChatAreaViewed?.Invoke(chatAreaName);
    }

    public static void TriggerUpdateBubbleNotification()
    {
        SetAreaBubbleNotification?.Invoke();
    }
}