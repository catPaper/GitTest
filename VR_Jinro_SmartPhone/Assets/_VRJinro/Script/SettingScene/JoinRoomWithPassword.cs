using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class JoinRoomWithPassword : MonoBehaviour {
    [SerializeField]
    private Text infoText;
    [SerializeField]
    private Text userName;
    [SerializeField]
    private RoomCanvasManager roomCanvasManager;

    public byte Version = 1;

    private bool IsConectingRoom = false;

    /// <summary>
    /// 接続するルーム名
    /// </summary>
    private string connectRoomName;

    private SettingManager setManager;

    void Awake()
    {
        setManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<SettingManager>();
        infoText.text = "";
    }

    /// <summary>
    /// ルームにランダムに入る　TODO ルームナンバーに基づきルームに入る
    /// </summary>
	public void JoinRoom(string _roomNo)
    {
        setManager.isHost = false;
        if (!IsConectingRoom && !PhotonNetwork.connected)
        {
            connectRoomName = _roomNo;
            PhotonNetwork.playerName = userName.text;
            roomCanvasManager.SetRoomCanvas();
            infoText.text = "接続中...";
            Debug.Log(" Let's connect to the Photon Master Server. Calling: PhotonNetwork.ConnectUsingSettings();");
            IsConectingRoom = true;
            PhotonNetwork.ConnectUsingSettings(Version + "." + SceneManagerHelper.ActiveSceneBuildIndex);
        }
    }

    /// <summary>
    /// ホストの部屋が見つからない場合に呼ばれる
    /// </summary>
    private void OnPhotonJoinRoomFailed()
    {
        if (!setManager.isHost)
        {
            IsConectingRoom = false;
            PhotonNetwork.Disconnect();
            infoText.text = "ルームがありません。";
        }
    }

    public virtual void Start()
    {
        PhotonNetwork.autoJoinLobby = false;    // we join randomly. always. no need to join a lobby to get the list of rooms.
    }


    // below, we implement some callbacks of PUN
    // you can find PUN's callbacks in the class PunBehaviour or in enum PhotonNetworkingMessage

    /// <summary>
    /// マスターサーバーに接続する
    /// </summary>
    public virtual void OnConnectedToMaster()
    {
        if (!setManager.isHost)
        {
            PhotonNetwork.JoinRoom(connectRoomName.ToString());
            Debug.Log("lobby=" + PhotonNetwork.lobby + "::");
        }
    }


    // the following methods are implemented to give you some context. re-implement them as needed.

    /// <summary>
    /// マスターサーバーに接続できなかった時に呼ばれる
    /// </summary>
    /// <param name="cause"></param>
    public virtual void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        if (!setManager.isHost)
        {
            IsConectingRoom = false;
            Debug.LogError("Cause: " + cause);
        }
    }


    /// <summary>
    /// ルームに入れたときに呼ばれる
    /// </summary>
    public void OnJoinedRoom()
    {
        if (!setManager.isHost)
        {
            IsConectingRoom = false;
            infoText.text = "";
            Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room. From here on, your game would be running. For reference, all callbacks are listed in enum: PhotonNetworkingMessage");
            GameObject.FindGameObjectWithTag("Manager").GetComponent<CanvasManager>().OpenRoomCanvas();

            setManager.CreatePlayerPrefab();
            PhotonVoiceNetwork.Connect();
        }
    }
}
