using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private NameDataUI nameDataUI;
    [SerializeField] private GameObject toBeContinuedPanel;

    [Header("Dependencies")]
    [SerializeField] private DayManager dayManager;
    [SerializeField] private AppSystemManager appSystemManager;
    [SerializeField] private GameStateManager gameStateManager;

    private bool isPaused = false;
    private bool isSettings = false;
    private string currentSceneName;
    private bool hasPlayerSetName = false;

    private static GameManager instance;
    public static GameManager GetInstance() => instance;

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

    private void Start()
    {
        currentSceneName = SceneManager.GetActiveScene().name;
        InitializeDaySystem();
        InitializeAdditionalManagers();

        ShowNameInputPanel();
    }

    private void InitializeAdditionalManagers()
    {
        if (FirstDayManager.GetInstance() == null)
        {
            GameObject firstDayObj = new GameObject("FirstDayManager");
            firstDayObj.transform.SetParent(transform);
            firstDayObj.AddComponent<FirstDayManager>();
        }
    }

    private void ShowNameInputPanel()
    {
        if (nameDataUI != null)
        {
            nameDataUI.Show(OnNameConfirmed, OnNamePanelClosed);
        }
    }

    private void OnNameConfirmed()
    {
        audioCollection.PlaySFX(audioCollection.buttonClick);
        hasPlayerSetName = true;
        StartCoroutine(StartGameWithTransition());
    }

    private void OnNamePanelClosed()
    {
        if (!hasPlayerSetName)
        {
            UseDefaultNameAndStart();
        }
    }

    private void UseDefaultNameAndStart()
    {
        PlayerDataManager.GetInstance().SetPlayerName("Player");
        hasPlayerSetName = true;
        StartCoroutine(StartGameWithTransition());
    }

    private void InitializeDaySystem()
    {
        if (dayManager != null)
        {
            dayManager.SetDay(1);
        }
    }

    private IEnumerator StartGameWithTransition()
    {
        if (dayManager != null)
        {
            yield return StartCoroutine(dayManager.ShowFirstDayTransition());
        }

        yield return null;

        if (appSystemManager != null)
        {
            appSystemManager.LaunchEchocordImmediately();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }

    private void Pause()
    {
        if (pausePanel == null) return;

        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void Resume()
    {
        if (pausePanel == null) return;

        audioCollection.PlaySFX(audioCollection.buttonClick);
        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Settings()
    {
        if (settingsPanel == null) return;

        audioCollection.PlaySFX(audioCollection.buttonClick);
        isSettings = !isSettings;
        settingsPanel.SetActive(isSettings);
    }

    public void LoadMainMenu()
    {
        audioCollection.PlaySFX(audioCollection.buttonClick);
        ForceCompleteCleanup();
        SceneManager.LoadScene("MainMenu");
    }

    public void ShowToBeContinuedPanel()
    {
        if (toBeContinuedPanel != null)
        {
            Debug.Log("Showing To Be Continued panel");
            toBeContinuedPanel.SetActive(true);

            Time.timeScale = 0f;
        }
        else
        {
            Debug.LogError("ToBeContinuedPanel reference is not set in GameManager!");
        }
    }

    public void HideToBeContinuedPanel()
    {
        if (toBeContinuedPanel != null)
        {
            toBeContinuedPanel.SetActive(false);
        }
    }

    public void ReturnToMainMenuFromTBC()
    {
        HideToBeContinuedPanel();
        LoadMainMenu();
    }

    private void ForceCompleteCleanup()
    {
        ChatDialogueManager chatManager = ChatDialogueManager.GetInstance();
        if (chatManager != null)
        {
            chatManager.GetComponent<ChatDialogueManager>()?.GetChatAreaManager()?.CurrentChatArea?.Clear();
        }

        DestroyManagerInstance<ChatDialogueManager>();
        DestroyManagerInstance<StatsManager>();
        DestroyManagerInstance<DayManager>();
        DestroyManagerInstance<PlayerDataManager>();
        DestroyManagerInstance<ChatAreaButtonManager>();
        DestroyManagerInstance<FirstDayManager>();
        DestroyManagerInstance<TimeManager>();
        DestroyManagerInstance<FoodChoiceManager>();
        DestroyManagerInstance<ServerManager>();
        DestroyManagerInstance<ServerLockManager>();
        DestroyManagerInstance<ChatNotificationManager>();

        Time.timeScale = 1f;

        StopAllCoroutines();
    }

    private void DestroyManagerInstance<T>() where T : MonoBehaviour
    {
        T managerInstance = FindFirstObjectByType<T>();
        if (managerInstance != null && managerInstance.gameObject != this.gameObject)
        {
            Destroy(managerInstance.gameObject);
        }
    }

    public void ShowNameChangePanel()
    {
        if (nameDataUI != null)
        {
            nameDataUI.Show();
        }
    }

    public bool HasPlayerSetName()
    {
        return hasPlayerSetName;
    }

    public string GetPlayerName()
    {
        return PlayerDataManager.GetInstance().PlayerData.PlayerName;
    }
}