using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorkAppUI : BaseAppUI
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI effectsText;
    [SerializeField] private Button performWorkButton;

    [Header("Work Effects")]
    [SerializeField]
    private ActionEffect workEffect = new ActionEffect
    {
        actionName = "Work",
        healthChange = -10f,
        cashChange = 120
    };

    protected override void OnEnable()
    {
        base.OnEnable();

        if (performWorkButton != null)
            performWorkButton.onClick.AddListener(PerformWork);

        UpdateUI();
        UpdateButtonState();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (performWorkButton != null)
            performWorkButton.onClick.RemoveListener(PerformWork);
    }

    private void UpdateUI()
    {
        if (effectsText != null)
        {
            effectsText.text = $"Work Effects:\n" +
                              $"{workEffect.healthChange}% Health\n" +
                              $"+{workEffect.stressChange}% Stress\n" +
                              $"+${workEffect.cashChange} Cash";
        }

        UpdateButtonState();
    }

    private void UpdateButtonState()
    {
        if (performWorkButton != null)
        {
            PlayerStats stats = StatsManager.GetInstance().GetCurrentStats();
            bool isNight = TimeManager.GetInstance().CurrentTime == TimeManager.TimeOfDay.Night;
            bool hasActionsLeft = stats.actionsPerformedToday < stats.maxActionsPerDay;

            performWorkButton.interactable = hasActionsLeft && !isNight;

            //Debug.Log($"Work Button State - Actions Left: {hasActionsLeft}, Is Night: {isNight}, Interactable: {performWorkButton.interactable}");
        }
    }

    private void PerformWork()
    {
        bool success = StatsManager.GetInstance().PerformAction(workEffect);
        if (success)
        {
            Debug.Log("Work completed!");
            AudioCollection.GetInstance().PlaySFX(AudioCollection.GetInstance().buttonClick);
            AppSystemManager.GetInstance().ReturnToHomeScreen();
        }
        else
        {
            Debug.LogWarning("Could not perform work - no actions left or it's night time");
        }
    }

    protected override void OnStatsChanged(PlayerStats stats)
    {
        //Debug.Log("WorkAppUI: Stats changed, updating button state");
        UpdateButtonState();
    }

    protected override void OnTimeChanged(TimeManager.TimeOfDay time)
    {
        //Debug.Log($"WorkAppUI: Time changed to {time}, updating button state");
        UpdateButtonState();
    }
}