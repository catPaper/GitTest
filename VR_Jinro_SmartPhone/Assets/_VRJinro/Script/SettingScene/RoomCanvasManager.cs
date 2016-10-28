using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView))]
public class RoomCanvasManager : Photon.MonoBehaviour {
    [SerializeField]
    private GameObject memberBoxPrefab;
    [SerializeField]
    private Text disconnectText;
    [SerializeField]
    private Button hostStartButton;
    [SerializeField]
    private Button hostDisconnectButton;
    [SerializeField]
    private Text memberCountText;
    [SerializeField]
    private GameObject memberList;

    private PhotonView myPV;
    //ホストが定めるゲームプレイヤー数
    private int gamePlayMemberNumber;

    private SettingManager setManager;

    void Awake()
    {
        myPV = GetComponent<PhotonView>();
        setManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<SettingManager>();

    }

    void Update()
    {
        if(setManager.isHost)
        {
            if(gamePlayMemberNumber == PhotonNetwork.playerList.Length)
            {
                hostStartButton.interactable = true;
            }else
            {
                hostStartButton.interactable = false;
            }
        }
    }

    public void HOSTONLY_SetGameMemberNumber(int _gameMemberCount)
    {
        gamePlayMemberNumber = _gameMemberCount;
    }

    public void SetRoomCanvas()
    {
        
        if (setManager.isHost)
        {
            //一人のためRPC無し
            SyncPlayerCount(gamePlayMemberNumber);
            AddNewPlayer(PhotonNetwork.playerName);
            hostStartButton.gameObject.SetActive(true);
            hostDisconnectButton.gameObject.SetActive(true);
            disconnectText.text = "解散する";
        }else
        {
            hostStartButton.gameObject.SetActive(false);
            hostDisconnectButton.gameObject.SetActive(false);
        }
    }


    public virtual void OnPhotonPlayerConnected(PhotonPlayer localPlayer)
    {
        AddNewPlayer(localPlayer.name);
       
        if (setManager.isHost)
        {
            myPV.RPC("SyncPlayerList", localPlayer);
            myPV.RPC("SyncPlayerCount", PhotonTargets.All, gamePlayMemberNumber);
        }

        setManager.SendPlayerInfo(localPlayer);

    }


    public virtual void OnMasterClientSwitched()
    {
        Debug.Log("Host is left room.");
        PhotonNetwork.Disconnect();
        deleteAllUserBoxes();
        GameObject.FindGameObjectWithTag("Manager").GetComponent<CanvasManager>().OpenUserNameCanvas();
    }

    public void DisconnectRoom()
    {
        PhotonNetwork.Disconnect();
        deleteAllUserBoxes();
        GameObject.FindGameObjectWithTag("Manager").GetComponent<CanvasManager>().OpenUserNameCanvas();
        if (setManager.isHost)
            setManager.DestroyHostOnly_GameInfo();
            
    }

    private void deleteAllUserBoxes()
    {
        foreach(Transform userbox in memberList.transform)
        {
            Destroy(userbox.gameObject);
        }
    }

    public virtual void OnLeftRoom()
    {
       //TODO 退出者の削除
        Debug.Log("LeftRoom");
    }

    private void AddNewPlayer(string userName)
    {
        GameObject newMeber = Instantiate(memberBoxPrefab,memberList.transform) as GameObject;
        newMeber.GetComponentInChildren<Text>().text = userName;
        newMeber.transform.localScale = new Vector3(1, 1, 1);
    }

    /// <summary>
    /// すべてのプレイヤーに一度ソートを駆けるように促す
    /// </summary>
    public void AllPlayerSortList()
    {
        myPV.RPC("SortPlayerList", PhotonTargets.All);
    }

    [PunRPC]
    private void SyncPlayerList()
    {
        deleteAllUserBoxes();
        foreach (PhotonPlayer player in PhotonNetwork.playerList)
        {
            AddNewPlayer(player.name);
        }
    }
    
    public void TestUpdateList()
    {
        for(int i= 0;i < PhotonNetwork.playerList.Length;i++)
        {
            Debug.Log("photonPlayer" + PhotonNetwork.playerList[i].name + ":index=" + i);
        }
    }

    [PunRPC]
    private void SyncPlayerCount(int _gamePlayerNumber)
    {
        memberCountText.text = PhotonNetwork.playerList.Length.ToString() + '/' + _gamePlayerNumber;
    }

    /// <summary>
    /// プレイヤーリストがずれるため、ここでIDをもとに直す（※プレイヤーが出入りしていた場合、このソートがうまくいかない）
    /// </summary>
    [PunRPC]
    private void SortPlayerList()
    {
        if (PhotonNetwork.playerList.Length != PhotonNetwork.player.ID)
        {
            int endIndex = PhotonNetwork.player.ID;
            PhotonPlayer tmpPlayer;
            for (int i = 0; i < endIndex; i++)
            {
                tmpPlayer = PhotonNetwork.playerList[i];
                PhotonNetwork.playerList[i] = PhotonNetwork.playerList[i + 1];
                PhotonNetwork.playerList[i + 1] = tmpPlayer;
            }
        }
    }

}
