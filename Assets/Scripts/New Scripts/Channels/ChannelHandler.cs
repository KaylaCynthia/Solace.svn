using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using Ink.Runtime;
using Ink.UnityIntegration;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;

public class ChannelHandler : MonoBehaviour
{
    [Header("Channel Data")]
    public string channelName;
    public bool isActive;
    public bool isWaitingForResponse;

    [Header("Channel UI")]
    [SerializeField] private Button channelButton;
    [SerializeField] private TMP_Text channelText;
    Color originalTextColor;

    [Header("Channel Events")]
    [SerializeField] ChannelDialogPlayer channelDialogPlayer;
    public static Action OnChannelOpened;
    public static Action ContinueConversation;
    private Story currentStory;

    #region Temp Test
    [SerializeField] private InkFile testInkFile;

    void Start()
    {
        StartConversation(testInkFile);
    }
    #endregion

    void Awake()
    {
        channelButton.onClick.AddListener(ButtonClicked);
        originalTextColor = channelText.color;
    }
    void ButtonClicked()
    {
        Debug.Log("Channel button clicked: " + channelName);
        ChannelEventManager.OnChannelClicked?.Invoke(channelName);
    }
    void OnEnable()
    {
        ChannelEventManager.OnChannelClicked += HandleChannelClickedCallback;
        ChannelEventManager.OnDialogTriggered += HandleDialogTriggered;
    }

    void OnDisable()
    {
        ChannelEventManager.OnChannelClicked -= HandleChannelClickedCallback;
    }

    void HandleDialogTriggered(string channelName, InkFile inkFile)
    {
        if (channelName == this.channelName)
        {
            StartConversation(inkFile);
        }
    }
    
    void HandleChannelClickedCallback(string clickedChannelName)
    {
        if (clickedChannelName == channelName)
        {
            // Return the channel handler to the event manager so it can be set as the current channel handler
            ChannelEventManager.OnChannelClickedCallback?.Invoke(this);
        }
    }
    public void OnChannelOpenedHandler()
    {
        CheckWaitingForResponse();
        OnChannelOpened?.Invoke();
    }

    public void TriggerTryShowChoicesHandler(string channelName)
    {
        // Check if this channel is the one that was opened
        if(channelName != this.channelName)
        {
            return;
        }

        // If it is, trigger the TryShowChoices event
        TryShowChoices();
    }

    public void HandleChoiceSelected(int choiceIndex)
    {
        ChooseChoice(choiceIndex);
    }

    void StartConversation(InkFile conversationInk)
    {
        if(conversationInk == null)
        {
            Debug.LogError("Conversation Ink file is null for channel: " + channelName);
            return;
        }
        isActive = true;
        currentStory = new Story(conversationInk.storyJson);
        NextLine();
    }

    void NextLine(bool isInstant = false)
    {
        if (currentStory.canContinue)
        {
            string text = currentStory.Continue();
            if(string.IsNullOrWhiteSpace(text) && currentStory.currentTags.Count == 0)
            {
                Debug.Log("Empty text, continuing...");
                NextLine();
                return;
            }

            if(currentStory.currentTags.Count > 0)
            {
                if(currentStory.currentTags.Exists(tag => tag.StartsWith("CallConversation")))
                {
                    HandleCallConversationTag(currentStory.currentTags);
                }
            }
            
            if(!string.IsNullOrWhiteSpace(text))
            {
                float typingTime = isInstant ? 0f : GetTypingTime(text, currentStory.currentTags);

                DOVirtual.DelayedCall(typingTime, () => 
                {
                    channelDialogPlayer.CreateNewMessage(text, currentStory.currentTags);
                    NextLine();
                }, false);
            }
            else
            {
                Debug.Log("No text to display, continuing...");
                NextLine();
            }
        }
        else
        {
            Debug.Log("Cant continue");
            // Check if its choice or end
            if (currentStory.currentChoices.Count > 0)
            {
                CheckWaitingForResponse();
            }
            else
            {
                Debug.Log("End of conversation for channel: " + channelName);
                // End of conversation
                isActive = false;
                isWaitingForResponse = false;
                currentStory = null;
            }
        }
    }

    void HandleCallConversationTag(List<string> tags)
    {
        string callConversationTag = tags.Find(tag => tag.StartsWith("CallConversation"));
        Debug.Log("CallConversation tag found: " + callConversationTag);
        string channel = callConversationTag.Split(':')[1].Trim();
        InkFile inkFile = DataManager.Instance.GetInkFile(callConversationTag.Split(':')[2].Trim());
        ChannelEventManager.OnDialogTriggered?.Invoke(channel, inkFile);
    }

    void TryShowChoices()
    {
        if(currentStory == null)
        {
            Debug.LogError("No current story to show choices from.");
            return;
        }

        if (currentStory.currentChoices.Count > 0)
        {
            ChoiceHandler.TriggerShowChoices?.Invoke(currentStory.currentChoices);
        }
        else
        {
            Debug.Log("No choices available, ending conversation.");
            // End of conversation
        }
    }

    void ChooseChoice(int choiceIndex)
    {
        if(currentStory == null)
        {
            Debug.LogError("No current story to choose from.");
            return;
        }

        if(choiceIndex < 0 || choiceIndex >= currentStory.currentChoices.Count)
        {
            Debug.LogError("Invalid choice index: " + choiceIndex);
            return;
        }
        currentStory.ChooseChoiceIndex(choiceIndex);
        CheckWaitingForResponse();
        ChoiceHandler.TriggerHideChoices?.Invoke();
        NextLine(true);
    }

    public void CheckWaitingForResponse()
    {
        if(currentStory == null)
        {
            isWaitingForResponse = false;
            ChannelEventManager.OnChannelNotWaitingForResponse?.Invoke();
            return;
        }

        if (currentStory.currentChoices.Count > 0)
        {
            isWaitingForResponse = true;
            channelText.color = Color.red; // Change text color to red to indicate waiting for response
            ChannelEventManager.OnChannelWaitingForResponse?.Invoke(channelName);
        }
        else
        {
            isWaitingForResponse = false;
            channelText.color = originalTextColor; // Reset text color to original
            ChannelEventManager.OnChannelNotWaitingForResponse?.Invoke();
        }
    }

    private float typingTimePerCharacter = 0.1f; // Default typing time per character
    float GetTypingTime(string text, List<string> tags)
    {
        float typingTime = 2f; // Default typing time

        if(currentStory.currentTags.Exists(tag => tag.StartsWith("Speed")))
        {
            string speedTag = currentStory.currentTags.Find(tag => tag.StartsWith("Speed"));
            float speedValue;
            if(float.TryParse(speedTag.Split(':')[1].Trim(), out speedValue))
            {
                typingTime = speedValue;
            }
        }
        else
        {
            typingTime = typingTimePerCharacter * text.Length;
        }

        return typingTime;
    }
}
