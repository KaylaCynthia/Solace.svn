using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AppSystemManager : MonoBehaviour
{
    [System.Serializable]
    public class AppInfo
    {
        public string appName;
        public GameObject appPanel;
        public Button appButton;
        public AppType appType;
    }

    public enum AppType
    {
        Echocord,
        Settings,
        MiniGame,
        Exercise,
        Work,
        Sleep,
        Bank,
        Other
    }

    [Header("Home Screen")]
    [SerializeField] private GameObject homeScreen;

    [Header("All Applications")]
    [SerializeField] private List<AppInfo> applications = new List<AppInfo>();
    [SerializeField] private EchocordWarningUI warningPanel;

    [Header("Dependencies")]
    [SerializeField] private GameStateManager gameStateManager;

    private GameObject currentOpenApp;
    private AppType currentAppType;
    private bool isFirstLaunch = true;

    private static AppSystemManager instance;
    public static AppSystemManager GetInstance() => instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        InitializeAllApps();
        UpdateAllAppButtons();
        TimeEvents.OnTimeChanged += OnTimeChanged;
    }

    private void OnDestroy()
    {
        TimeEvents.OnTimeChanged -= OnTimeChanged;
    }

    private void InitializeAllApps()
    {
        foreach (AppInfo app in applications)
        {
            if (app.appButton != null)
                app.appButton.onClick.AddListener(() => LaunchApp(app.appName));
        }
    }

    private void OnTimeChanged(TimeManager.TimeOfDay time)
    {
        UpdateAllAppButtons();
    }

    private void UpdateAllAppButtons()
    {
        bool isNight = TimeManager.GetInstance().CurrentTime == TimeManager.TimeOfDay.Night;
        bool isAppOpen = currentOpenApp != null;

        foreach (AppInfo app in applications)
        {
            if (app.appButton != null)
            {
                bool shouldBeInteractable = (app.appType == AppType.Echocord || !isNight) && !isAppOpen;
                app.appButton.interactable = shouldBeInteractable;
            }
        }
    }

    private bool ShouldShowEchocordWarning()
    {
        FirstDayManager firstDayManager = FirstDayManager.GetInstance();
        if (firstDayManager == null) return false;

        return !firstDayManager.IsFirstDay() || firstDayManager.HasReturnedToHome();
    }

    public void LaunchEchocordImmediately()
    {
        AppInfo echocordApp = applications.Find(app => app.appType == AppType.Echocord);

        if (echocordApp != null)
        {
            Debug.Log("Launching Echocord immediately after day transition");
            LaunchAppInternal(echocordApp);
        }
        else
        {
            Debug.LogError("Echocord app not found in applications list!");
        }
    }

    public void LaunchApp(string appName)
    {
        AudioCollection.GetInstance().PlaySFX(AudioCollection.GetInstance().buttonClick);
        AppInfo targetApp = applications.Find(app => app.appName == appName);

        if (targetApp == null)
        {
            Debug.LogWarning($"App not found: {appName}");
            return;
        }

        bool isNight = TimeManager.GetInstance().CurrentTime == TimeManager.TimeOfDay.Night;
        if (isNight && targetApp.appType != AppType.Echocord)
        {
            Debug.LogWarning($"Cannot launch {appName} at night time!");
            return;
        }

        if (currentOpenApp != null && currentOpenApp != targetApp.appPanel)
        {
            Debug.LogWarning($"Cannot launch {appName} because {currentOpenApp.name} is already open!");
            return;
        }

        if (targetApp.appType == AppType.Echocord && ShouldShowEchocordWarning())
        {
            ShowEchocordWarning(targetApp);
            return;
        }

        LaunchAppInternal(targetApp);
    }

    private void ShowEchocordWarning(AppInfo echocordApp)
    {
        if (warningPanel != null)
        {
            warningPanel.ShowWarning(
                () => LaunchAppInternal(echocordApp),
                () => Debug.Log("Echocord launch cancelled")
            );
        }
        else
        {
            Debug.LogWarning("EchocordWarningPanel not found - launching Echocord directly");
            LaunchAppInternal(echocordApp);
        }
    }

    private void LaunchAppInternal(AppInfo targetApp)
    {
        Debug.Log($"Launching {targetApp.appName} as {(IsOverlayApp(targetApp.appType) ? "OVERLAY" : "FULLSCREEN")} app");

        if (currentOpenApp != null && currentOpenApp != targetApp.appPanel)
        {
            CloseCurrentApp();
        }

        if (!IsOverlayApp(targetApp.appType))
        {
            homeScreen.SetActive(false);
        }

        targetApp.appPanel.SetActive(true);
        currentOpenApp = targetApp.appPanel;
        currentAppType = targetApp.appType;

        UpdateAllAppButtons();

        HandleAppSpecificInitialization(targetApp);
    }

    private bool IsOverlayApp(AppType appType)
    {
        return appType == AppType.Exercise ||
               appType == AppType.Work ||
               appType == AppType.Sleep ||
               appType == AppType.Bank;
    }

    private void HandleAppSpecificInitialization(AppInfo app)
    {
        switch (app.appType)
        {
            case AppType.Echocord:
                if (!isFirstLaunch)
                {
                    gameStateManager.StartConversationForCurrentDay();
                }
                else
                {
                    StartCoroutine(StartFirstConversationDelayed());
                }
                break;

            case AppType.Exercise:
            case AppType.Work:
            case AppType.Sleep:
            case AppType.Bank:
                //Debug.Log($"{app.appName} overlaying on home screen");
                break;
        }

        Debug.Log($"Launched {app.appName} ({app.appType}) - Home screen active: {homeScreen.activeSelf}");
    }

    private IEnumerator StartFirstConversationDelayed()
    {
        yield return new WaitForSeconds(0.1f);
        GameStateManager.GetInstance().StartFirstConversation();
        isFirstLaunch = false;
    }

    public void ReturnToHomeScreen()
    {
        if (currentOpenApp == null) return;

        Debug.Log($"Returning to home screen from {currentAppType}");

        CloseCurrentApp();
        homeScreen.SetActive(true);
        UpdateAllAppButtons();
    }

    private void CloseCurrentApp()
    {
        if (currentOpenApp != null)
        {
            currentOpenApp.SetActive(false);
            currentOpenApp = null;
            currentAppType = AppType.Other;
        }
    }

    public bool IsAppOpen => currentOpenApp != null;

    public void RegisterApp(string appName, GameObject appPanel, Button appButton, AppType appType)
    {
        applications.Add(new AppInfo
        {
            appName = appName,
            appPanel = appPanel,
            appButton = appButton,
            appType = appType
        });

        appPanel.SetActive(false);
        appButton.onClick.AddListener(() => LaunchApp(appName));

        UpdateAllAppButtons();
    }
}