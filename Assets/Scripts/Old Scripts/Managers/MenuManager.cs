using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject creditsPanel;

    private static MenuManager instance;
    public static MenuManager GetInstance() => instance;
    private bool isSettings = false;
    private bool isCredits = false;

    private AudioCollection audioCollection;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        audioCollection = FindFirstObjectByType<AudioCollection>();
        audioCollection.PlayBGM(audioCollection.mainMenu);
    }

    public void StartGame()
    {
        AudioCollection.GetInstance()?.PlaySFX(AudioCollection.GetInstance().buttonClick);
        SceneManager.LoadScene("GameContent");
    }

    public void Settings()
    {
        AudioCollection.GetInstance()?.PlaySFX(AudioCollection.GetInstance().buttonClick);
        if (settingsPanel == null) return;

        isSettings = !isSettings;
        settingsPanel.SetActive(isSettings);
    }

    public void Credits()
    {
        AudioCollection.GetInstance()?.PlaySFX(AudioCollection.GetInstance().buttonClick);
        if (creditsPanel == null) return;

        isCredits = !isCredits;
        creditsPanel.SetActive(isCredits);
    }

    public void QuitGame()
    {
        AudioCollection.GetInstance()?.PlaySFX(AudioCollection.GetInstance().buttonClick);
        Application.Quit();
    }
}