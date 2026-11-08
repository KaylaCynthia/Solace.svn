using System;
using UnityEngine;

public class ChannelEventManager : MonoBehaviour
{
    static string currentChannelName = "Channel";
    
    // TODO: OnChannelClicked(string channelName/channelID)
    // Hide other channels by looking at the id and changing the CanvasGroup
    // Or we can just do the background thing, but still filter whether its a dm or server chat so
    // the latest visible channel is the most recently opened one.

    public static Action<string> OnChannelClicked; // channelName

    void HandleChannelClicked(string channelName)
    {
        Debug.Log("Channel clicked: " + channelName);
        currentChannelName = channelName;
    }

    public static Action<string, int> OnChoiceSelected; // channelName, choiceIndex

    public static void SelectChoice(int choiceIndex)
    {
        Debug.Log($"Choice selected: {choiceIndex}, calling OnChoiceSelected with channelName: {currentChannelName}");
        // TODO: Have a way to get the current channel name, replace "CurrentChannel" with the actual channel name.
        OnChoiceSelected?.Invoke(currentChannelName, choiceIndex);
    }

    void OnEnable()
    {
        OnChannelClicked += HandleChannelClicked;
    }

    void OnDisable()
    {
        OnChannelClicked -= HandleChannelClicked;
    }
}
