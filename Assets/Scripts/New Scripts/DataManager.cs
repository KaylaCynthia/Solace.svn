using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    [SerializeField] private ProfilePictureDatabase profilePictureDatabase;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Sprite GetProfilePicture(string profileName)
    {
        return profilePictureDatabase.GetProfilePicture(profileName);
    }
}
