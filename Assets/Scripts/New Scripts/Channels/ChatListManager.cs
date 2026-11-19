using UnityEngine;
using UnityEngine.UI;
using System;
using Ink.UnityIntegration;

public class ChatListManager : MonoBehaviour
{
    [SerializeField] private Button dmsButton;
    [SerializeField] private Button serverButton;
    [SerializeField] private CanvasGroup channelListCanvasGroup;
    [SerializeField] GameObject serverBubble;
    public static Action UnlockServer;
    public static Action<string> UnlockChannel;
    public static Action<ChannelHandler> UnlockChannelCallback;
    [SerializeField] private ChannelHandler dmFirstChannel;
    [SerializeField] private ChannelHandler serverFirstChannel;
    Transform chatListParent;

    void Awake()
    {
        AddChannelHandlersToList();
        dmsButton.onClick.AddListener(OpenDMs);
        serverButton.onClick.AddListener(OpenServer);
    }

    void AddChannelHandlersToList()
    {
        ChannelEventManager.RegisterChannelHandler(ChannelType.DMs, dmFirstChannel);
        ChannelEventManager.RegisterChannelHandler(ChannelType.Server, serverFirstChannel);

        chatListParent = dmFirstChannel.transform.parent;
    }

    void HandleNewChannelClicked(string channelName)
    {
        // Hide all channels except the one that was clicked
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

        UnlockServer += HandleUnlockServer;
        UnlockChannelCallback += HandleUnlockChannelCallback;

        ChannelEventManager.OnChannelTypeClicked += HandleChannelTypeClicked;
    }

    void OnDisable()
    {
        ChannelEventManager.OnNewChannelClicked -= HandleNewChannelClicked;

        UnlockServer -= HandleUnlockServer;
        UnlockChannelCallback -= HandleUnlockChannelCallback;

        ChannelEventManager.OnChannelTypeClicked -= HandleChannelTypeClicked;
    }

    void HandleUnlockServer()
    {
        Debug.Log("Unlocking Server");
        if (serverBubble != null)
        {
            serverBubble.SetActive(true);
        }
        InkFile inkFile = DataManager.Instance.GetInkFile("ServerVerification");
        ChannelEventManager.OnDialogTriggered?.Invoke("ServerVerification", inkFile);
    }

    public static void TriggerUnlockChannel(string channelName)
    {
        Debug.Log("Triggering UnlockChannel for: " + channelName);
        UnlockChannel?.Invoke(channelName);
    }

    void HandleUnlockChannelCallback(ChannelHandler channelHandler)
    {
        Button channelButton = channelHandler.GetChannelButton();
        if (channelButton != null)
        {
            channelButton.gameObject.SetActive(true);
            channelButton.transform.parent.gameObject.SetActive(true);
        }
    }

    void HandleChannelTypeClicked(ChannelType channelType)
    {
        // Hide or show server channel list based on the channel type clicked
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
}
