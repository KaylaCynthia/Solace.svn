using UnityEngine;

public class ServerManager : MonoBehaviour
{
    [Header("Server References")]
    [SerializeField] private GameObject dmsServer;
    [SerializeField] private GameObject channelsServer;

    [Header("Default Server")]
    [SerializeField] private bool startWithDMs = true;

    private string currentServerType = "DMs";
    private ServerLockManager serverLockManager;

    private static ServerManager instance;
    public static ServerManager GetInstance() => instance;

    public System.Action<string> OnServerChanged;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        serverLockManager = ServerLockManager.GetInstance();
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
        InitializeServers();
    }

    private void InitializeServers()
    {
        if (startWithDMs)
        {
            ShowDMsServer();
        }
        else
        {
            if (serverLockManager == null || !serverLockManager.IsServerSwitchingLocked())
            {
                ShowChannelsServer();
            }
            else
            {
                ShowDMsServer();
            }
        }
    }

    public void ShowDMsServer()
    {
        if (dmsServer != null) dmsServer.SetActive(true);
        if (channelsServer != null) channelsServer.SetActive(false);
        currentServerType = "DMs";

        ServerEvents.TriggerServerChanged(currentServerType);
        OnServerChanged?.Invoke(currentServerType);
    }

    public void ShowChannelsServer()
    {
        if (serverLockManager != null && serverLockManager.IsServerSwitchingLocked())
        {
            Debug.LogWarning("Server switching is currently locked. Cannot switch to channels.");
            return;
        }

        if (dmsServer != null) dmsServer.SetActive(false);
        if (channelsServer != null) channelsServer.SetActive(true);
        currentServerType = "Channels";

        ServerEvents.TriggerServerChanged(currentServerType);
        OnServerChanged?.Invoke(currentServerType);
    }

    public void ToggleServers()
    {
        if (currentServerType == "DMs" && !serverLockManager.IsServerSwitchingLocked())
        {
            AudioCollection.GetInstance().PlaySFX(AudioCollection.GetInstance().buttonClick);
            ShowChannelsServer();
        }
        else
        {
            AudioCollection.GetInstance().PlaySFX(AudioCollection.GetInstance().buttonClick);
            ShowDMsServer();
        }
    }

    public string GetCurrentServerType() => currentServerType;
    public bool IsDMsServerActive() => currentServerType == "DMs";
    public bool IsChannelsServerActive() => currentServerType == "Channels";
}