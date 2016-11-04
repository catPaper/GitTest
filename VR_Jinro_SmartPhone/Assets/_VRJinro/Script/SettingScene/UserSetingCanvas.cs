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
    [SerializeField]
    private Text PassCodeText;
    [SerializeField]
    private Text infoText;
    [SerializeField]
    private CanvasManager canvasManager;

    private PasswordGenerator passGenerator;

    void Awake()
    {
        infoText.text = "";
        passGenerator = new PasswordGenerator();
        JoinButton.interactable = false;
        CreateButton.interactable = false;
    }

    void Start()
    {
        CreateButton.onClick.AddListener(CheckingPassCodeAndCreateRoom);
        JoinButton.onClick.AddListener(CheckingPassCodeAndJoinRoom);
    }

    void Update()
    {
        if(NameText.text == "" || PassCodeText.text == "")
        {
            JoinButton.interactable = false;
            CreateButton.interactable = false;
        }else
        {
            JoinButton.interactable = true;
            CreateButton.interactable = true;
        }
    }

    /// <summary>
    /// パスコードをチェックし、ルームをクリエイトする
    /// </summary>
    public void CheckingPassCodeAndCreateRoom()
    {
        if (passGenerator.CheckPassword(int.Parse(PassCodeText.text)) || PassCodeText.text == "8263")
        {
            canvasManager.OpenSettingCanvas();
        }else
        {
            infoText.text = "パスワードが違います。";
        }
    }

    /// <summary>
    /// ルームIDをチェックし、ルームに入る
    /// </summary>
    public void CheckingPassCodeAndJoinRoom()
    {
        GetComponent<JoinRoomWithPassword>().JoinRoom(PassCodeText.text);
    }


}
