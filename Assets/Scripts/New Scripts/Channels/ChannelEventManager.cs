using System;
using UnityEngine;
using Ink.UnityIntegration;

public class ChannelEventManager : MonoBehaviour
{
    static ChannelHandler currentChannelHandler;
    
    // TODO: OnChannelClicked(string channelName/channelID)
    // Hide other channels by looking at the id and changing the CanvasGroup
    // Or we can just do the background thing, but still filter whether its a dm or server chat so
    // the latest visible channel is the most recently opened one.

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
        // TODO: Have a way to get the current channel name, replace "CurrentChannel" with the actual channel name.
        // OnChoiceSelected?.Invoke(currentChannelHandler?.channelName ?? "Unknown", choiceIndex);
        currentChannelHandler?.HandleChoiceSelected(choiceIndex);
    }

    void OnEnable()
    {
        OnChannelClicked += HandleChannelClicked;
        OnChannelClickedCallback += OnChannelClickedCallbackHandler;
        OnChannelWaitingForResponse += OnChannelWaitingForResponseHandler;
        TryShowChoices += TryShowChoicesHandler;
    }

    void OnDisable()
    {
        OnChannelClicked -= HandleChannelClicked;
        OnChannelClickedCallback -= OnChannelClickedCallbackHandler;
        OnChannelWaitingForResponse -= OnChannelWaitingForResponseHandler;
        TryShowChoices -= TryShowChoicesHandler;
    }
}
