using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogBoxScript : MonoBehaviour
{
    [SerializeField] private Image portraitImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text dialogText;
    public void SetText(string name, string dialog)
    {
        nameText.text = name;
        portraitImage.sprite = DataManager.Instance.GetProfilePicture(name);
        dialogText.text = dialog;
        RebuildLayout();
    }
    public void SetPortrait(string portraitName)
    {
        portraitImage.sprite = DataManager.Instance.GetProfilePicture(portraitName);
    }

    public void AppendText(string dialog)
    {
        dialogText.text += dialog;
        RebuildLayout();
    }

    void RebuildLayout()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent as RectTransform);
    }
}
