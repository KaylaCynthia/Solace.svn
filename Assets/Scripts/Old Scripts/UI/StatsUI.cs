using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StatsUI : MonoBehaviour
{
    [Header("Health Display")]
    [SerializeField] private Image healthFill;

    [Header("Stress Display")]
    [SerializeField] private Image stressFill;

    [Header("Time Display")]
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI dayText;

    private void OnEnable()
    {
        StatsEvents.OnStatsChanged += UpdateStatsDisplay;
        TimeEvents.OnTimeChanged += UpdateTimeDisplay;
        StartCoroutine(DelayedInitialization());
    }

    private void OnDisable()
    {
        StatsEvents.OnStatsChanged -= UpdateStatsDisplay;
        TimeEvents.OnTimeChanged -= UpdateTimeDisplay;
    }

    private IEnumerator DelayedInitialization()
    {
        yield return null;
        UpdateAllDisplays();
    }

    private void UpdateAllDisplays()
    {
        StatsManager statsManager = StatsManager.GetInstance();
        TimeManager timeManager = TimeManager.GetInstance();

        if (statsManager != null)
            UpdateStatsDisplay(statsManager.GetCurrentStats());

        if (timeManager != null)
            UpdateTimeDisplay(timeManager.CurrentTime);

        if (DayManager.GetInstance() != null)
            UpdateDayDisplay();
    }

    private void UpdateStatsDisplay(PlayerStats stats)
    {
        if (healthFill != null) healthFill.fillAmount = stats.health / 100f;

        if (stressFill != null) stressFill.fillAmount = stats.stress / 100f;
    }

    private void UpdateTimeDisplay(TimeManager.TimeOfDay time)
    {
        if (timeText != null) timeText.text = time.ToString();
    }

    private void UpdateDayDisplay()
    {
        int currentDay = DayManager.GetInstance().GetCurrentDay();
        if (dayText != null) dayText.text = $"Day {currentDay}";
    }
}