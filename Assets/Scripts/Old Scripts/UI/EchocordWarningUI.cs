using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EchocordWarningUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject warningPanel;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private TextMeshProUGUI warningText;

    private System.Action onConfirm;
    private System.Action onCancel;

    private void Awake()
    {
        warningPanel.SetActive(false);

        if (confirmButton != null)
            confirmButton.onClick.AddListener(OnConfirm);

        if (cancelButton != null)
            cancelButton.onClick.AddListener(OnCancel);
    }

    public void ShowWarning(System.Action confirmCallback, System.Action cancelCallback = null)
    {
        onConfirm = confirmCallback;
        onCancel = cancelCallback;

        warningPanel.SetActive(true);

        string warningMessage = "Are you sure you want to enter Echocord? Once you enter, " +
                               "you'll be engaged in conversations until the next day.";
        warningText.text = warningMessage;
    }

    private void OnConfirm()
    {
        warningPanel.SetActive(false);
        onConfirm?.Invoke();
    }

    private void OnCancel()
    {
        warningPanel.SetActive(false);
        onCancel?.Invoke();
    }

    public void Hide()
    {
        warningPanel.SetActive(false);
    }
}