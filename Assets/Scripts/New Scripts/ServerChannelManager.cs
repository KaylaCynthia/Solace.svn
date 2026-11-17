using UnityEngine;
using UnityEngine.UI;
using System;

public class ServerChannelManager : MonoBehaviour
{
    [SerializeField] GameObject channelListParent;
    CanvasGroup channelListCanvasGroup;
    [SerializeField] GameObject serverBubble;
    public static Action UnlockServer;
    public static Action<string> UnlockChannel;
    public static Action<ChannelHandler> UnlockChannelCallback;

    void HandleUnlockServer()
    {
        Debug.Log("Unlocking Server");
        if (serverBubble != null)
        {
            serverBubble.SetActive(true);
        }
    }

    public static void TriggerUnlockChannel(string channelName)
    {
        UnlockChannel?.Invoke(channelName);
    }

    void HandleUnlockChannelCallback(ChannelHandler channelHandler)
    {
        Button channelButton = channelHandler.GetChannelButton();
        if (channelButton != null)
        {
            channelButton.gameObject.SetActive(true);
        }
    }

    void HandleChannelTypeClicked(ChannelType channelType)
    {
        if(channelType == ChannelType.Server)
        {
            channelListCanvasGroup.alpha = 1f;
            channelListCanvasGroup.interactable = true;
            channelListCanvasGroup.blocksRaycasts = true;
        }
        else
        {
            channelListCanvasGroup.alpha = 0f;
            channelListCanvasGroup.interactable = false;
            channelListCanvasGroup.blocksRaycasts = false;
        }
    }

    void Awake()
    {
        channelListCanvasGroup = channelListParent.GetComponent<CanvasGroup>();
        if (channelListCanvasGroup == null)
        {
            Debug.LogError("CanvasGroup component not found on channelListParent.");
        }
    }
    void OnEnable()
    {
        UnlockServer += HandleUnlockServer;
        UnlockChannelCallback += HandleUnlockChannelCallback;

        ChannelEventManager.OnChannelTypeClicked += HandleChannelTypeClicked;
    }

    void OnDisable()
    {
        UnlockServer -= HandleUnlockServer;
        UnlockChannelCallback -= HandleUnlockChannelCallback;

        ChannelEventManager.OnChannelTypeClicked -= HandleChannelTypeClicked;
    }
}
