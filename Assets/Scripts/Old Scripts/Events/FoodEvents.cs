using System;

public static class FoodEvents
{
    public static event Action<FoodChoice> OnFoodChosen;
    public static event Action OnFoodPanelClosed;

    public static void TriggerFoodChosen(FoodChoice foodChoice)
    {
        OnFoodChosen?.Invoke(foodChoice);
    }

    public static void TriggerFoodPanelClosed()
    {
        OnFoodPanelClosed?.Invoke();
    }
}

public enum FoodChoice
{
    Healthy,
    Junk
}