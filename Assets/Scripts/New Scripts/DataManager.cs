using UnityEngine;
using Ink.UnityIntegration;

public class DataManager : MonoBehaviour
{
    [SerializeField] GameObject textMessagePrefab;
    public static DataManager Instance;
    [SerializeField] private ProfilePictureDatabase profilePictureDatabase;
    [SerializeField] private InkDatabase inkDatabase;

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
    
    public InkFile GetInkFile(string inkName)
    {
        return inkDatabase.GetInkFile(inkName);
    }

    public GameObject GetTextMessagePrefab()
    {
        return textMessagePrefab;
    }
}
