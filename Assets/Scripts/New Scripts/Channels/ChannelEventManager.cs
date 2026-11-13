using System;
using UnityEngine;
using Ink.UnityIntegration;

public class ChannelEventManager : MonoBehaviour
{
    static ChannelHandler currentChannelHandler;
    public static ChannelHandler GetCurrentChannelHandler()
    {
        return currentChannelHandler;
    }
    public static Action<string> OnChannelClicked; // channelName
    public static Action<ChannelHandler> OnChannelClickedCallback; // channelHandler
    public static Action OnNewChannelClicked;

    void HandleChannelClicked(string channelName)
    {
        Debug.Log("Channel clicked: " + channelName);
    }

    void OnChannelClickedCallbackHandler(ChannelHandler channelHandler)
    {
        if(channelHandler == currentChannelHandler)
        {
            Debug.Log("Channel clicked is the same as the current channel handler: " + channelHandler.channelName);
            return;
        }
        currentChannelHandler = channelHandler;
        currentChannelHandler.OnChannelOpenedHandler();
        OnNewChannelClicked?.Invoke();
    }

    public static Action<string> TriggerDialogEvent; // inkName
    public static Action<string, InkFile> OnDialogTriggered; // channelName, inkFile
    public static Action TryShowChoices; // Call only

    void TryShowChoicesHandler()
    {
        // Message button calls try opening choices, call the currently open channel to see if they have choices to show
        if (currentChannelHandler == null)
        {
            Debug.LogError("No current channel handler set.");
            return;
        }
        // Call the current channel handler to try showing choices
        currentChannelHandler.TriggerTryShowChoicesHandler(currentChannelHandler.channelName);
    }

    public static Action<string> OnChannelWaitingForResponse; // Call only
    public static Action ChannelWaitingForResponse;
    public static Action OnChannelNotWaitingForResponse; // Call only

    void OnChannelWaitingForResponseHandler(string channelName)
    {
        if(currentChannelHandler == null)
        {
            Debug.LogError("No current channel handler set.");
            return;
        }

        // Check if the channel that is waiting for response is the current channel
        if(currentChannelHandler.channelName == channelName)
        {
            ChannelWaitingForResponse?.Invoke();
        }
    }

    public static Action<string, int> OnChoiceSelected; // channelName, choiceIndex

    public static void SelectChoice(int choiceIndex)
    {
        Debug.Log($"Choice selected: {choiceIndex}, calling OnChoiceSelected with channelName: {currentChannelHandler?.channelName}");
        currentChannelHandler?.HandleChoiceSelected(choiceIndex);
    }

    public static Action<string> OnNewMessageReceived; // channelName
    public static Action<string> UpdateChannelNotifications; // channelName
    public static Action UpdateBubbleNotification; // Call only

    void OnNewMessageReceivedHandler(string channelName)
    {
        // Debug.Log("New message received in channel: " + channelName);
        UpdateChannelNotifications?.Invoke(channelName);
        UpdateBubbleNotification?.Invoke();
    }

    void UpdateChannelNotificationsHandler(string channelName)
    {
        // Debug.Log("Updating channel notifications for channel: " + channelName);
        UpdateBubbleNotification?.Invoke();
    }

    void OnEnable()
    {
        OnChannelClicked += HandleChannelClicked;
        OnChannelClickedCallback += OnChannelClickedCallbackHandler;
        OnChannelWaitingForResponse += OnChannelWaitingForResponseHandler;
        TryShowChoices += TryShowChoicesHandler;

        OnNewMessageReceived += OnNewMessageReceivedHandler;
        UpdateChannelNotifications += UpdateChannelNotificationsHandler;
    }

    void OnDisable()
    {
        OnChannelClicked -= HandleChannelClicked;
        OnChannelClickedCallback -= OnChannelClickedCallbackHandler;
        OnChannelWaitingForResponse -= OnChannelWaitingForResponseHandler;
        TryShowChoices -= TryShowChoicesHandler;

        OnNewMessageReceived -= OnNewMessageReceivedHandler;
        UpdateChannelNotifications -= UpdateChannelNotificationsHandler;
    }
}
