using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ChannelDialogPlayer : MonoBehaviour
{
    [SerializeField] private Transform dialogParent;
    [SerializeField] private GameObject dialogPrefab;
    GameObject currentDialog;
    string currentSpeaker;
    public void CreateNewMessage(string text, List<string> tags)
    {
        // Debug.Log("Creating new message, tags: " + string.Join(",", tags?.ToArray() ?? new string[0]));
        bool hasSpeakerTag = tags.Find(tag => tag.StartsWith("speaker")) != null;
        if(currentDialog == null && !hasSpeakerTag)
        {
            Debug.LogError("No previous textbox and no portrait");
            return;
        }
        // Debug.Log("Has portrait tag: " + hasPortraitTag);

        if(hasSpeakerTag)
        {
            // Create a new dialog
            currentDialog = Instantiate(dialogPrefab, dialogParent);
            currentSpeaker = GetMessageName(tags);
            currentDialog.GetComponent<DialogBoxScript>().SetText(currentSpeaker, text);

            bool hasPortraitTag = tags.Find(tag => tag.StartsWith("portrait")) != null;
            if(hasPortraitTag)
            {
                string portraitName = tags.Find(tag => tag.StartsWith("portrait")).Split(':')[1].Trim();
                currentDialog.GetComponent<DialogBoxScript>().SetPortrait(portraitName);
            }
            Debug.Log("New dialog created: " + text);
        }
        else
        {
            if(currentDialog != null)
            {
                // Append to the existing dialog
                Debug.Log("Appending " + text +  " existing dialog");
                currentDialog.GetComponent<DialogBoxScript>().AppendText(text);
            }
            else
            {
                Debug.LogError("This should be impossible because of the check above");
            }
        }

        string GetMessageName(List<string> tags)
        {
            return tags.FirstOrDefault(tag => tag.StartsWith("speaker:"))?.Split(':')[1].Trim() ?? "Unknown";
        }
    }
}
