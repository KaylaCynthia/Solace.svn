using UnityEngine;
using UnityEngine.UI;

public class ChatAreaScript : MonoBehaviour
{
    private Image chatAreaBG;

    void OnEnable()
    {
        chatAreaBG = GetComponent<Image>();
    }
    public void SetChatAreaSelected(bool isSelected)
    {
        if (chatAreaBG == null)
        {
            chatAreaBG = GetComponent<Image>();
        }

        if (chatAreaBG != null)
        {
            chatAreaBG.enabled = isSelected;
        }
    }
}
