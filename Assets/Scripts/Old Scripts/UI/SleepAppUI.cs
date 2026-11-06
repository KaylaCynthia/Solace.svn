using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SleepAppUI : BaseAppUI
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI effectsText;
    [SerializeField] private Button sleepButton;

    [Header("Sleep Effects")]
    [SerializeField]
    private ActionEffect sleepEffect = new ActionEffect
    {
        actionName = "Sleep",
        healthChange = 20f,
        stressChange = -25f,
        cashChange = 0
    };

    protected override void OnEnable()
    {
        base.OnEnable();

        if (sleepButton != null)
            sleepButton.onClick.AddListener(PerformSleep);

        RefreshUI();
    }

    protected override void OnDisable()
    {
        if (sleepButton != null)
            sleepButton.onClick.RemoveListener(PerformSleep);

        base.OnDisable();
    }

    protected override void RefreshUI()
    {
        UpdateEffectsText();
        UpdateButtonState();
    }

    private void UpdateEffectsText()
    {
        if (effectsText != null)
        {
            effectsText.text = $"Sleep Benefits:\n" +
                                    $"+{sleepEffect.healthChange}% Health\n" +
                                    $"{sleepEffect.stressChange}% Stress";
        }
    }

    private void UpdateButtonState()
    {
        if (sleepButton != null)
        {
            PlayerStats stats = StatsManager.GetInstance().GetCurrentStats();
            bool isNight = TimeManager.GetInstance().CurrentTime == TimeManager.TimeOfDay.Night;
            bool hasActionsLeft = stats.actionsPerformedToday < stats.maxActionsPerDay;

            sleepButton.interactable = hasActionsLeft && !isNight;

            //Debug.Log($"Sleep Button - Actions: {stats.actionsPerformedToday}/{stats.maxActionsPerDay}, " +
            //         $"Is Night: {isNight}, Interactable: {sleepButton.interactable}");
        }
    }

    private void PerformSleep()
    {
        bool success = StatsManager.GetInstance().PerformAction(sleepEffect);
        if (success)
        {
            Debug.Log("Sleep completed! Stats updated.");
            AudioCollection.GetInstance().PlaySFX(AudioCollection.GetInstance().buttonClick);
            AppSystemManager.GetInstance().ReturnToHomeScreen();
        }
        else
        {
            Debug.LogWarning("Sleep failed - no actions left or night time");
            UpdateButtonState();
        }
    }

    protected override void OnStatsChanged(PlayerStats stats)
    {
        //Debug.Log("SleepApp: Stats changed, updating button");
        UpdateButtonState();
    }

    protected override void OnTimeChanged(TimeManager.TimeOfDay time)
    {
        //Debug.Log($"SleepApp: Time changed to {time}, updating button");
        UpdateButtonState();
    }
}