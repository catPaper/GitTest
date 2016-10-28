using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PhotonView))]
public class NoonPhase : Photon.MonoBehaviour {
    [SerializeField]
    private MainGameManager mgManager;
    [SerializeField]
    private List<Transform> spownList;
    [SerializeField]
    private List<Transform> voteSpownPoint;
    [SerializeField]
    private GameTimer gameTimer;
    [SerializeField]
    private SelectTarget selectTargetScript;
    [SerializeField]
    private Transform liveCameraPos;

    private PhotonView myPV;

    //選択先のプレイヤー情報
    private PLAYER_Info targetInfo;

    void Awake()
    {
        myPV = GetComponent<PhotonView>();
    }

    void Update()
    {
       
        if (mgManager.AllMembersPhase() == DataBase.Phase.VOTETIME)
        {
            int searchID = selectTargetScript.SearchSelectTargetID(mgManager.MySelectObject());
            if (searchID != -1)
            {
                targetInfo =  mgManager.SearchByID(searchID);
                selectTargetScript.SetResultText(DataBase.Phase.VOTETIME, targetInfo,mgManager.MyRoll());
            }
            //投票確認ボタン
            if(mgManager.MySelectObject() ==  selectTargetScript.DicisionButtonColider())
            {
                if (targetInfo != null)
                {
                    mgManager.SetVotingDestination(targetInfo.PlayerID());
                    selectTargetScript.ShowDicisonButton(false);
                    selectTargetScript.ShowWaitText(true);
                    selectTargetScript.HideTargets();
                }
                mgManager.SetPlayerPhase(DataBase.Phase.EVENING);
            }
            //Debug用
            if (Input.GetKeyDown(KeyCode.A))
            {
                selectTargetScript.ShowDicisonButton(true);
            }
        }
    }

    public Transform LiveCameraPos()
    {
        return liveCameraPos;
    }

    public List<Transform> SpownList()
    {
        return spownList;
    }

    public List<Transform> VoteSpownPoint()
    {
        return voteSpownPoint;
    }

    /// <summary>
    /// 相談時間開始時のセットアップ
    /// </summary>
    public void HOSTONLY_FirstSetUp()
    {
        myPV.RPC("SyncFirstSetUp", PhotonTargets.All);    
    }

    /// <summary>
    /// フェード時間中にセットアップを行う
    /// </summary>
    public void HOSTONLY_DelaySecondSetUp()
    {
        DataBase tmpDatabase = new DataBase();
        StartCoroutine(HOSTONLY_SecondSetUp(tmpDatabase.FadeTime));
    }

    /// <summary>
    /// 投票時間開始時のセットアップ
    /// </summary>
    private IEnumerator  HOSTONLY_SecondSetUp(float delayCount)
    {
        yield return new WaitForSeconds(delayCount);
        myPV.RPC("SyncSecondSetUp", PhotonTargets.All);
    }

    /// <summary>
    /// ホストからすべてのプレイヤーへタイマー更新の指示を行う
    /// </summary>
    /// <param name="restCount"></param>
    public void HOSTONLY_UpdateTimerText(int restCount)
    {
        if(!PhotonNetwork.isMasterClient)
        {
            Debug.Log("ホストでないプレイヤーが時間を操作しています。");
            return;
        }
        myPV.RPC("SyncTimerText", PhotonTargets.All, restCount);
    }

    [PunRPC]
    private void SyncTimerText(int restCount)
    {
        gameTimer.UpdateTimerText(restCount);
    }

    [PunRPC]
    private void SyncFirstSetUp()
    {
        selectTargetScript.AllGroupHide();
    }

    [PunRPC]
    private void SyncSecondSetUp()
    {
        List<PLAYER_Info> _infoList = mgManager.AlivePlayerInfoWithoutMe();

        _infoList =  mgManager.ShufflePlayerInfo(_infoList);
        selectTargetScript.SetTargetsSetting(_infoList,mgManager.GameCharacterList());
        selectTargetScript.ShowSetUp();
    }
}
