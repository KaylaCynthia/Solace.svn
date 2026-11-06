using System;

public static class TimeEvents
{
    public static System.Action<TimeManager.TimeOfDay> OnTimeChanged;
    public static void TriggerTimeChanged(TimeManager.TimeOfDay time) => OnTimeChanged?.Invoke(time);
}