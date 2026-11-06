using System;

public static class ChatAreaEvents
{
    public static event Action<string> OnChatAreaChanged;
    public static event Action<string> OnChatAreaUnlocked;
    public static event Action<bool> OnChoicePanelStateChanged;

    public static void TriggerChatAreaChanged(string areaName)
    {
        OnChatAreaChanged?.Invoke(areaName);
    }

    public static void TriggerChatAreaUnlocked(string areaName)
    {
        OnChatAreaUnlocked?.Invoke(areaName);
    }

    public static void TriggerChoicePanelStateChanged(bool isOpen)
    {
        OnChoicePanelStateChanged?.Invoke(isOpen);
    }
}