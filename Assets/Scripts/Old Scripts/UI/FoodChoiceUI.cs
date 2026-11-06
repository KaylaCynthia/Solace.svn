using UnityEngine;
using UnityEngine.UI;

public class FoodChoiceUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button healthyFoodButton;
    [SerializeField] private Button junkFoodButton;

    private FoodChoiceManager foodChoiceManager;

    private void Awake()
    {
        foodChoiceManager = FoodChoiceManager.GetInstance();

        if (healthyFoodButton != null)
            healthyFoodButton.onClick.AddListener(OnHealthyFoodChosen);

        if (junkFoodButton != null)
            junkFoodButton.onClick.AddListener(OnJunkFoodChosen);
    }

    private void OnHealthyFoodChosen()
    {
        foodChoiceManager.ChooseHealthyFood();
    }

    private void OnJunkFoodChosen()
    {
        foodChoiceManager.ChooseJunkFood();
    }

    private void OnDestroy()
    {
        if (healthyFoodButton != null)
            healthyFoodButton.onClick.RemoveListener(OnHealthyFoodChosen);

        if (junkFoodButton != null)
            junkFoodButton.onClick.RemoveListener(OnJunkFoodChosen);
    }
}