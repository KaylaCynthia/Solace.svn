using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NameDataUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject namePanel;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private Button confirmButton;
    [SerializeField] private TextMeshProUGUI errorText;

    [Header("Validation Settings")]
    [SerializeField] private int minNameLength = 3;
    [SerializeField] private int maxNameLength = 12;

    private System.Action onNameConfirmed;
    private System.Action onPanelClosed;

    private void Awake()
    {
        InitializeUIComponents();
    }

    private void InitializeUIComponents()
    {
        if (nameInputField != null)
        {
            nameInputField.characterLimit = maxNameLength;
            nameInputField.onValueChanged.AddListener(OnNameInputChanged);
        }

        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(OnConfirmButtonClicked);
        }

        if (errorText != null)
        {
            errorText.gameObject.SetActive(false);
        }
    }

    public void Show(System.Action onConfirmed = null, System.Action onClosed = null)
    {
        onNameConfirmed = onConfirmed;
        onPanelClosed = onClosed;

        if (nameInputField != null)
        {
            nameInputField.text = PlayerDataManager.GetInstance().PlayerData.PlayerName;
        }

        if (errorText != null)
        {
            errorText.gameObject.SetActive(false);
        }

        UpdateConfirmButtonState();
        namePanel.SetActive(true);
    }

    public void Hide()
    {
        namePanel.SetActive(false);
        ClearInput();
    }

    private void OnNameInputChanged(string input)
    {
        UpdateConfirmButtonState();
        ValidateInput(input);
    }

    private void UpdateConfirmButtonState()
    {
        if (confirmButton == null) return;

        string input = nameInputField?.text ?? "";
        bool isValid = PlayerData.IsValidPlayerName(input);
        confirmButton.interactable = isValid;
    }

    private void ValidateInput(string input)
    {
        if (errorText == null) return;

        if (string.IsNullOrEmpty(input))
        {
            errorText.gameObject.SetActive(false);
            return;
        }

        if (input.Length < minNameLength)
        {
            ShowError($"Name must be at least {minNameLength} characters long.");
        }
        else if (input.Length > maxNameLength)
        {
            ShowError($"Name must be no more than {maxNameLength} characters long.");
        }
        else if (input.Contains(" "))
        {
            ShowError("Name cannot contain spaces.");
        }
        else if (!IsAllLetters(input))
        {
            ShowError("Name can only contain letters.");
        }
        else
        {
            errorText.gameObject.SetActive(false);
        }
    }

    private bool IsAllLetters(string input)
    {
        foreach (char c in input)
        {
            if (!char.IsLetter(c)) return false;
        }
        return true;
    }

    private void ShowError(string message)
    {
        if (errorText != null)
        {
            errorText.text = message;
            errorText.gameObject.SetActive(true);
        }
    }

    private void OnConfirmButtonClicked()
    {
        string inputName = nameInputField?.text?.Trim() ?? "";

        if (PlayerData.IsValidPlayerName(inputName))
        {
            PlayerDataManager.GetInstance().SetPlayerName(inputName);

            onNameConfirmed?.Invoke();

            Hide();
        }
        else
        {
            ShowError("Please enter a valid name.");
        }
    }

    private void ClearInput()
    {
        if (nameInputField != null)
        {
            nameInputField.text = "";
        }
        if (errorText != null)
        {
            errorText.gameObject.SetActive(false);
        }
    }

    public static bool ValidatePlayerName(string name)
    {
        return PlayerData.IsValidPlayerName(name);
    }
}