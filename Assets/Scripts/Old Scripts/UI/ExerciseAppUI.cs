using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExerciseAppUI : BaseAppUI
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI effectsText;
    [SerializeField] private Button performExerciseButton;

    [Header("Exercise Effects")]
    [SerializeField]
    private ActionEffect exerciseEffect = new ActionEffect
    {
        actionName = "Exercise",
        healthChange = 10f,
        stressChange = -15f,
        cashChange = 0
    };

    protected override void OnEnable()
    {
        base.OnEnable();

        if (performExerciseButton != null)
            performExerciseButton.onClick.AddListener(PerformExercise);

        RefreshUI();
    }

    protected override void OnDisable()
    {
        if (performExerciseButton != null)
            performExerciseButton.onClick.RemoveListener(PerformExercise);

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
            effectsText.text = $"Exercise Effects:\n" +
                              $"+{exerciseEffect.healthChange}% Health\n" +
                              $"{exerciseEffect.stressChange}% Stress";
        }
    }

    private void UpdateButtonState()
    {
        if (performExerciseButton != null)
        {
            PlayerStats stats = StatsManager.GetInstance().GetCurrentStats();
            bool isNight = TimeManager.GetInstance().CurrentTime == TimeManager.TimeOfDay.Night;
            bool hasActionsLeft = stats.actionsPerformedToday < stats.maxActionsPerDay;

            performExerciseButton.interactable = hasActionsLeft && !isNight;

            //Debug.Log($"Exercise Button - Actions: {stats.actionsPerformedToday}/{stats.maxActionsPerDay}, " +
            //         $"Is Night: {isNight}, Interactable: {performExerciseButton.interactable}");
        }
    }

    private void PerformExercise()
    {
        bool success = StatsManager.GetInstance().PerformAction(exerciseEffect);
        if (success)
        {
            AudioCollection.GetInstance().PlaySFX(AudioCollection.GetInstance().buttonClick);
            Debug.Log("Exercise completed!");
            AppSystemManager.GetInstance().ReturnToHomeScreen();
        }
        else
        {
            Debug.LogWarning("Exercise failed - no actions left or night time");
            UpdateButtonState();
        }
    }

    protected override void OnStatsChanged(PlayerStats stats)
    {
        //Debug.Log("ExerciseApp: Stats changed, updating button");
        UpdateButtonState();
    }

    protected override void OnTimeChanged(TimeManager.TimeOfDay time)
    {
        //Debug.Log($"ExerciseApp: Time changed to {time}, updating button");
        UpdateButtonState();
    }
}