using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogBoxScript : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text dialogText;
    public void SetText(string name, string dialog)
    {
        nameText.text = name;
        dialogText.text = dialog;
    }

    public void AppendText(string dialog)
    {
        dialogText.text += dialog;
    }
}
