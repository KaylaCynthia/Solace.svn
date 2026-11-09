using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MessageInput : MonoBehaviour
{
    [SerializeField] private Button messageButton;
    [SerializeField] private TMP_Text messageInputField;
    Color originalColor;
    bool isChoicesOpen = false;

    private void Awake()
    {
        messageButton.onClick.AddListener(OnMessageButtonClicked);
        originalColor = messageInputField.color;
    }

    void OnMessageButtonClicked()
    {
        if(isChoicesOpen)
        {
            ChoiceHandler.TriggerHideChoices?.Invoke();
            return;
        }
        else
        {
            ChannelEventManager.TryShowChoices?.Invoke();
        }
    }

    void MakeTextRed()
    {
        Debug.Log("Channel is waiting for response. Making text red.");
        messageInputField.color = Color.red;
    }
    
    void ResetTextColor()
    {
        messageInputField.color = originalColor;
    }

    void HandleChoicesOpened(List<Ink.Runtime.Choice> choices)
    {
        isChoicesOpen = true;
        // Debug.Log("Choices opened. isChoicesOpen set to true.");
    }

    void HandleChoicesClosed()
    {
        isChoicesOpen = false;
        // Debug.Log("Choices closed. isChoicesOpen set to false.");
    }

    void OnEnable()
    {
        ChannelEventManager.ChannelWaitingForResponse += MakeTextRed;
        ChannelEventManager.OnChannelNotWaitingForResponse += ResetTextColor;
        ChoiceHandler.TriggerShowChoices += HandleChoicesOpened;
        ChoiceHandler.TriggerHideChoices += HandleChoicesClosed;
    }

    void OnDisable()
    {
        ChannelEventManager.ChannelWaitingForResponse -= MakeTextRed;
        ChannelEventManager.OnChannelNotWaitingForResponse -= ResetTextColor;
        ChoiceHandler.TriggerShowChoices -= HandleChoicesOpened;
        ChoiceHandler.TriggerHideChoices -= HandleChoicesClosed;
    }
}
