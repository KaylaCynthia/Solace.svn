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
            Invoke(nameof(NextLine), 10f);
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
            NextLine();
        }
    }

    void ChooseChoice(int choiceIndex)
    {
        // Act like its a dialog
        channelDialogPlayer.CreateNewMessage(currentStory.currentChoices[choiceIndex].text, currentStory.currentChoices[choiceIndex].tags);
        currentStory.ChooseChoiceIndex(choiceIndex);
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
