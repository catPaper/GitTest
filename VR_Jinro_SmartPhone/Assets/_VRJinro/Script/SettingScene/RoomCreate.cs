using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RoomCreate : Photon.MonoBehaviour {

    [SerializeField]
    private Text userName;
    [SerializeField]
    private RoomCanvasManager roomCanvasManager;

    public byte Version = 1;

    private SettingManager setManager;

    private RoomOptions roomOptions;

    private GameObject myPlayerPrefab;

    public virtual void Start()
    {
        setManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<SettingManager>();
        PhotonNetwork.autoJoinLobby = false;    // we join randomly. always. no need to join a lobby to get the list of rooms.
    }

   public void CreateRoom(RoomOptions _roomOptions,int _gamePlayerNumber)
    {
        roomOptions = _roomOptions;
        setManager.isHost = true;
        if (!PhotonNetwork.connected)
        {
            PhotonNetwork.playerName = userName.text;
            roomCanvasManager.HOSTONLY_SetGameMemberNumber(_gamePlayerNumber);
            roomCanvasManager.SetRoomCanvas();
            Debug.Log("Let's connect to the Photon Master Server. Calling: PhotonNetwork.ConnectUsingSettings();");

            PhotonNetwork.ConnectUsingSettings(Version + "." + SceneManagerHelper.ActiveSceneBuildIndex);
        }
    }


    // below, we implement some callbacks of PUN
    // you can find PUN's callbacks in the class PunBehaviour or in enum PhotonNetworkingMessage


    public virtual void OnConnectedToMaster()
    {
        if (setManager.isHost)
        {
            Debug.Log("OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room. Calling: PhotonNetwork.CreateRoom();");
          
            PhotonNetwork.CreateRoom(null,roomOptions , null);
        }
    }

    /*
    public virtual void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby(). This client is connected and does get a room-list, which gets stored as PhotonNetwork.GetRoomList(). This script now calls: PhotonNetwork.JoinRandomRoom();");
        PhotonNetwork.JoinRandomRoom();
    }
    */

    public virtual void OnPhotonRandomJoinFailed()
    {
        if (setManager.isHost)
        {
            Debug.Log("OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one. ");
        }
        
    }

    // the following methods are implemented to give you some context. re-implement them as needed.

    public virtual void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        if (setManager.isHost)
        {
            Debug.LogError("Cause: " + cause);
        }
    }

    public void OnJoinedRoom()
    {
        if (setManager.isHost)
        {
            Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room. From here on, your game would be running. For reference, all callbacks are listed in enum: PhotonNetworkingMessage");
            GameObject.FindGameObjectWithTag("Manager").GetComponent<CanvasManager>().OpenRoomCanvas();
            setManager.CreatePlayerPrefab();
        }
    }
}
