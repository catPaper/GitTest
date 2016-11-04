using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MemberBox : MonoBehaviour {

    [SerializeField]
    private Text playerNameText;
    [SerializeField]
    private Image voiceImage;

    public string PlayerNameText()
    {
        return playerNameText.text;
    }

    public void SetPlayerName(string name)
    {
        playerNameText.text = name;
    }

    public void ShowVoiceImage(bool show)
    {
        voiceImage.enabled = show;
    }

}
