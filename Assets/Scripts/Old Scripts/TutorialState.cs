using UnityEngine;

public class TutorialState
{
    private bool arrowActive = false;
    private Vector2 arrowPosition = Vector2.zero;
    private float arrowRotation = 0f;

    public bool ArrowActive => arrowActive;
    public Vector2 ArrowPosition => arrowPosition;
    public float ArrowRotation => arrowRotation;

    public void SetArrowActive(bool active)
    {
        arrowActive = active;
    }

    public void SetArrowPosition(Vector2 position)
    {
        arrowPosition = position;
    }

    public void SetArrowRotation(float rotation)
    {
        arrowRotation = rotation;
    }

    public void Reset()
    {
        arrowActive = false;
        arrowPosition = Vector2.zero;
        arrowRotation = 0f;
    }
}