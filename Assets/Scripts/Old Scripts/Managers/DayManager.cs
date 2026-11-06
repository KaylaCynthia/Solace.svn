using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DayManager : MonoBehaviour
{
    [Header("Day Transition UI")]
    [SerializeField] private GameObject dayTransitionPanel;
    [SerializeField] private Image fadeImage;
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private float transitionDuration = 3f;

    public float TransitionDuration => transitionDuration;

    private int currentDay = 1;
    private bool isTransitioning = false;
    private bool isFirstTransition = true;

    public System.Action OnTransitionComplete;

    private static DayManager instance;
    public static DayManager GetInstance() => instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    public void StartNextDay()
    {
        if (isTransitioning) return;

        currentDay++;
        StartCoroutine(DayTransitionCoroutine(false));
    }

    public IEnumerator ShowFirstDayTransition()
    {
        if (isTransitioning) yield break;

        yield return StartCoroutine(DayTransitionCoroutine(true));
        isFirstTransition = false;
    }

    public void SetDay(int day)
    {
        currentDay = day;
        dayText.text = $"Day {currentDay}";
    }

    public int GetCurrentDay() => currentDay;

    public void ResetDailyStats()
    {
        TimeManager.GetInstance()?.ResetToMorning();

        StatsManager statsManager = StatsManager.GetInstance();
        if (statsManager != null)
        {
            statsManager.ResetDailyStats();
            Debug.Log($"Daily stats reset - actions: {statsManager.GetCurrentStats().actionsPerformedToday}/{statsManager.GetCurrentStats().maxActionsPerDay}");
        }
        else
        {
            Debug.LogError("StatsManager not found when resetting daily stats!");
        }
    }

    private IEnumerator DayTransitionCoroutine(bool isFirstTransition)
    {
        isTransitioning = true;

        dayTransitionPanel.SetActive(true);
        dayText.text = $"Day {currentDay}";

        if (isFirstTransition)
        {
            SetAlpha(1f);
            yield return new WaitForSeconds(0.5f);
            yield return StartCoroutine(Fade(1f, 0f, transitionDuration));
        }
        else
        {
            SetAlpha(0f);
            yield return StartCoroutine(Fade(0f, 1f, transitionDuration / 2f));
            yield return new WaitForSeconds(0.1f);
            AppSystemManager appSystemManager = AppSystemManager.GetInstance();
            if (appSystemManager != null && appSystemManager.IsAppOpen)
            {
                appSystemManager.ReturnToHomeScreen();
            }
            yield return new WaitForSeconds(0.5f);
            yield return StartCoroutine(Fade(1f, 0f, transitionDuration / 2f));
        }

        dayTransitionPanel.SetActive(false);
        isTransitioning = false;

        ResetDailyStats();

        DayEvents.TriggerDayChanged(currentDay);

        OnTransitionComplete?.Invoke();
    }

    private void SetAlpha(float alpha)
    {
        Color color = fadeImage.color;
        color.a = alpha;
        fadeImage.color = color;

        Color textColor = dayText.color;
        textColor.a = alpha;
        dayText.color = textColor;
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
}