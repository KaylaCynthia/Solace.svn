using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
using Ink.Runtime;
using Ink.UnityIntegration;

public class ChannelHandler : MonoBehaviour
{
    [Header("Channel Data")]
    public string channelName;
    public bool isActive;
    public bool isWaitingForResponse;

    [Header("Channel UI")]
    [SerializeField] private Button channelButton;

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
    }
    void ButtonClicked()
    {
        Debug.Log("Channel button clicked: " + channelName);
        ChannelEventManager.OnChannelClicked?.Invoke(channelName);
    }
    void OnEnable()
    {
        ChannelEventManager.OnChannelClicked += HandleChannelClickedCallback;

    }

    void OnDisable()
    {
        ChannelEventManager.OnChannelClicked -= HandleChannelClickedCallback;
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

    void NextLine()
    {
        if (currentStory.canContinue)
        {
            // Check for portrait and speaker tags
            string text = currentStory.Continue();
            if(string.IsNullOrWhiteSpace(text))
            {
                Debug.Log("Empty text, continuing...");
                NextLine();
                return;
            }
            // Debug.Log("Continuing conversation");
            // Debug.Log("Line: " + text);
            // Debug.Log("Tags: " + string.Join(", ", currentStory.currentTags));
            channelDialogPlayer.CreateNewMessage(text, currentStory.currentTags);
            // NotificationEvent.TriggerNewMessage(channelName);
            // Process the text and update the UI
            Invoke(nameof(NextLine), 2f);
        }
        else
        {
            Debug.Log("Cant continue");
            // Check if its choice or end
            if (currentStory.currentChoices.Count > 0)
            {
                isWaitingForResponse = true;
                ChannelEventManager.OnChannelWaitingForResponse?.Invoke(channelName);
            }
            else
            {
                // End of conversation
                isActive = false;
                isWaitingForResponse = false;
                currentStory = null;
            }
        }
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

    [ContextMenu("ChooseFirstChoice")]
    void ChooseFirstChoice()
    {
        if (currentStory.currentChoices.Count > 0)
        {
            ChooseChoice(0);
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
        ChoiceHandler.TriggerHideChoices?.Invoke();
        ChannelEventManager.OnChannelNotWaitingForResponse?.Invoke();
        NextLine();
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
            ChannelEventManager.OnChannelWaitingForResponse?.Invoke(channelName);
        }
        else
        {
            isWaitingForResponse = false;
            ChannelEventManager.OnChannelNotWaitingForResponse?.Invoke();
        }
    }
}
