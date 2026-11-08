using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChoiceButton : MonoBehaviour
{
    Button button;
    int choiceIndex;
    private TMP_Text buttonText;
    void Awake()
    {
        button = GetComponent<Button>();
        choiceIndex = transform.GetSiblingIndex();
        button.onClick.AddListener(OnButtonClicked);

        buttonText = GetComponentInChildren<TMP_Text>();
        if (buttonText == null)
        {
            Debug.LogError("TMP_Text component not found in children of ChoiceButton.");
        }
    }

    public void SetChoiceText(string text)
    {
        buttonText.text = text;
        LayoutRebuilder.ForceRebuildLayoutImmediate(buttonText.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent as RectTransform);
    }

    void OnButtonClicked()
    {
        ChannelEventManager.SelectChoice(choiceIndex);
    }
}
