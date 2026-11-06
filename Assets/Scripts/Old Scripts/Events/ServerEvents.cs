using System;

public static class ServerEvents
{
    public static event Action<string> OnServerChanged;
    public static event Action OnServerSwitchingUnlocked;
    public static event Action OnServerSwitchingLocked;

    public static void TriggerServerChanged(string serverType)
    {
        OnServerChanged?.Invoke(serverType);
    }

    public static void TriggerServerSwitchingUnlocked()
    {
        OnServerSwitchingUnlocked?.Invoke();
    }

    public static void TriggerServerSwitchingLocked()
    {
        OnServerSwitchingLocked?.Invoke();
    }
}