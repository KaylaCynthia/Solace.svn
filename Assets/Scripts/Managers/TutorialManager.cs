using System.Collections;
using UnityEngine;
using Ink.Runtime;
using Ink.UnityIntegration;

public class TutorialManager : MonoBehaviour
{
    [Header("Tutorial References")]
    [SerializeField] private TutorialUI tutorialUI;
    [SerializeField] private float lettersPerSecond = 30f;
    [SerializeField] private InkFileManager inkFileManager;

    private Story currentTutorialStory;
    private TutorialParser tutorialParser;
    private TutorialState currentState;
    private bool isTutorialActive = false;
    private bool isDisplayingText = false;
    private Coroutine currentDisplayCoroutine;

    private static TutorialManager instance;
    public static TutorialManager GetInstance() => instance;

    public System.Action OnTutorialStarted;
    public System.Action OnTutorialCompleted;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        tutorialParser = new TutorialParser();
        currentState = new TutorialState();

        if (tutorialUI != null)
        {
            tutorialUI.Initialize(this);
        }
    }

    private void Update()
    {
        if (isTutorialActive && Input.GetMouseButtonDown(0))
        {
            ContinueTutorial();
        }
    }

    public void StartTutorial(string conversationKey)
    {
        if (inkFileManager == null)
        {
            Debug.LogError("InkFileManager not found! Cannot start tutorial.");
            return;
        }

        InkFile tutorialInkFile = inkFileManager.GetInkFileByKey(conversationKey);
        if (tutorialInkFile == null)
        {
            Debug.LogError($"Tutorial Ink file not found for key: {conversationKey}");
            return;
        }

        currentTutorialStory = new Story(tutorialInkFile.storyJson);
        isTutorialActive = true;
        isDisplayingText = false;

        tutorialUI.Show();
        OnTutorialStarted?.Invoke();

        ContinueTutorial();
    }

    private void ContinueTutorial()
    {
        if (isDisplayingText)
        {
            if (currentDisplayCoroutine != null)
            {
                StopCoroutine(currentDisplayCoroutine);
                currentDisplayCoroutine = null;
            }
            isDisplayingText = false;

            ContinueToNextLine();
            return;
        }

        ContinueToNextLine();
    }

    private void ContinueToNextLine()
    {
        if (currentTutorialStory == null) return;

        AudioCollection.GetInstance().PlaySFX(AudioCollection.GetInstance().buttonClick);

        if (!currentTutorialStory.canContinue)
        {
            EndTutorial();
            return;
        }

        string nextLine = currentTutorialStory.Continue();
        tutorialParser.ProcessTags(currentTutorialStory.currentTags, currentState);
        ApplyArrowState();
        currentDisplayCoroutine = StartCoroutine(DisplayTutorialText(nextLine));
    }

    private void ApplyArrowState()
    {
        if (currentState.ArrowActive)
        {
            tutorialUI.ShowArrow(currentState.ArrowPosition, currentState.ArrowRotation);
        }
        else
        {
            tutorialUI.HideArrow();
        }
    }

    private IEnumerator DisplayTutorialText(string text)
    {
        isDisplayingText = true;

        tutorialUI.SetDialogueText("");
        string processedText = ProcessTextWithPlayerName(text);

        string currentDisplayText = "";
        foreach (char letter in processedText.ToCharArray())
        {
            currentDisplayText += letter;
            tutorialUI.SetDialogueText(currentDisplayText);
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }

        isDisplayingText = false;
        currentDisplayCoroutine = null;
    }

    private string ProcessTextWithPlayerName(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        PlayerDataManager playerDataManager = PlayerDataManager.GetInstance();
        if (playerDataManager != null)
        {
            string playerName = playerDataManager.PlayerData.PlayerName;
            return text.Replace("Player_Name", playerName);
        }

        return text;
    }

    private void EndTutorial()
    {
        isTutorialActive = false;
        isDisplayingText = false;

        if (currentDisplayCoroutine != null)
        {
            StopCoroutine(currentDisplayCoroutine);
            currentDisplayCoroutine = null;
        }

        tutorialUI.Hide();
        OnTutorialCompleted?.Invoke();

        Debug.Log("Tutorial completed!");
    }

    public bool IsTutorialActive() => isTutorialActive;

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}