using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ChannelDialogPlayer : MonoBehaviour
{
    [SerializeField] private Transform dialogParent;
    [SerializeField] private GameObject dialogPrefab;
    GameObject currentDialog;

    public void CreateNewMessage(string text, List<string> tags)
    {
        // Debug.Log("Creating new message, tags: " + string.Join(",", tags?.ToArray() ?? new string[0]));
        bool hasPortraitTag = tags?.Any(tag => tag.StartsWith("portrait")) ?? false;
        if(currentDialog == null && !hasPortraitTag)
        {
            Debug.LogError("No previous textbox and no portrait");
            return;
        }
        // Debug.Log("Has portrait tag: " + hasPortraitTag);

        if(hasPortraitTag)
        {
            // Create a new dialog
            currentDialog = Instantiate(dialogPrefab, dialogParent);
            currentDialog.GetComponent<DialogBoxScript>().SetText("aa", text);
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
    }
}
