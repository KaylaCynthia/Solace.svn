using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private RectTransform arrowTransform;
    [SerializeField] private Animator arrowAnimator;

    private TutorialManager tutorialManager;

    public void Initialize(TutorialManager manager)
    {
        tutorialManager = manager;
        Hide();
    }

    public void Show()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(true);
        }

        SetDialogueText("");
        HideArrow();
    }

    public void Hide()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }

        HideArrow();
    }

    public void SetDialogueText(string text)
    {
        if (dialogueText != null)
        {
            dialogueText.text = text;
        }
    }

    public void AppendDialogueText(string text)
    {
        if (dialogueText != null)
        {
            dialogueText.text += text;
        }
    }

    public void ShowArrow(Vector2 position, float rotation = 0f)
    {
        if (arrowTransform != null)
        {
            arrowTransform.gameObject.SetActive(true);
            arrowTransform.anchoredPosition = position;
            arrowTransform.localEulerAngles = new Vector3(0f, 0f, rotation);

            //if (arrowAnimator != null)
            //{
            //    arrowAnimator.Rebind();
            //    arrowAnimator.Update(0f);
            //}
        }
    }

    public void HideArrow()
    {
        if (arrowTransform != null)
        {
            arrowTransform.gameObject.SetActive(false);
        }
    }

    public void UpdateArrowPosition(Vector2 newPosition, float rotation = 0f)
    {
        if (arrowTransform != null && arrowTransform.gameObject.activeInHierarchy)
        {
            arrowTransform.anchoredPosition = newPosition;
            arrowTransform.localEulerAngles = new Vector3(0f, 0f, rotation);

            //if (arrowAnimator != null)
            //{
            //    arrowAnimator.Rebind();
            //    arrowAnimator.Update(0f);
            //}
        }
    }
}