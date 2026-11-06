using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    [SerializeField] private PlayerData playerData = new PlayerData();

    private static PlayerDataManager instance;
    public static PlayerDataManager GetInstance() => instance;

    public PlayerData PlayerData => playerData;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        playerData.Reset();
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    public void SetPlayerName(string newName)
    {
        if (PlayerData.IsValidPlayerName(newName))
        {
            playerData.PlayerName = newName;
        }
    }

    //private void SavePlayerData()
    //{
    //    PlayerPrefs.SetString("PlayerName", playerData.PlayerName);
    //    PlayerPrefs.Save();
    //}

    //private void LoadPlayerData()
    //{
    //    if (PlayerPrefs.HasKey("PlayerName"))
    //    {
    //        string savedName = PlayerPrefs.GetString("PlayerName");
    //        if (PlayerData.IsValidPlayerName(savedName))
    //        {
    //            playerData.PlayerName = savedName;
    //        }
    //    }
    //}

    public void ResetPlayerData()
    {
        playerData.Reset();
        PlayerPrefs.DeleteKey("PlayerName");
    }
}