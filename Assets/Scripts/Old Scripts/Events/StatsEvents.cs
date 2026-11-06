using System;

public static class StatsEvents
{
    public static System.Action<PlayerStats> OnStatsChanged;
    public static void TriggerStatsChanged(PlayerStats stats) => OnStatsChanged?.Invoke(stats);
}