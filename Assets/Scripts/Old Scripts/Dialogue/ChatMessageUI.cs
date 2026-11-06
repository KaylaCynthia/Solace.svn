using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChatMessageUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image portraitImage;
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private RectTransform messageContainer;
    [SerializeField] private RectTransform chatMessageRect;
    [SerializeField] private HorizontalLayoutGroup layoutGroup;

    [Header("Sizing")]
    [SerializeField] private float minHeight = 60f;
    [SerializeField] private float maxWidth = 600f;
    [SerializeField] private float spacingBetweenMessages = 10f;

    [Header("Default Portrait")]
    [SerializeField] private Sprite defaultPortrait;

    private float originalMessageTextY;
    private float originalSpeakerTextY;

    private void Awake()
    {
        if (messageText != null)
        {
            originalMessageTextY = messageText.rectTransform.anchoredPosition.y;
        }
        if (speakerText != null)
        {
            originalSpeakerTextY = speakerText.rectTransform.anchoredPosition.y;
        }

        SetupAnchorsForDownwardExpansion();
    }

    private void SetupAnchorsForDownwardExpansion()
    {
        if (chatMessageRect != null)
        {
            chatMessageRect.pivot = new Vector2(0.5f, 1f);
            chatMessageRect.anchorMin = new Vector2(0.5f, 1f);
            chatMessageRect.anchorMax = new Vector2(0.5f, 1f);
        }

        if (messageText != null)
        {
            RectTransform textRect = messageText.rectTransform;
            textRect.pivot = new Vector2(0f, 1f);
            textRect.anchorMin = new Vector2(0f, 1f);
            textRect.anchorMax = new Vector2(0f, 1f);
        }
    }

    public void Initialize(ChatMessage message)
    {
        SetPortraitSprite(message.PortraitSprite);

        speakerText.text = message.Speaker;
        messageText.text = message.Message;

        if (message.IsPlayer)
        {
            SetPlayerMessageStyle();
        }
        else
        {
            SetNPCMessageStyle();
        }

        StartCoroutine(ResizeAfterLayout());
    }

    private void SetPortraitSprite(Sprite portraitSprite)
    {
        if (portraitImage != null)
        {
            portraitImage.sprite = portraitSprite ?? defaultPortrait;
            portraitImage.preserveAspect = true;
        }
    }

    public void AppendMessage(string additionalText)
    {
        messageText.text += additionalText;
        if(messageText.text.Contains("\n\n"))
        {
            messageText.text = messageText.text.Replace("\n\n", "\n");
        }
        StartCoroutine(ResizeAfterLayout());
        messageText.GetComponent<ClickableText>()?.SetText(messageText.text);
    }

    public void SetMessageText(string text)
    {
        messageText.text = text;
        StartCoroutine(ResizeAfterLayout());
    }

    private System.Collections.IEnumerator ResizeAfterLayout()
    {
        messageText.ForceMeshUpdate();
        yield return new WaitForEndOfFrame();
        messageText.ForceMeshUpdate(true);
        ResizeMessageBubble();
    }

    private void ResizeMessageBubble()
    {
        if (messageText == null || chatMessageRect == null || speakerText == null) return;

        float messageHeight = messageText.textBounds.size.y;

        if (messageHeight <= 0.1f)
        {
            Vector2 messagePreferredSize = messageText.GetPreferredValues();
            messageHeight = messagePreferredSize.y;
        }

        Vector2 speakerPreferredSize = speakerText.GetPreferredValues(speakerText.text);
        float speakerHeight = speakerPreferredSize.y;
        float totalHeight = speakerHeight + messageHeight + spacingBetweenMessages;

        messageText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, messageHeight);
        chatMessageRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalHeight);

        if (messageContainer != null)
        {
            messageContainer.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalHeight);
        }

        LayoutElement layoutElement = chatMessageRect.GetComponent<LayoutElement>();
        if (layoutElement == null)
        {
            layoutElement = chatMessageRect.gameObject.AddComponent<LayoutElement>();
        }
        layoutElement.preferredHeight = totalHeight;
        layoutElement.minHeight = totalHeight;

        LayoutRebuilder.ForceRebuildLayoutImmediate(chatMessageRect);

        if (chatMessageRect.parent != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(chatMessageRect.parent as RectTransform);
        }
    }

    private void SetPlayerMessageStyle()
    {
        if (layoutGroup != null)
        {
            layoutGroup.childAlignment = TextAnchor.UpperLeft;
        }

        if (chatMessageRect != null)
        {
            chatMessageRect.pivot = new Vector2(0f, 1f);
            chatMessageRect.anchorMin = new Vector2(0f, 1f);
            chatMessageRect.anchorMax = new Vector2(0f, 1f);
        }
    }

    private void SetNPCMessageStyle()
    {
        if (layoutGroup != null)
        {
            layoutGroup.childAlignment = TextAnchor.UpperLeft;
        }

        if (chatMessageRect != null)
        {
            chatMessageRect.pivot = new Vector2(0f, 1f);
            chatMessageRect.anchorMin = new Vector2(0f, 1f);
            chatMessageRect.anchorMax = new Vector2(0f, 1f);
        }
    }
}