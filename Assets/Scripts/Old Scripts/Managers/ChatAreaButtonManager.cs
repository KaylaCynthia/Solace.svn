using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChatAreaButtonManager : MonoBehaviour
{
    [System.Serializable]
    public class ChatAreaButton
    {
        public string chatAreaName;
        public Button button;
        public TextMeshProUGUI buttonText;
        public Transform buttonTransform;
        public bool isDMArea;
        public bool isActive = false;
    }

    [Header("Button Containers")]
    [SerializeField] private Transform dmsButtonsParent;
    [SerializeField] private Transform channelsButtonsParent;

    [Header("Button Lists")]
    [SerializeField] private List<ChatAreaButton> dmButtons = new List<ChatAreaButton>();
    [SerializeField] private List<ChatAreaButton> channelButtons = new List<ChatAreaButton>();

    private Dictionary<string, ChatAreaButton> buttonMap = new Dictionary<string, ChatAreaButton>();

    private ChatAreaManager chatAreaManager;
    private ServerManager serverManager;
    private ServerLockManager serverLockManager;

    private static ChatAreaButtonManager instance;
    public static ChatAreaButtonManager GetInstance()
    {
        if (instance == null)
        {
            instance = FindFirstObjectByType<ChatAreaButtonManager>();
        }
        return instance;
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    private void Start()
    {
        InitializeAllDMButtonsAsLocked();
    }

    public void Initialize(ChatAreaManager areaManager, ServerManager serverMgr)
    {
        this.chatAreaManager = areaManager;
        this.serverManager = serverMgr;
        this.serverLockManager = ServerLockManager.GetInstance();

        InitializeButtonMap();
        SubscribeToEvents();

        UpdateServerButtonVisibility();
    }

    private void InitializeAllDMButtonsAsLocked()
    {
        foreach (var buttonInfo in dmButtons)
        {
            if (buttonInfo.button != null && buttonInfo.isDMArea)
            {
                buttonInfo.button.gameObject.SetActive(false);
                buttonInfo.isActive = false;
            }
        }
    }

    private void InitializeButtonMap()
    {
        buttonMap.Clear();

        foreach (var buttonInfo in dmButtons)
        {
            buttonInfo.isDMArea = true;
            buttonMap[buttonInfo.chatAreaName] = buttonInfo;

            if (buttonInfo.button != null)
            {
                buttonInfo.button.onClick.AddListener(() =>
                    chatAreaManager.SwitchToChatAreaViaButton(buttonInfo.chatAreaName));
            }
        }

        foreach (var buttonInfo in channelButtons)
        {
            buttonInfo.isDMArea = false;
            buttonInfo.isActive = true;
            buttonMap[buttonInfo.chatAreaName] = buttonInfo;

            if (buttonInfo.button != null)
            {
                buttonInfo.button.onClick.AddListener(() =>
                    chatAreaManager.SwitchToChatAreaViaButton(buttonInfo.chatAreaName));
            }
        }
    }

    private void SubscribeToEvents()
    {
        ServerEvents.OnServerChanged += OnServerChanged;
        ServerEvents.OnServerSwitchingUnlocked += OnServerSwitchingUnlocked;
    }

    private void UnsubscribeFromEvents()
    {
        ServerEvents.OnServerChanged -= OnServerChanged;
        ServerEvents.OnServerSwitchingUnlocked -= OnServerSwitchingUnlocked;
    }

    public void UnlockDMArea(string chatAreaName)
    {
        if (buttonMap.ContainsKey(chatAreaName))
        {
            var buttonInfo = buttonMap[chatAreaName];
            if (buttonInfo.isDMArea && buttonInfo.button != null && !buttonInfo.isActive)
            {
                buttonInfo.button.gameObject.SetActive(true);
                buttonInfo.isActive = true;

                ChatNotificationEvents.TriggerNewMessage(chatAreaName);

                ReorderButtons();
            }
        }
    }

    public bool IsDMAreaUnlocked(string chatAreaName)
    {
        return buttonMap.ContainsKey(chatAreaName) &&
               buttonMap[chatAreaName].isDMArea &&
               buttonMap[chatAreaName].isActive;
    }

    private void OnServerChanged(string serverType)
    {
        UpdateServerButtonVisibility();
    }

    private void OnServerSwitchingUnlocked()
    {
        UpdateServerButtonVisibility();
    }

    private void UpdateServerButtonVisibility()
    {
        bool isDMsActive = serverManager?.IsDMsServerActive() ?? true;
        bool isServerSwitchingLocked = serverLockManager?.IsServerSwitchingLocked() ?? true;

        if (channelsButtonsParent != null)
        {
            channelsButtonsParent.gameObject.SetActive(!isDMsActive && !isServerSwitchingLocked);
        }

        if (dmsButtonsParent != null)
        {
            dmsButtonsParent.gameObject.SetActive(isDMsActive || isServerSwitchingLocked);
        }

        ReorderButtons();
    }

    private void ReorderButtons()
    {
        bool isDMsActive = serverManager?.IsDMsServerActive() ?? true;
        bool isServerSwitchingLocked = serverLockManager?.IsServerSwitchingLocked() ?? true;

        if (isServerSwitchingLocked)
        {
            isDMsActive = true;
        }

        Transform activeParent = isDMsActive ? dmsButtonsParent : channelsButtonsParent;
        List<ChatAreaButton> activeButtons = isDMsActive ? dmButtons : channelButtons;

        if (activeParent == null) return;

        List<ChatAreaButton> buttonsToShow = new List<ChatAreaButton>();
        foreach (var buttonInfo in activeButtons)
        {
            if (buttonInfo.isActive)
            {
                buttonsToShow.Add(buttonInfo);
            }
        }

        for (int i = 0; i < buttonsToShow.Count; i++)
        {
            if (buttonsToShow[i].buttonTransform != null)
            {
                buttonsToShow[i].buttonTransform.SetParent(activeParent, false);
                buttonsToShow[i].buttonTransform.SetSiblingIndex(i);
            }
        }
    }

    public void UpdateAllButtonStates()
    {
        UpdateServerButtonVisibility();
    }

    public void RegisterChatAreaButton(string chatAreaName, Button button, TextMeshProUGUI buttonText, Transform buttonTransform, bool isDMArea)
    {
        var newButton = new ChatAreaButton
        {
            chatAreaName = chatAreaName,
            button = button,
            buttonText = buttonText,
            buttonTransform = buttonTransform,
            isDMArea = isDMArea,
            isActive = !isDMArea
        };

        if (isDMArea)
        {
            dmButtons.Add(newButton);
            if (button != null) button.gameObject.SetActive(false);
        }
        else
        {
            channelButtons.Add(newButton);
        }

        buttonMap[chatAreaName] = newButton;
        ReorderButtons();
    }
}