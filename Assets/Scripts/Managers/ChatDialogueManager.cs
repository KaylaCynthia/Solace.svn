using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChatDialogueManager : MonoBehaviour
{
    [Header("Prefab Reference")]
    [SerializeField] private GameObject chatMessagePrefab;

    [Header("UI References")]
    [SerializeField] private ChatUI chatUI;
    [SerializeField] private ChoiceHandler choiceHandler;

    [Header("Dialogue Parameters")]
    [SerializeField] private float lettersPerSecond;
    [SerializeField] private InkFileManager inkFileManager;

    private Story currentStory;
    public bool DialogueIsPlaying { get; private set; }

    private DialogueState currentState;
    private DialogueParser dialogueParser;
    private Dictionary<string, GameObject> lastMessageFromUser;
    private string currentConversationChatArea = "ChatAreaSunny";
    private GameObject currentPlayerMessageUI = null;
    private string lastSpeakerInCurrentArea = null;

    private static ChatDialogueManager instance;
    public static ChatDialogueManager GetInstance() => instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one ChatDialogueManager");
            Destroy(gameObject);
            return;
        }
        instance = this;

        if (chatMessagePrefab != null)
        {
            ChatMessage.SetMessagePrefab(chatMessagePrefab);
        }
        else
        {
            Debug.LogError("ChatMessagePrefab is not assigned in inspector!");
        }

        dialogueParser = new DialogueParser();
        currentState = new DialogueState();
        lastMessageFromUser = new Dictionary<string, GameObject>();

        if (chatUI != null)
        {
            chatUI.Initialize(this);
        }
        else
        {
            Debug.LogError("ChatUI reference is not set in ChatDialogueManager!");
        }

        if (choiceHandler != null)
        {
            choiceHandler.Initialize(MakeChoice, chatUI);
        }
        else
        {
            Debug.LogError("ChoiceHandler reference is null in ChatDialogueManager!");
        }

        lastMessageFromUser.Clear();
        InitializeButtonSystem();
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }

        if (DialogueIsPlaying)
        {
            StopAllCoroutines();
            DialogueIsPlaying = false;
        }

        lastMessageFromUser?.Clear();
        currentPlayerMessageUI = null;
        lastSpeakerInCurrentArea = null;
        currentState?.Reset();

        chatUI?.Cleanup();
    }

    private void InitializeButtonSystem()
    {
        ChatAreaButtonManager buttonManager = ChatAreaButtonManager.GetInstance();
        ServerManager serverManager = ServerManager.GetInstance();

        ChatAreaManager chatAreaManager = chatUI?.GetChatAreaManager();
        //Debug.Log($"ChatAreaManager: {chatAreaManager}");

        if (buttonManager != null && chatAreaManager != null && serverManager != null)
        {
            buttonManager.Initialize(chatAreaManager, serverManager);
        }
    }

    public void StartConversation(string conversationKey, string chatAreaName)
    {
        if (inkFileManager == null)
        {
            Debug.LogError("InkFileManager is not assigned!");
            return;
        }

        currentConversationChatArea = chatAreaName;

        ChatAreaButtonManager buttonManager = ChatAreaButtonManager.GetInstance();
        if (buttonManager != null)
        {
            buttonManager.UnlockDMArea(chatAreaName);
        }

        inkFileManager.StartConversation(conversationKey);
        TextAsset inkFile = inkFileManager.GetCurrentInkFile();

        if (inkFile != null)
        {
            //Debug.Log($"Starting conversation: {conversationKey} in area: {chatAreaName}");
            EnterDialogueMode(inkFile);
        }
    }

    public void EnterDialogueMode(TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);
        DialogueIsPlaying = true;
        chatUI.Show();
        ContinueStory();
    }

    private void ContinueStory()
    {
        if (!currentStory.canContinue)
        {
            Debug.Log("End of Conversation reached.");
            StartCoroutine(HandleEndOfConversation());
            return;
        }

        string nextLine = currentStory.Continue();
        dialogueParser.HandleTags(currentStory.currentTags, currentState);

        bool isPlayerMessage = currentState.IsPlayerSpeaking;
        StartCoroutine(DisplayMessage(nextLine, isPlayerMessage, currentState.CurrentSpeaker));
    }

    private IEnumerator DisplayMessage(string message, bool isPlayerMessage , string senderName = null)
    {
        currentState.SetWaitingForInput(false);

        string processedMessage = ProcessMessageWithPlayerName(message);
        string currentSpeaker = currentState.CurrentSpeaker;
        string currentChatArea = chatUI.GetCurrentChatAreaName();

        if (isPlayerMessage && string.IsNullOrEmpty(message.Trim()) && currentStory.currentChoices.Count > 0)
        {
            // Debug.Log($"Skipping empty player choice line");

            currentState.SetWaitingForInput(true);

            if (currentStory.currentChoices.Count > 0)
            {
                yield return new WaitForSeconds(0.5f);
                ShowChoicesIfInCorrectArea();
            }

            yield break;
        }

        if (isPlayerMessage && currentPlayerMessageUI != null)
        {
            bool shouldAppendToPlayer = lastSpeakerInCurrentArea == "You" && lastMessageFromUser.ContainsKey("You");

            if (shouldAppendToPlayer)
            {
                float delay = processedMessage.Length / lettersPerSecond;
                yield return new WaitForSeconds(delay);

                // Debug.Log($"Appending to existing player message: {processedMessage}");
                yield return StartCoroutine(AppendToPlayerMessage(processedMessage));

                if (currentStory.canContinue && currentStory.currentChoices.Count == 0)
                {
                    yield return new WaitForSeconds(1f);
                    ContinueStory();
                }
                else if (currentStory.currentChoices.Count > 0)
                {
                    yield return new WaitForSeconds(0.5f);
                    ShowChoicesIfInCorrectArea();
                }

                yield break;
            }
        }

        if (!isPlayerMessage)
        {
            if (currentChatArea == currentConversationChatArea)
            {
                yield return StartCoroutine(chatUI.ShowTypingForDuration(processedMessage.Length / lettersPerSecond, currentConversationChatArea, senderName));
            }
            else
            {
                yield return new WaitForSeconds(processedMessage.Length / lettersPerSecond);
            }
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }

        GameObject messageUI;

        bool shouldAppend = currentSpeaker == lastSpeakerInCurrentArea && lastMessageFromUser.ContainsKey(currentSpeaker);

        // Debug.Log($"Appending Check - Speaker: {currentSpeaker}, LastSpeaker: {lastSpeakerInCurrentArea}, HasLastMessage: {lastMessageFromUser.ContainsKey(currentSpeaker)}, ShouldAppend: {shouldAppend}");

        if (shouldAppend)
        {
            // Debug.Log($"Appending to last message from {currentSpeaker}: {processedMessage}");
            messageUI = lastMessageFromUser[currentSpeaker];
            chatUI.AppendToMessage(messageUI, processedMessage);
        }
        else
        {
            // Debug.Log($"Creating new message from {currentSpeaker}: {processedMessage} in {currentConversationChatArea}");
            Sprite portraitSprite = GetPortraitSprite(currentState.CurrentPortraitTag);
            ChatMessage chatMessage = currentState.IsPlayerSpeaking ? new PlayerMessage(processedMessage, portraitSprite) : new NPCMessage(currentState.CurrentSpeaker, processedMessage, portraitSprite);

            // Debug.Log($"Before AddMessageToChat - IsPlayer: {currentState.IsPlayerSpeaking}, CurrentChatArea: {currentChatArea}, IntendedArea: {currentConversationChatArea}");

            messageUI = chatUI.AddMessageToChat(chatMessage, currentConversationChatArea);

            if (messageUI != null)
            {
                if (!isPlayerMessage)
                {
                    lastMessageFromUser[currentSpeaker] = messageUI;
                    lastSpeakerInCurrentArea = currentSpeaker;
                }
                else
                {
                    currentPlayerMessageUI = messageUI;
                    lastMessageFromUser["You"] = messageUI;
                    lastSpeakerInCurrentArea = "You";
                }
            }
        }

        if (!currentStory.canContinue && currentStory.currentChoices.Count == 0)
        {
            Debug.Log("No more content and no choices - ending conversation.");
            ContinueStory();
            yield break;
        }

        if (currentChatArea == currentConversationChatArea || isPlayerMessage)
        {
            chatUI.ScrollToBottom();
        }

        currentState.SetWaitingForInput(true);

        if (currentStory.currentChoices.Count > 0)
        {
            yield return new WaitForSeconds(0.5f);
            ShowChoicesIfInCorrectArea();
        }
        else
        {
            choiceHandler.Hide();

            if (currentStory.canContinue)
            {
                yield return new WaitForSeconds(1f);
                ContinueStory();
            }
        }
    }

    private string ProcessMessageWithPlayerName(string message)
    {
        if (string.IsNullOrEmpty(message))
            return message;

        PlayerDataManager playerDataManager = PlayerDataManager.GetInstance();
        if (playerDataManager == null)
        {
            Debug.LogWarning("PlayerDataManager not found - cannot replace Player_Name placeholder");
            return message;
        }

        string playerName = playerDataManager.PlayerData.PlayerName;
        string processedMessage = message.Replace("Player_Name", playerName);

        if (processedMessage != message)
        {
            Debug.Log($"Replaced Player_Name with '{playerName}' in message: {message} -> {processedMessage}");
        }

        return processedMessage;
    }

    private Sprite GetPortraitSprite(string portraitTag)
    {
        if (string.IsNullOrEmpty(portraitTag))
        {
            Debug.LogWarning("Portrait tag is null or empty");
            return null;
        }

        PortraitManager portraitManager = PortraitManager.GetInstance();
        if (portraitManager == null)
        {
            Debug.LogError("PortraitManager instance not found!");
            return null;
        }

        return portraitManager.GetPortrait(portraitTag);
    }

    private IEnumerator AppendToPlayerMessage(string additionalText)
    {
        if (currentPlayerMessageUI != null)
        {
            Debug.Log($"Appending to player message UI: {additionalText}");
            chatUI.AppendToMessage(currentPlayerMessageUI, "\n" + additionalText);
        }
        else
        {
            Debug.Log("No current player message UI to append to.");
        }
        yield return null;
    }

    private void ShowChoicesIfInCorrectArea()
    {
        if (chatUI.GetCurrentChatAreaName() == currentConversationChatArea)
        {
            choiceHandler.ShowChoices(currentStory.currentChoices, currentConversationChatArea);
        }
        else
        {
            StartCoroutine(WaitForPlayerToSwitchToCorrectArea());
        }
    }

    private IEnumerator WaitForPlayerToSwitchToCorrectArea()
    {
        while (chatUI.GetCurrentChatAreaName() != currentConversationChatArea)
        {
            yield return null;
            if (!DialogueIsPlaying) yield break;
        }
        choiceHandler.ShowChoices(currentStory.currentChoices, currentConversationChatArea);
    }

    public void MakeChoice(int choiceIndex)
    {
        if (currentStory.currentChoices.Count > choiceIndex)
        {
            AudioCollection.GetInstance().PlaySFX(AudioCollection.GetInstance().buttonClick);

            string choiceText = currentStory.currentChoices[choiceIndex].text;

            DisplayPlayerChoice(choiceText);

            currentStory.ChooseChoiceIndex(choiceIndex);

            ContinueStory();
        }
    }

    private void DisplayPlayerChoice(string choiceText)
    {
        if (string.IsNullOrEmpty(choiceText))
        {
            Debug.LogWarning("Attempted to create empty choice message, skipping.");
            return;
        }

        Sprite playerPortrait = GetPortraitSprite("player_portrait");
        ChatMessage playerMessage = new PlayerMessage(choiceText, playerPortrait);

        string currentChatArea = chatUI.GetCurrentChatAreaName();
        string currentSpeaker = "You";

        bool shouldAppendChoice = lastSpeakerInCurrentArea == currentSpeaker &&
                                 lastMessageFromUser.ContainsKey(currentSpeaker);

        GameObject messageUI;

        if (shouldAppendChoice)
        {
            // Debug.Log($"Appending choice to existing player message: {choiceText}");
            messageUI = lastMessageFromUser[currentSpeaker];
            currentPlayerMessageUI = messageUI;
            StartCoroutine(AppendToPlayerMessage(choiceText));
        }
        else
        {
            // Debug.Log($"Creating new player choice message: {choiceText}");
            messageUI = chatUI.AddMessageToChat(playerMessage);

            if (messageUI != null)
            {
                ChatMessageUI chatMessageUI = messageUI.GetComponentInChildren<ChatMessageUI>();
                if (chatMessageUI != null)
                {
                    chatMessageUI.SetMessageText(choiceText);
                }
                currentPlayerMessageUI = messageUI;
                lastMessageFromUser["You"] = messageUI;
                lastSpeakerInCurrentArea = "You";
            }
        }

        chatUI.ScrollToBottom();
    }

    private IEnumerator HandleEndOfConversation()
    {
        string nextChatArea = DetermineNextChatArea();
        string nextBranch = DetermineNextBranch();
        //Debug.Log($"End of conversation. NextChatArea: {nextChatArea}, NextBranch: {nextBranch}");

        CheckAndUnlockNextChatArea();

        FirstDayManager firstDayManager = FirstDayManager.GetInstance();

        if (firstDayManager != null && firstDayManager.IsFirstDay() && !firstDayManager.IsCompletedFirstDialogue())
        {
            firstDayManager.CompleteFirstDialogue();
            firstDayManager.HandleFirstDayConversationEnd();

            ServerLockManager serverLockManager = ServerLockManager.GetInstance();
            if (serverLockManager != null)
            {
                serverLockManager.UnlockServerSwitching();
            }

            StartCoroutine(ExitDialogueMode());
            yield break;
        }

        if (inkFileManager.IsDayTransitionBranch(nextBranch))
        {
            yield return new WaitForSeconds(3f);
            yield return StartCoroutine(HandleDayTransition());
            if (DialogueIsPlaying) yield break;
        }
        else if (inkFileManager.IsToBeContinuedBranch(nextBranch))
        {
            yield return new WaitForSeconds(3f);
            GameManager gameManager = GameManager.GetInstance();
            if (gameManager != null)
            {
                gameManager.ShowToBeContinuedPanel();
            }
        }
        else if (!string.IsNullOrEmpty(nextBranch))
        {
            inkFileManager.MoveToNextBranch(nextBranch);
            TextAsset nextInkFile = inkFileManager.GetCurrentInkFile();
            if (nextInkFile != null)
            {
                currentConversationChatArea = nextChatArea ?? currentConversationChatArea;
                EnterDialogueMode(nextInkFile);
                yield break;
            }
        }
        StartCoroutine(ExitDialogueMode());
    }

    private void CheckAndUnlockNextChatArea()
    {
        string nextChatArea = DetermineNextChatArea();

        if (!string.IsNullOrEmpty(nextChatArea))
        {
            ChatAreaButtonManager buttonManager = ChatAreaButtonManager.GetInstance();
            if (buttonManager != null)
            {
                buttonManager.UnlockDMArea(nextChatArea);
            }
        }
    }

    private IEnumerator HandleDayTransition()
    {
        lastMessageFromUser.Clear();
        currentState.Reset();
        currentPlayerMessageUI = null;
        lastSpeakerInCurrentArea = null;

        DayManager dayManager = DayManager.GetInstance();
        if (dayManager != null)
        {
            dayManager.StartNextDay();
            yield return new WaitForSeconds(dayManager.TransitionDuration + 0.5f);

            Debug.Log("Day transition complete. Food panel should appear for day 2+");
        }

        StartCoroutine(ExitDialogueMode());
    }

    private string DetermineNextChatArea()
    {
        if (currentStory.variablesState.GlobalVariableExistsWithName("nextChatArea"))
        {
            object nextChatAreaObj = currentStory.variablesState["nextChatArea"];
            return nextChatAreaObj?.ToString();
        }
        return null;
    }

    private string DetermineNextBranch()
    {
        if (currentStory.variablesState.GlobalVariableExistsWithName("nextBranch"))
        {
            object nextBranchObj = currentStory.variablesState["nextBranch"];
            return nextBranchObj?.ToString();
        }

        List<string> possibleBranches = inkFileManager.GetNextPossibleBranches();
        if (possibleBranches.Count == 1) return possibleBranches[0];
        else if (possibleBranches.Count > 1) return possibleBranches[0];
        return null;
    }

    private IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(0.2f);
        DialogueIsPlaying = false;
        lastMessageFromUser.Clear();
        currentState.Reset();
        currentPlayerMessageUI = null;
        lastSpeakerInCurrentArea = null;
    }

    public void SwitchToChatArea(string areaName)
    {
        chatUI.SwitchToChatArea(areaName);
    }

    public ChatAreaManager GetChatAreaManager()
    {
        return chatUI?.GetChatAreaManager();
    }
}