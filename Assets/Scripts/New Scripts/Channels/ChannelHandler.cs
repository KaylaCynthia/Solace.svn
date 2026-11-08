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
        ChannelEventManager.OnChannelClicked += OnChannelOpenedHandler;
        ChannelEventManager.OnChoiceSelected += HandleChoiceSelected;
    }

    void OnDisable()
    {
        ChannelEventManager.OnChoiceSelected -= HandleChoiceSelected;
        ChannelEventManager.OnChannelClicked -= OnChannelOpenedHandler;
    }

    void OnChannelOpenedHandler(string channelName)
    {
        // Check if this channel is the one that was opened
        if(channelName != this.channelName)
        {
            return;
        }

        // If it is, trigger the OnChannelOpened event
        OnChannelOpened?.Invoke();
    }

    void HandleChoiceSelected(string channelName, int choiceIndex)
    {
        if (channelName == this.channelName)
        {
            ChooseChoice(choiceIndex);
        }
    }

    void OnChannelOpenedHandler()
    {
        // Check if this chanel has an active conversation
        if (currentStory != null && currentStory.canContinue)
        {
            NextLine();
        }
    }

    void StartConversation(InkFile conversationInk)
    {
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
            Debug.Log("Continuing conversation");
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
                Debug.Log("Choices available");
                // Debug.Log("Curr Text: " + currentStory.currentText);
                // Debug.Log("Tags: " + string.Join(", ", currentStory.currentTags));
                foreach (Choice choice in currentStory.currentChoices)
                {
                    Debug.Log("Choice: " + choice.text);
                    Debug.Log("Choice tags: " + string.Join(", ", choice.tags?.ToArray() ?? new string[0]));
                }
                ChoiceHandler.TriggerShowChoices?.Invoke(currentStory.currentChoices);
            }
            else
            {
                // End of conversation
                
            }
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
        if(choiceIndex < 0 || choiceIndex >= currentStory.currentChoices.Count)
        {
            Debug.LogError("Invalid choice index: " + choiceIndex);
            return;
        }
        currentStory.ChooseChoiceIndex(choiceIndex);
        ChoiceHandler.TriggerHideChoices?.Invoke();
        NextLine();
    }

    IEnumerator WaitForResponse()
    {
        isWaitingForResponse = true;
        while (isWaitingForResponse)
        {
            yield return null;
        }
    }
}
