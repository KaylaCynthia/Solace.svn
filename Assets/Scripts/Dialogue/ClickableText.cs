using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Text.RegularExpressions;

[RequireComponent(typeof(TMP_Text))]
public class ClickableText : MonoBehaviour,IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    TMP_Text text;
    private bool isHovering = false;
    private bool isLinkHovered = false;

    void OnEnable()
    {
        text = GetComponent<TMP_Text>();
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        ChangeLinkHighlight(false);
    }

    void Update()
    {
        // Only run link-checking logic if the mouse is actually inside the text box!
        if (!isHovering) return;

        CheckHover(new PointerEventData(EventSystem.current) { position = Input.mousePosition });
    }

    public void CheckHover(PointerEventData eventData)
    {
        if(!isHovering) return;

        // Find the link index
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(text, eventData.position, null);

        // SAFETY CHECK: Ensure linkIndex is valid and within the active linkCount range
        if (linkIndex != -1 && linkIndex < text.textInfo.linkCount)
        {
            TMP_LinkInfo linkInfo = text.textInfo.linkInfo[linkIndex];
            string linkID = linkInfo.GetLinkID();

            ChangeLinkHighlight(true);

        }
        else
        {
            ChangeLinkHighlight(false);
        }
    }

    void ChangeLinkHighlight(bool isHovered)
    {
        if (isHovered)
        {
            if (isLinkHovered) return;
            isLinkHovered = true;
            SetLinkColorHighlight("#47f3ff55");
        }
        else
        {
            if (!isLinkHovered) return;
            isLinkHovered = false;
            SetLinkColorHighlight("#FFFF0088");
        }
    }

    void SetLinkColorHighlight(string colorHex)
    {
        Debug.Log($"Setting link color highlight to: {colorHex}");
        string textContent = text.text;
        string pattern = @"(<mark=)[^>]+(>)";
        string replacement = $"$1{colorHex}$2";
        string updatedText = Regex.Replace(textContent, pattern, replacement);
        text.text = updatedText;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(text, eventData.position, null);

        // SAFETY CHECK: Ensure linkIndex is valid and within the active linkCount range
        if (linkIndex != -1 && linkIndex < text.textInfo.linkCount)
        {
            TMP_LinkInfo linkInfo = text.textInfo.linkInfo[linkIndex];
            string linkID = linkInfo.GetLinkID();

            Debug.Log($"Clicked link ID: {linkID}");

        }
        else
        {
            Debug.Log("Missed link (Clicked normal text or empty space).");
        }
    }
}
