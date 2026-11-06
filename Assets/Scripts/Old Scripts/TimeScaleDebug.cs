using UnityEngine;
using UnityEngine.UI;

public class TimeScaleDebug : MonoBehaviour
{
    public Button normalTimeScale;
    public Button button100x;

    private void Start()
    {
        normalTimeScale.onClick.AddListener(SetNormalTimeScale);
        button100x.onClick.AddListener(Set100xTimeScale);
    }

    private void SetNormalTimeScale()
    {
        Time.timeScale = 1f;
    }

    private void Set100xTimeScale()
    {
        Time.timeScale = 100f;
    }
}
