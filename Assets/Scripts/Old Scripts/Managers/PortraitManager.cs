using System.Collections.Generic;
using UnityEngine;

public class PortraitManager : MonoBehaviour
{
    [System.Serializable]
    public class PortraitEntry
    {
        public string portraitName;
        public Sprite portraitSprite;
    }

    [Header("Portrait Sprites")]
    [SerializeField] private List<PortraitEntry> portraitEntries = new List<PortraitEntry>();
    [SerializeField] private Sprite defaultPortrait;

    private Dictionary<string, Sprite> portraitDictionary = new Dictionary<string, Sprite>();

    private static PortraitManager instance;
    public static PortraitManager GetInstance() => instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        InitializePortraitDictionary();
    }

    private void InitializePortraitDictionary()
    {
        portraitDictionary.Clear();

        foreach (var entry in portraitEntries)
        {
            if (!string.IsNullOrEmpty(entry.portraitName) && entry.portraitSprite != null)
            {
                portraitDictionary[entry.portraitName] = entry.portraitSprite;
            }
        }

        Debug.Log($"PortraitManager initialized with {portraitDictionary.Count} portraits");
    }

    public Sprite GetPortrait(string portraitName)
    {
        if (string.IsNullOrEmpty(portraitName))
        {
            Debug.LogWarning("Portrait name is null or empty");
            return defaultPortrait;
        }

        if (portraitDictionary.ContainsKey(portraitName))
        {
            return portraitDictionary[portraitName];
        }
        else
        {
            Debug.LogWarning($"Portrait not found: {portraitName}. Using default portrait.");
            return defaultPortrait;
        }
    }

    public bool HasPortrait(string portraitName)
    {
        return portraitDictionary.ContainsKey(portraitName);
    }

    public void AddPortrait(string portraitName, Sprite portraitSprite)
    {
        if (string.IsNullOrEmpty(portraitName) || portraitSprite == null)
        {
            Debug.LogWarning("Cannot add portrait - name or sprite is null");
            return;
        }

        portraitDictionary[portraitName] = portraitSprite;

        if (!portraitEntries.Exists(entry => entry.portraitName == portraitName))
        {
            portraitEntries.Add(new PortraitEntry { portraitName = portraitName, portraitSprite = portraitSprite });
        }
    }

    public List<string> GetAllPortraitNames()
    {
        return new List<string>(portraitDictionary.Keys);
    }
}