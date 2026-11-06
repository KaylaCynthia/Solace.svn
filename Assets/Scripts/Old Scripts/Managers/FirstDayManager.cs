using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FirstDayManager : MonoBehaviour
{
    [Header("First Day Settings")]
    [SerializeField] private float autoReturnToHomeDelay = 5f;
    [SerializeField] private GameObject fadePanel;
    [SerializeField] private float fadeDuration = 1f;

    private bool isFirstDay = true;
    private bool isCompletedFirstDialogue = false;
    private bool hasReturnedToHome = false;
    private Image fadeImage;

    private static FirstDayManager instance;
    public static FirstDayManager GetInstance() => instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        if (fadePanel != null)
        {
            fadeImage = fadePanel.GetComponent<Image>();
            if (fadeImage == null)
            {
                Debug.LogError("FadePanel doesn't have an Image component!");
            }
        }

        ServerLockManager serverLockManager = ServerLockManager.GetInstance();
        if (serverLockManager != null && isFirstDay)
        {
            serverLockManager.LockServerSwitching();
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    public void HandleFirstDayConversationEnd()
    {
        if (isFirstDay && !hasReturnedToHome)
        {
            Debug.Log("First day first conversation ended.");
            StartCoroutine(AutoReturnToHome());
        }
    }

    private IEnumerator AutoReturnToHome()
    {
        Debug.Log("First conversation ended, returning to home in " + autoReturnToHomeDelay + " seconds");
        yield return new WaitForSeconds(autoReturnToHomeDelay);

        yield return StartCoroutine(FadeOut());

        AppSystemManager appManager = AppSystemManager.GetInstance();
        TutorialManager tutorialManager = TutorialManager.GetInstance();
        if (appManager != null)
        {
            appManager.ReturnToHomeScreen();

            yield return StartCoroutine(FadeIn());

            if (tutorialManager != null)
            {
                tutorialManager.StartTutorial("first_tutorial");
            }

            hasReturnedToHome = true;
        }
    }

    private IEnumerator FadeOut()
    {
        if (fadePanel == null || fadeImage == null) yield break;

        fadePanel.SetActive(true);
        yield return StartCoroutine(Fade(0f, 1f, fadeDuration));
    }

    private IEnumerator FadeIn()
    {
        if (fadePanel == null || fadeImage == null) yield break;

        yield return StartCoroutine(Fade(1f, 0f, fadeDuration));
        fadePanel.SetActive(false);
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(from, to, elapsed / duration);
            SetAlpha(alpha);
            yield return null;
        }
    }

    private void SetAlpha(float alpha)
    {
        if (fadeImage != null)
        {
            Color color = fadeImage.color;
            color.a = alpha;
            fadeImage.color = color;
        }
    }

    public void CompleteFirstDay()
    {
        isFirstDay = false;
    }

    public void CompleteFirstDialogue()
    {
        isCompletedFirstDialogue = true;
    }

    public bool IsFirstDay() => isFirstDay;
    public bool IsCompletedFirstDialogue() => isCompletedFirstDialogue;
    public bool HasReturnedToHome() => hasReturnedToHome;
}