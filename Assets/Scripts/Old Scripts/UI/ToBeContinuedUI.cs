using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToBeContinuedUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI creditsText;
    [SerializeField] private TextMeshProUGUI toBeContinuedText;
    [SerializeField] private GameObject backToMainMenuButton;
    [SerializeField] private float scrollSpeed = 40f;

    private float creditsTargetY = 2500f;
    private float tBCTargetY = 100f;
    private float backButtonTargetY = -100f;

    private float creditsStartY = -1900f;
    private float tBCStartY = -4000f;
    private float backButtonStartY = -4200f;

    private RectTransform creditsRect;
    private RectTransform tBCRect;
    private RectTransform backButtonRect;

    private bool creditsDone;
    private bool tBCDone;
    private bool backButtonDone;
    private bool isInitialized = false;

    private void OnEnable()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (isInitialized) return;

        if (creditsText != null)
        {
            creditsRect = creditsText.rectTransform;
        }

        if (toBeContinuedText != null)
        {
            tBCRect = toBeContinuedText.rectTransform;
        }

        if (backToMainMenuButton != null)
        {
            backButtonRect = backToMainMenuButton.GetComponent<RectTransform>();

            Button button = backToMainMenuButton.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.RemoveListener(OnBackToMainMenuClicked);
                button.onClick.AddListener(OnBackToMainMenuClicked);
            }
        }

        ResetRectTransform();

        creditsDone = tBCDone = backButtonDone = false;
        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized) return;

        float delta = scrollSpeed * Time.unscaledDeltaTime;

        if (creditsRect != null && !creditsDone)
        {
            Vector2 pos = creditsRect.anchoredPosition;
            pos.y = Mathf.Min(creditsTargetY, pos.y + delta);
            creditsRect.anchoredPosition = pos;
            if (pos.y >= creditsTargetY) creditsDone = true;
        }

        if (tBCRect != null && !tBCDone)
        {
            Vector2 pos = tBCRect.anchoredPosition;
            pos.y = Mathf.Min(tBCTargetY, pos.y + delta);
            tBCRect.anchoredPosition = pos;
            if (pos.y >= tBCTargetY) tBCDone = true;
        }

        if (backButtonRect != null && !backButtonDone)
        {
            Vector2 pos = backButtonRect.anchoredPosition;
            pos.y = Mathf.Min(backButtonTargetY, pos.y + delta);
            backButtonRect.anchoredPosition = pos;
            if (pos.y >= backButtonTargetY) backButtonDone = true;
        }
    }

    private void ResetRectTransform()
    {
        if (creditsRect != null)
        {
            creditsRect.anchoredPosition = new Vector2(0, creditsStartY);
        }
        if (tBCRect != null)
        {
            tBCRect.anchoredPosition = new Vector2(0, tBCStartY);
        }
        if (backButtonRect != null)
        {
            backButtonRect.anchoredPosition = new Vector2(0, backButtonStartY);
        }
    }

    public void RestartAnimation()
    {
        ResetRectTransform();
        creditsDone = tBCDone = backButtonDone = false;
    }

    private void OnBackToMainMenuClicked()
    {
        GameManager gameManager = GameManager.GetInstance();
        if (gameManager != null)
        {
            gameManager.ReturnToMainMenuFromTBC();
        }
        else
        {
            Debug.LogError("GameManager not found when trying to return to main menu!");
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }
}