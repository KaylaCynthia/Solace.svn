using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class TextSpeedManager : MonoBehaviour
{
    static float textSpeed = 1.0f; // Default text speed multiplier
    private static readonly float[] textSpeedOptions = { 1f, 2f, 4f, 8f}; // Available text speed options
    private static readonly int textSpeedOptionCount = textSpeedOptions.Length; // Available text speed options
    static int currentSpeedIndex = 0;

    [SerializeField] private List<Button> increaseSpeedButton;
    [SerializeField] private List<Button> decreaseSpeedButton;
    [SerializeField] private List<TMP_Text> speedDisplayText;

    public static float GetSpeedMultiplier()
    {
        return textSpeedOptions[currentSpeedIndex];
    }

    public void IncreaseTextSpeed()
    {
        currentSpeedIndex = (currentSpeedIndex + 1) % textSpeedOptionCount;
        textSpeed = textSpeedOptions[currentSpeedIndex];
        UpdateSpeedDisplay();
    }

    public void DecreaseTextSpeed()
    {
        currentSpeedIndex = (currentSpeedIndex - 1 + textSpeedOptionCount) % textSpeedOptionCount;
        textSpeed = textSpeedOptions[currentSpeedIndex];
        UpdateSpeedDisplay();
    }

    void Awake()
    {
        foreach (var button in increaseSpeedButton)
        {
            button.onClick.AddListener(IncreaseTextSpeed);
        }
        foreach (var button in decreaseSpeedButton)
        {
            button.onClick.AddListener(DecreaseTextSpeed);
        }
        UpdateSpeedDisplay();
    }

    void UpdateSpeedDisplay()
    {
        Debug.Log("Updating speed display to: " + textSpeedOptions[currentSpeedIndex] + "x");
        foreach (var text in speedDisplayText)
        {
            text.text = textSpeedOptions[currentSpeedIndex] + "x";
        }
    }
}
