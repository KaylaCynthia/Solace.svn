using UnityEngine;

public class ServerLockManager : MonoBehaviour
{
    private bool isServerSwitchingLocked = true;

    private static ServerLockManager instance;
    public static ServerLockManager GetInstance() => instance;

    private void Awake()
    {
        if (instance != null)
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

    public void UnlockServerSwitching()
    {
        isServerSwitchingLocked = false;
        Debug.Log("Server switching unlocked");

        ServerEvents.TriggerServerSwitchingUnlocked();
    }

    public void LockServerSwitching()
    {
        isServerSwitchingLocked = true;
    }

    public bool IsServerSwitchingLocked() => isServerSwitchingLocked;

    public bool CanSwitchToServer(string serverType)
    {
        if (isServerSwitchingLocked)
        {
            return serverType == "DMs";
        }
        return true;
    }
}