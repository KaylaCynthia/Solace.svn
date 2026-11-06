using UnityEngine;

[System.Serializable]
public class PlayerStats
{
    [Header("Core Stats")]
    [Range(0, 100)] public float health = 100f;
    [Range(0, 100)] public float stress = 0f;
    public int cash = 0;

    [Header("Daily Limits")]
    public int actionsPerformedToday = 0;
    public int maxActionsPerDay = 3;

    public bool CanPerformAction()
    {
        return actionsPerformedToday < maxActionsPerDay;
    }

    public void ResetDailyStats()
    {
        actionsPerformedToday = 0;
    }

    public void ModifyHealth(float amount)
    {
        health = Mathf.Clamp(health + amount, 0f, 100f);
    }

    public void ModifyStress(float amount)
    {
        stress = Mathf.Clamp(stress + amount, 0f, 100f);
    }

    public void ModifyCash(int amount)
    {
        cash = Mathf.Max(0, cash + amount);
    }
}