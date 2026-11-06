using UnityEngine;

[System.Serializable]
public class DialogueState
{
    public string CurrentSpeaker { get; private set; } = "Unknown";
    public string CurrentPortraitTag { get; private set; } = "default";
    public bool IsWaitingForInput { get; private set; }
    public bool IsPlayerSpeaking => CurrentSpeaker.Equals("player", System.StringComparison.OrdinalIgnoreCase);
    public bool IsPlayerContinuingMessage { get; private set; } = false;
    public GameObject CurrentPlayerMessageUI { get; set; } = null;

    public void Reset()
    {
        CurrentSpeaker = "Unknown";
        CurrentPortraitTag = "default";
        IsWaitingForInput = false;
        IsPlayerContinuingMessage = false;
        CurrentPlayerMessageUI = null;
    }

    public void SetSpeaker(string speaker) => CurrentSpeaker = speaker;
    public void SetPortraitTag(string portraitTag) => CurrentPortraitTag = portraitTag;
    public void SetWaitingForInput(bool waiting) => IsWaitingForInput = waiting;
    public void SetPlayerContinuingMessage(bool continuing) => IsPlayerContinuingMessage = continuing;
}