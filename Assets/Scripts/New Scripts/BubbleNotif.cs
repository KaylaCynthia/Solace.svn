using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BubbleNotif : MonoBehaviour
{
    [SerializeField] private GameObject bubbleImage;
    [SerializeField] private List<ChannelNotifs> channelNotifs;
    
    void Awake()
    {
        if(bubbleImage == null)
        {
            Debug.LogError("Bubble image is not assigned in BubbleNotif script.");
            bubbleImage = transform.GetChild(0).gameObject; // Attempt to assign the first child as the bubble image
        }
    }
    void OnEnable()
    {
        ChannelEventManager.UpdateBubbleNotification += UpdateBubbleNotification;
    }

    void OnDisable()
    {
        ChannelEventManager.UpdateBubbleNotification -= UpdateBubbleNotification;
    }

    void UpdateBubbleNotification()
    {
        bool hasNewMessages = false;
        foreach (var channel in channelNotifs)
        {
            if (channel.hasNewMessage)
            {
                hasNewMessages = true;
                break;
            }
        }

        bubbleImage.SetActive(hasNewMessages);
    }
}
