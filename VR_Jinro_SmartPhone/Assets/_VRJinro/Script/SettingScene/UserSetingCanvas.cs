using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class UserSetingCanvas : MonoBehaviour {

    [SerializeField]
    private Text NameText;
    [SerializeField]
    private Button JoinButton;
    [SerializeField]
    private Button CreateButton;

    void Awake()
    {
        JoinButton.interactable = false;
        CreateButton.interactable = false;
    }

    void Update()
    {
        if(NameText.text == "")
        {
            JoinButton.interactable = false;
            CreateButton.interactable = false;
        }else
        {
            JoinButton.interactable = true;
            CreateButton.interactable = true;
        }
    }
}
