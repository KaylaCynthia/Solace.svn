using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Ink.Runtime;
using DG.Tweening;

[System.Serializable]
public class ChoiceHandler
{
    [SerializeField] private GameObject chatAreaPanel;
    [SerializeField] private GameObject choicePanel;
    [SerializeField] private Button inputMessageButton;
    [SerializeField] private GameObject[] choiceButtons;

    private TextMeshProUGUI[] choicesText;
    private TextMeshProUGUI inputMessageText;
    private System.Action<int> onChoiceSelected;
    private List<Choice> currentChoices;
    private string currentChatAreaForChoices;
    private string currentActiveChatArea;
    private bool isInitialized = false;
    private bool isChoicesOpen = false;

    public void Initialize(System.Action<int> choiceCallback, ChatUI chatUI)
    {
        if (isInitialized) return;

        onChoiceSelected = choiceCallback;

        if (choicePanel == null)
        {
            Debug.LogError("ChoicePanel is not assigned in ChoiceHandler!");
            return;
        }

        if (chatUI != null)
        {
            currentActiveChatArea = chatUI.GetCurrentChatAreaName();
        }

        if (choiceButtons != null && choiceButtons.Length > 0)
        {
            choicesText = new TextMeshProUGUI[choiceButtons.Length];
            for (int i = 0; i < choiceButtons.Length; i++)
            {
                if (choiceButtons[i] != null)
                {
                    choicesText[i] = choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                    choiceButtons[i].SetActive(false);
                }
                else
                {
                    Debug.LogWarning($"Choice button at index {i} is null!");
                }
            }
        }
        else
        {
            Debug.LogError("ChoiceButtons array is empty or null!");
        }

        if (inputMessageButton != null)
        {
            inputMessageButton.onClick.RemoveAllListeners();
            inputMessageButton.onClick.AddListener(ToggleChoicePanel);
            inputMessageText = inputMessageButton.GetComponentInChildren<TextMeshProUGUI>();
        }
        else
        {
            Debug.LogError("InputMessageButton is not assigned in ChoiceHandler!");
        }

        ChatAreaEvents.OnChatAreaChanged += OnChatAreaChanged;

        isInitialized = true;
        //Debug.Log("ChoiceHandler Initialized successfully");
    }

    public void ShowChoices(List<Choice> choices, string currentChatArea)
    {
        inputMessageText = inputMessageButton.GetComponentInChildren<TextMeshProUGUI>();
        inputMessageText.color = Color.red;
        currentChatAreaForChoices = currentChatArea;
        currentChoices = choices;
    }

    private void ToggleChoicePanel()
    {
        //Debug.Log($"ToggleChoicePanel called. IsOpen: {IsChoicePanelOpen}, InCorrectArea: {IsInCorrectChatArea()}");


        // Check if area is correct
        if (!IsInCorrectChatArea())
        {
            Debug.Log($"Cannot open choice panel. Current area: {currentActiveChatArea}, Required area: {currentChatAreaForChoices}");
            return;
        }

        if(currentChoices == null || currentChoices.Count == 0)
        {
            Debug.Log("No choices available to display.");
            return;
        }

        if(isChoicesOpen)
        {
            CloseChoicePanel();
        }
        else 
        {
            OpenChoicePanel();
        }
    }

    bool isChoiceMoving = false;
    private void OpenChoicePanel()
    {
        if (!IsInCorrectChatArea())
        {
            Debug.LogWarning("Tried to open choice panel in wrong chat area");
            return;
        }

        DisplayChoices();
        TriggerShowAnimation();
        ChatAreaEvents.TriggerChoicePanelStateChanged(true);
    }

    private void CloseChoicePanel()
    {
        inputMessageText.color = new Color32(0xFF, 0xE1, 0xBA, 0xFF);
        TriggerCloseAnimation();
        ChatAreaEvents.TriggerChoicePanelStateChanged(false);
    }

    private void TriggerShowAnimation()
    {
        if (isChoiceMoving) return;
        isChoiceMoving = true;

        int choiceCount = currentChoices?.Count ?? 0;

        float targetY = choiceCount * 70f;

        chatAreaPanel.GetComponent<RectTransform>().DOAnchorPosY(targetY, 0.5f).OnComplete(() =>
        {
            isChoiceMoving = false;
            isChoicesOpen = true;
        });
    }

    private void TriggerCloseAnimation()
    {
        if (isChoiceMoving) return;
        isChoiceMoving = true;

        Debug.Log("TriggerCloseAnimation called");

        chatAreaPanel.GetComponent<RectTransform>().DOAnchorPosY(0f, 0.5f).OnComplete(() =>
        {
            isChoiceMoving = false;
            isChoicesOpen = false;
        });
    }

    private void DisplayChoices()
    {
        if (!IsInCorrectChatArea()) return;

        foreach (var button in choiceButtons)
        {
            if (button != null)
                button.SetActive(false);
        }

        int choiceCount = currentChoices?.Count ?? 0;
        for (int i = 0; i < Mathf.Min(choiceCount, choiceButtons.Length); i++)
        {
            if (choiceButtons[i] != null)
            {
                choiceButtons[i].SetActive(true);
                if (choicesText[i] != null)
                {
                    choicesText[i].text = currentChoices[i].text;
                }

                Button button = choiceButtons[i].GetComponent<Button>();
                if (button != null)
                {
                    int index = i;
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => OnChoiceButtonClicked(index));
                }
            }
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(choicePanel.GetComponent<RectTransform>());
        SelectFirstChoice();
    }

    private void OnChoiceButtonClicked(int choiceIndex)
    {
        // Make sure choice cannot be pressed in animation
        if(isChoiceMoving) return;

        List<Choice> choicesCopy = currentChoices != null ? new List<Choice>(currentChoices) : null;
        int selectedIndex = choiceIndex;

        if (IsInCorrectChatArea() && choiceIndex < currentChoices.Count)
        {
            CloseChoicePanel();

            onChoiceSelected?.Invoke(selectedIndex);
            //Debug.Log($"Choice selected: {selectedIndex} - {choicesCopy[selectedIndex].text}");
        }
        else
        {
            Debug.LogError($"Invalid choice selection: index {choiceIndex}, choices count: {currentChoices?.Count}");
        }
    }

    private void SelectFirstChoice()
    {
        if (EventSystem.current == null) return;

        EventSystem.current.SetSelectedGameObject(null);
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < currentChoices.Count && choiceButtons[i] != null && choiceButtons[i].activeInHierarchy)
            {
                EventSystem.current.SetSelectedGameObject(choiceButtons[i].gameObject);
                break;
            }
        }
    }

    public void Hide()
    {
        TriggerCloseAnimation();
        ChatAreaEvents.TriggerChoicePanelStateChanged(false);

        currentChoices = null;
        currentChatAreaForChoices = null;
    }

    private void OnChatAreaChanged(string newAreaName)
    {
        currentActiveChatArea = newAreaName;
        CloseChoicePanel();
    }

    private bool IsInCorrectChatArea()
    {
        bool isCorrect = currentActiveChatArea == currentChatAreaForChoices && currentChoices != null && currentChoices.Count > 0;
        return isCorrect;
    }
}