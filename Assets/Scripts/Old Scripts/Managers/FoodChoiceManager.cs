using UnityEngine;

public class FoodChoiceManager : MonoBehaviour
{
    [Header("Food Panel Reference")]
    [SerializeField] private GameObject foodPanel;

    [Header("Food Effects")]
    [SerializeField] private float healthyFoodHealthGain;
    [SerializeField] private float healthyFoodStressGain;
    [SerializeField] private int healthyFoodCashChange;
    [SerializeField] private float junkFoodHealthLoss;
    [SerializeField] private float junkFoodStressLoss;
    [SerializeField] private int junkFoodCashChange;

    private bool isFoodPanelActive = false;
    private DayManager dayManager;

    private static FoodChoiceManager instance;
    public static FoodChoiceManager GetInstance() => instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        dayManager = DayManager.GetInstance();
        HideFoodPanel();

        DayEvents.OnDayChanged += OnDayChanged;
    }

    private void OnDestroy()
    {
        DayEvents.OnDayChanged -= OnDayChanged;
        if (instance == this)
        {
            instance = null;
        }
    }

    private void OnDayChanged(int dayNumber)
    {
        if (dayNumber >= 2)
        {
            ShowFoodPanel();
        }
    }

    public void ShowFoodPanel()
    {
        foodPanel.SetActive(true);
        isFoodPanelActive = true;
        //Debug.Log("Food panel shown for day: " + dayManager.GetCurrentDay());
    }

    public void HideFoodPanel()
    {
        if (foodPanel != null)
        {
            foodPanel.SetActive(false);
        }
        isFoodPanelActive = false;
        FoodEvents.TriggerFoodPanelClosed();
    }

    public void ChooseHealthyFood()
    {
        ApplyFoodEffects(FoodChoice.Healthy);
        Debug.Log("Healthy food chosen - Health: +50%, Stress: +25%");
    }

    public void ChooseJunkFood()
    {
        ApplyFoodEffects(FoodChoice.Junk);
        Debug.Log("Junk food chosen - Health: -50%, Stress: -50%");
    }

    private void ApplyFoodEffects(FoodChoice choice)
    {
        StatsManager statsManager = StatsManager.GetInstance();
        if (statsManager == null)
        {
            Debug.LogError("StatsManager not found!");
            return;
        }

        PlayerStats currentStats = statsManager.GetCurrentStats();

        switch (choice)
        {
            case FoodChoice.Healthy:
                currentStats.ModifyHealth(healthyFoodHealthGain);
                currentStats.ModifyStress(healthyFoodStressGain);
                currentStats.ModifyCash(healthyFoodCashChange);
                break;
            case FoodChoice.Junk:
                currentStats.ModifyHealth(-junkFoodHealthLoss);
                currentStats.ModifyStress(-junkFoodStressLoss);
                currentStats.ModifyCash(junkFoodCashChange);
                break;
        }

        statsManager.SetStats(currentStats);
        FoodEvents.TriggerFoodChosen(choice);
        HideFoodPanel();
    }

    public bool IsFoodPanelActive() => isFoodPanelActive;
}