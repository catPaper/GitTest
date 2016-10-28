using UnityEngine;
using System.Collections;

public class SettingManager : MonoBehaviour {

    
    [Header("Read Only")]
    public bool isHost = false;

    private HOSTONLY_GameInfo hostonly_GameInfo;
    private GameObject myPlayerPrefab;

    public void CreatePlayerPrefab()
    {
        myPlayerPrefab = PhotonNetwork.Instantiate("PlayerPrefab", Vector3.zero, Quaternion.identity, 0);
        myPlayerPrefab.GetComponent<PLAYER_Info>().AssignPlayerInfo();
    }

    public void SendPlayerInfo(PhotonPlayer _player)
    {
        myPlayerPrefab.GetComponent<PLAYER_Info>().SendPlayerInfo(_player);
    }

    public void SetVRModeToggle(bool _isVRMode)
    {
        myPlayerPrefab.GetComponent<PLAYER_Info>().SetVRMode(false);
#if (UNITY_ANDROID || UNITY_IOS)
        myPlayerPrefab.GetComponent<PLAYER_Info>().SetVRMode(_isVRMode);
#endif
    }

    public void DestroyHostOnly_GameInfo()
    {
        Destroy(hostonly_GameInfo.gameObject);
    }


    /// <summary>
    /// ホスト専用ゲームインフォプレファブをセットする
    /// </summary>
    /// <param name="_hostonly_gameinfo"></param>
    public void SetHostOnly_GameInfo(HOSTONLY_GameInfo _hostonly_gameinfo)
    {
        hostonly_GameInfo = _hostonly_gameinfo;
    }

    /// <summary>
    /// 部屋を非公開設定にする
    /// </summary>
    public void RoomCloseMode()
    {
        hostonly_GameInfo.IsOpenRoom(false);
    }

    
	
}
