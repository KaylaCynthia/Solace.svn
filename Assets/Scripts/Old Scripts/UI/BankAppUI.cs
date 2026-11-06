using TMPro;
using UnityEngine;

public class BankAppUI : BaseAppUI
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI cashBalanceText;

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void RefreshUI()
    {
        UpdateBankDisplay();
    }

    private void UpdateBankDisplay()
    {
        if (cashBalanceText != null)
        {
            PlayerStats currentStats = StatsManager.GetInstance().GetCurrentStats();
            cashBalanceText.text = $"${currentStats.cash}";
        }
    }

    protected override void OnStatsChanged(PlayerStats stats)
    {
        UpdateBankDisplay();
    }
}