using System.Collections.Generic;
using UnityEngine;

public class DialogueParser
{
    private const string SPEAKER_TAG = "speaker";
    private const string PORTRAIT_TAG = "portrait";

    public void HandleTags(List<string> currentTags, DialogueState state)
    {
        if (currentTags == null) return;

        foreach (string tag in currentTags)
        {
            ParseAndApplyTag(tag, state);
        }
    }

    private void ParseAndApplyTag(string tag, DialogueState state)
    {
        string[] splitTag = tag.Split(':');
        if (splitTag.Length != 2)
        {
            Debug.LogError("Invalid tag format: " + tag);
            return;
        }

        string tagKey = splitTag[0].Trim();
        string tagValue = splitTag[1].Trim();

        switch (tagKey)
        {
            case SPEAKER_TAG:
                state.SetSpeaker(tagValue);
                break;
            case PORTRAIT_TAG:
                state.SetPortraitTag(tagValue);
                break;
            default:
                Debug.LogWarning($"Unknown tag: {tagKey}");
                break;
        }
    }
}