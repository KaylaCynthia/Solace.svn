using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Text.RegularExpressions;

[RequireComponent(typeof(TMP_Text))]
public class ClickableText : MonoBehaviour,IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    // 8ab7ff default
    //<color=#326fd1><mark=#56e1e811>heyy!</mark></color>

    // Channel Links
    // <link="channel-id"><color=#8ab7ff><mark=#8ab7ff11>    text    </mark></color></link>

    // Hyperlinks
    // Default: #f06800
    // Hovered: #ffba85

    TMP_Text text;
    private bool isHovering = false;
    private string originalText;
    private string hoveredLinkID = null;

    [SerializeField] private Color channelLinkColor = new Color(0.541f, 0.718f, 1f); 
    [SerializeField] private Color channelLinkHoverColor = new Color(0.278f, 0.953f, 1f, 0.33f); 
    [SerializeField] private Color channelLinkMarkColor = Color.clear; 
    [SerializeField] private Color channelLinkMarkHoverColor = new Color(0.278f, 0.953f, 1f, 0.33f); 
    [SerializeField] private Color hyperlinkColor = new Color(0.941f, 0.408f, 0f); 
    [SerializeField] private Color hyperlinkHoverColor = new Color(1f, 0.729f, 0.522f); 

    void OnEnable()
    {
        text = GetComponent<TMP_Text>();
    }
    
    public void SetText(string newText)
    {
        originalText = newText;
        text.text = originalText;
        text.ForceMeshUpdate(); // Ensure TMP recalculates immediately for accurate hover detection
        RedrawText();
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        if (hoveredLinkID != null)
        {
            hoveredLinkID = null;
            RedrawText();
        }
    }

    void Update()
    {
        if (!isHovering) return;

        CheckHover(new PointerEventData(EventSystem.current) { position = Input.mousePosition });
    }

    public void CheckHover(PointerEventData eventData)
    {
        if (!isHovering) return;

        int linkIndex = TMP_TextUtilities.FindIntersectingLink(text, eventData.position, null);

        string newHoveredID = null;

        if (linkIndex != -1 && linkIndex < text.textInfo.linkCount)
        {
            TMP_LinkInfo linkInfo = text.textInfo.linkInfo[linkIndex];
            newHoveredID = linkInfo.GetLinkID();
        }

        // Only redraw if the hovered link actually changed (prevents redundant frame updates)
        if (newHoveredID != hoveredLinkID)
        {
            Debug.Log($"Hovered link changed from {hoveredLinkID} to {newHoveredID}");
            hoveredLinkID = newHoveredID;
            RedrawText();
        }
    }

    void RedrawText()
    {
        // Regex pattern to capture:
        // Group 1: Opening link tag (e.g., <link="channel-123"> or <link=shop>)
        // Group 2: The link ID itself
        // Group 3: The inner text inside the link
        // Group 4: Closing link tag (</link>)
        string pattern = @"(<link=([^>]+)>)(.*?)(<\/link>)";

        string updatedText = Regex.Replace(originalText, pattern, match =>
        {
            string openTag = match.Groups[1].Value;
            string linkID = match.Groups[2].Value;
            string innerText = match.Groups[3].Value;
            string closeTag = match.Groups[4].Value;

            bool isThisHovered = linkID == hoveredLinkID;
            bool isChannel = linkID.StartsWith("channel");

            if (isChannel)
            {
                // Channel Link Styling
                string textColor = isThisHovered ? ColorUtility.ToHtmlStringRGB(channelLinkHoverColor) : ColorUtility.ToHtmlStringRGB(channelLinkColor);
                string markColor = isThisHovered ? ColorUtility.ToHtmlStringRGBA(channelLinkMarkHoverColor) : ColorUtility.ToHtmlStringRGBA(channelLinkMarkColor);
                
                // Wrap inner text with color and mark tags
                return $"{openTag}<color=#{textColor}><mark=#{markColor}>{innerText}</mark></color>{closeTag}";
            }
            else
            {
                // Standard Hyperlink Styling
                string textColor = isThisHovered ? ColorUtility.ToHtmlStringRGB(hyperlinkHoverColor) : ColorUtility.ToHtmlStringRGB(hyperlinkColor);
                
                // Wrap inner text with hyperlink color
                return $"{openTag}<color=#{textColor}>{innerText}</color>{closeTag}";
            }
        });

        text.text = updatedText;
        text.ForceMeshUpdate(); // Ensure TMP recalculates immediately for accurate hover detection
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(text, eventData.position, null);

        if (linkIndex != -1 && linkIndex < text.textInfo.linkCount)
        {
            TMP_LinkInfo linkInfo = text.textInfo.linkInfo[linkIndex];
            string linkID = linkInfo.GetLinkID();

            Debug.Log($"Clicked link ID: {linkID}");
            
            if (linkID.StartsWith("channel") || linkID.Contains("channel"))
            {
                // Handle channel click logic here
            }
            else
            {
                // Handle standard hyperlink click logic here
            }
        }
        else
        {
            Debug.Log("Missed link (Clicked normal text or empty space).");
        }
    }
}
