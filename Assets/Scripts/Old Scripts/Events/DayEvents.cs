using System;

public static class DayEvents
{
    public static event Action<int> OnDayChanged;

    public static void TriggerDayChanged(int newDay)
    {
        OnDayChanged?.Invoke(newDay);
    }
}