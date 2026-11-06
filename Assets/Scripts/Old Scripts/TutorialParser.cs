using System.Collections.Generic;
using UnityEngine;

public class TutorialParser
{
    public void ProcessTags(List<string> currentTags, TutorialState state)
    {
        if (currentTags == null) return;

        foreach (string tag in currentTags)
        {
            ProcessTag(tag, state);
        }
    }

    private void ProcessTag(string tag, TutorialState state)
    {
        if (tag.StartsWith("arrowactive:"))
        {
            string value = tag.Substring("arrowactive:".Length).Trim().ToLower();
            state.SetArrowActive(value == "true");
        }
        else if (tag.StartsWith("arrowcoor:"))
        {
            string coordinates = tag.Substring("arrowcoor:".Length).Trim();
            ParseCoordinates(coordinates, state);
        }
        else if (tag.StartsWith("arrowrotate:"))
        {
            string rotation = tag.Substring("arrowrotate:".Length).Trim();
            ParseRotation(rotation, state);
        }
    }

    private void ParseCoordinates(string coordinateString, TutorialState state)
    {
        coordinateString = coordinateString.Replace("(", "").Replace(")", "").Trim();

        string[] parts = coordinateString.Split(',');

        if (parts.Length == 2)
        {
            if (float.TryParse(parts[0].Trim(), out float x) &&
                float.TryParse(parts[1].Trim(), out float y))
            {
                state.SetArrowPosition(new Vector2(x, y));
            }
            else
            {
                Debug.LogWarning($"Failed to parse arrow coordinates: {coordinateString}");
            }
        }
        else
        {
            Debug.LogWarning($"Invalid coordinate format: {coordinateString}. Expected 'x,y'");
        }
    }

    private void ParseRotation(string rotationString, TutorialState state)
    {
        if (float.TryParse(rotationString.Trim(), out float rotation))
        {
            state.SetArrowRotation(rotation);
        }
        else
        {
            Debug.LogWarning($"Failed to parse arrow rotation: {rotationString}");
        }
    }
}