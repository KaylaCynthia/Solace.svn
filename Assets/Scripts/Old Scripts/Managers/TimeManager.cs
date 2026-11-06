using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public enum TimeOfDay { Morning, Afternoon, Evening, Night }

    [Header("Time Settings")]
    [SerializeField] private TimeOfDay currentTime = TimeOfDay.Morning;

    public TimeOfDay CurrentTime => currentTime;

    private static TimeManager instance;
    public static TimeManager GetInstance() => instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        TimeEvents.TriggerTimeChanged(currentTime);
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    public void AdvanceTime()
    {
        switch (currentTime)
        {
            case TimeOfDay.Morning: currentTime = TimeOfDay.Afternoon; break;
            case TimeOfDay.Afternoon: currentTime = TimeOfDay.Evening; break;
            case TimeOfDay.Evening: currentTime = TimeOfDay.Night; break;
            case TimeOfDay.Night: currentTime = TimeOfDay.Morning; break;
        }

        Debug.Log($"Time advanced to: {currentTime}");
        TimeEvents.TriggerTimeChanged(currentTime);
    }

    public void ResetToMorning()
    {
        currentTime = TimeOfDay.Morning;
        TimeEvents.TriggerTimeChanged(currentTime);
    }

    public string GetTimeDisplayName()
    {
        return currentTime.ToString();
    }
}