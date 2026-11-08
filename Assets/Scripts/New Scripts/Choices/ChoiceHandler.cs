using UnityEngine;
using System;
using System.Collections.Generic;
using DG.Tweening;
using Ink.Runtime;
public class ChoiceHandler : MonoBehaviour
{
    [SerializeField] RectTransform chatAreaPanel;
    float originalHeight;

    public static Action<List<Choice>> TriggerShowChoices;
    public static Action TriggerHideChoices;

    [SerializeField] Transform choicesParent;
    List<ChoiceButton> choiceButtons;

    void OnEnable()
    {
        TriggerShowChoices += ShowChoicesHandler;
        TriggerHideChoices += HideChoicesHandler;
    }

    void OnDisable()
    {
        TriggerShowChoices -= ShowChoicesHandler;
        TriggerHideChoices -= HideChoicesHandler;
    }

    void Awake()
    {
        if (chatAreaPanel == null)
        {
            Debug.LogError("ChatAreaPanel is not assigned in ChoiceHandler!");
        }

        originalHeight = chatAreaPanel.rect.height;

        choiceButtons = new List<ChoiceButton>();
        foreach (Transform child in choicesParent)
        {
            ChoiceButton choiceButton = child.GetComponent<ChoiceButton>();
            if (choiceButton != null)
            {
                choiceButtons.Add(choiceButton);
            }
            else
            {
                Debug.LogError("ChoiceButton component not found on child: " + child.name);
            }
        }
    }

    void ShowChoicesHandler(List<Choice> choices)
    {
        float choicesHeight = 0;
        for(int i = 0; i < choices.Count; i++)
        {
            Debug.Log("Choice " + i + ": " + choices[i].text);
            if (i < choiceButtons.Count)
            {
                choiceButtons[i].SetChoiceText(choices[i].text);
                Debug.Log("Getting height of choice button " + i + ": " + choiceButtons[i].GetComponent<RectTransform>().rect.height);
                choicesHeight += choiceButtons[i].GetComponent<RectTransform>().rect.height;
            }
        }
        chatAreaPanel.DOSizeDelta(new Vector2(chatAreaPanel.sizeDelta.x, originalHeight - choicesHeight), 0.5f).SetEase(Ease.OutQuad);
    }
    
    void HideChoicesHandler()
    {
        chatAreaPanel.DOSizeDelta(new Vector2(chatAreaPanel.sizeDelta.x, originalHeight), 0.3f).SetEase(Ease.OutQuad);
    }
}
