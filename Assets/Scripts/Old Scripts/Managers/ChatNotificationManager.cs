using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChatNotificationManager : MonoBehaviour
{
    [System.Serializable]
    public class ChatAreaNotification
    {
        public string chatAreaName;
        public string serverType;
        public TextMeshProUGUI buttonText;
        public bool hasNewMessages;
    }

    [SerializeField] private List<ChatAreaNotification> chatAreaNotifications = new List<ChatAreaNotification>();

    private Dictionary<string, ChatAreaNotification> notificationMap = new Dictionary<string, ChatAreaNotification>();
    private Color normalColor = new Color(1f, 0.882353f, 0.7294118f, 1);
    private Color notificationColor = Color.red;
    // private Image serverButtonNotif;
    private ServerManager serverManager;

    private static ChatNotificationManager instance;
    public static ChatNotificationManager GetInstance() => instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        InitializeNotificationMap();

        ChatNotificationEvents.OnNewMessageInInactiveArea += OnNewMessage;
        ChatNotificationEvents.OnChatAreaViewed += OnChatAreaSwitched;
        ServerEvents.OnServerChanged += OnServerChanged;

        serverManager = ServerManager.GetInstance();

        // serverButtonNotif = GameObject.Find("DMsBubble").transform.GetChild(0).GetComponent<Image>();
        // serverButtonNotif.color = new Color(1f, 0f, 0f, 0f);
        // serverButtonNotif = GameObject.Find("ServerBubble").transform.GetChild(0).GetComponent<Image>();
        // serverButtonNotif.color = new Color(1f, 0f, 0f, 0f);
    }

    private void OnDestroy()
    {
        ChatNotificationEvents.OnNewMessageInInactiveArea -= OnNewMessage;
        ChatNotificationEvents.OnChatAreaViewed -= OnChatAreaSwitched;
        ServerEvents.OnServerChanged -= OnServerChanged;

        if (instance == this)
        {
            instance = null;
        }
    }

    private void InitializeNotificationMap()
    {
        notificationMap.Clear();
        foreach (var notification in chatAreaNotifications)
        {
            notificationMap[notification.chatAreaName] = notification;
            SetTextColor(notification.chatAreaName, normalColor);
        }
    }

    private void OnNewMessage(string chatAreaName)
    {
        //Debug.Log("New message!");
        if (notificationMap.ContainsKey(chatAreaName))
        {
            notificationMap[chatAreaName].hasNewMessages = true;

            // if (notificationMap[chatAreaName].serverType == "DMs" && serverManager.GetCurrentServerType() != "DMs")
            // {
            //     //Debug.Log("Setting DMs bubble color to red");
            //     serverButtonNotif = GameObject.Find("DMsBubble").transform.GetChild(0).GetComponent<Image>();
            //     serverButtonNotif.color = new Color(1f, 0f, 0f, 1f);
            // }
            // else if (notificationMap[chatAreaName].serverType == "Channels" && serverManager.GetCurrentServerType() != "Channels")
            // {
            //     //Debug.Log("Setting Server bubble color to red");
            //     serverButtonNotif = GameObject.Find("ServerBubble").transform.GetChild(0).GetComponent<Image>();
            //     serverButtonNotif.color = new Color(1f, 0f, 0f, 1f);
            // }

            if (IsChatAreaInActiveServer(chatAreaName))
            {
                SetTextColor(chatAreaName, notificationColor);
            }
        }
    }

    private void OnChatAreaSwitched(string chatAreaName)
    {
        if (notificationMap.ContainsKey(chatAreaName))
        {
            notificationMap[chatAreaName].hasNewMessages = false;
            SetTextColor(chatAreaName, normalColor);
        }
    }

    private void OnServerChanged(string serverType)
    {
        UpdateAllTextColors();
    }

    private void UpdateAllTextColors()
    {
        foreach (var notification in chatAreaNotifications)
        {
            Color targetColor = notification.hasNewMessages && IsChatAreaInActiveServer(notification.chatAreaName)
                ? notificationColor
                : normalColor;

            SetTextColor(notification.chatAreaName, targetColor);
        }
    }

    private void SetTextColor(string chatAreaName, Color color)
    {
        if (notificationMap.ContainsKey(chatAreaName))
        {
            TextMeshProUGUI text = notificationMap[chatAreaName].buttonText;
            if (text != null)
            {
                text.color = color;
            }
        }
    }

    private bool IsChatAreaInActiveServer(string chatAreaName)
    {
        string[] dmAreas = { "ChatAreaSunny", "ChatAreaRael" };

        if (serverManager == null) return true;

        if (serverManager.IsDMsServerActive())
        {
            foreach (string dmArea in dmAreas)
            {
                if (chatAreaName == dmArea) return true;
            }
            return false;
        }
        else
        {
            foreach (string dmArea in dmAreas)
            {
                if (chatAreaName == dmArea) return false;
            }
            return true;
        }
    }

    public bool HasNewMessages(string chatAreaName)
    {
        return notificationMap.ContainsKey(chatAreaName) && notificationMap[chatAreaName].hasNewMessages;
    }
}