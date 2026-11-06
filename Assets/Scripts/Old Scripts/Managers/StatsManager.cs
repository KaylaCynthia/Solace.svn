using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    [SerializeField] private PlayerStats currentStats = new PlayerStats();
    [SerializeField] private List<string> transactionHistory = new List<string>();

    private static StatsManager instance;
    public static StatsManager GetInstance() => instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        InitializeDefaultStats();
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    private void InitializeDefaultStats()
    {
        currentStats.health = 60f;
        currentStats.stress = 70f;
        currentStats.cash = 500;
        currentStats.actionsPerformedToday = 0;
        currentStats.maxActionsPerDay = 3;

        StatsEvents.TriggerStatsChanged(currentStats);
    }

    public PlayerStats GetCurrentStats() => currentStats;
    public List<string> GetTransactionHistory() => new List<string>(transactionHistory);

    public bool PerformAction(ActionEffect actionEffect)
    {
        if (!currentStats.CanPerformAction())
        {
            Debug.LogWarning("Cannot perform action - daily limit reached!");
            return false;
        }

        currentStats.ModifyHealth(actionEffect.healthChange);
        currentStats.ModifyStress(actionEffect.stressChange);
        currentStats.ModifyCash(actionEffect.cashChange);

        currentStats.actionsPerformedToday++;
        TimeManager.GetInstance().AdvanceTime();

        if (actionEffect.cashChange != 0)
        {
            string transaction = $"{actionEffect.actionName}: ${actionEffect.cashChange:+0;-#}";
            transactionHistory.Add(transaction);

            if (transactionHistory.Count > 5)
                transactionHistory.RemoveAt(0);
        }

        StatsEvents.TriggerStatsChanged(currentStats);
        ActionEvents.TriggerActionPerformed(actionEffect);

        Debug.Log($"Action performed: {actionEffect.actionName}. Health: {currentStats.health}%, Stress: {currentStats.stress}%, Cash: ${currentStats.cash}");

        return true;
    }

    public void ResetDailyStats()
    {
        currentStats.ResetDailyStats();
        StatsEvents.TriggerStatsChanged(currentStats);
        Debug.Log($"StatsManager: Reset daily stats. Actions: {currentStats.actionsPerformedToday}/{currentStats.maxActionsPerDay}");
    }

    public void SetStats(PlayerStats newStats)
    {
        currentStats = newStats;
        StatsEvents.TriggerStatsChanged(currentStats);
    }
}