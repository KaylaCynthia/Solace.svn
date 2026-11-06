using System;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    [SerializeField] private string playerName = "Player";

    public string PlayerName
    {
        get => playerName;
        set
        {
            if (IsValidPlayerName(value))
            {
                playerName = value;
                OnPlayerDataChanged?.Invoke(this);
            }
            else
            {
                Debug.LogWarning($"Invalid player name: {value}. Must be 3-12 letters with no spaces.");
            }
        }
    }

    public static event Action<PlayerData> OnPlayerDataChanged;

    public PlayerData(string name = "Player")
    {
        if (IsValidPlayerName(name))
        {
            playerName = name;
        }
    }

    public static bool IsValidPlayerName(string name)
    {
        if (string.IsNullOrEmpty(name)) return false;
        if (name.Length < 3 || name.Length > 12) return false;
        if (name.Contains(" ")) return false;

        foreach (char c in name)
        {
            if (!char.IsLetter(c)) return false;
        }

        return true;
    }

    public void Reset()
    {
        playerName = "Player";
        OnPlayerDataChanged?.Invoke(this);
    }
}