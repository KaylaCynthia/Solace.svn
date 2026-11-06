using System;

public static class ActionEvents
{
    public static System.Action<ActionEffect> OnActionPerformed;
    public static void TriggerActionPerformed(ActionEffect action) => OnActionPerformed?.Invoke(action);
}