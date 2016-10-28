using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView))]
public class NightPhase : Photon.MonoBehaviour {
    [SerializeField]
    private MainGameManager mgManager;
    [SerializeField]
    private GameTimer gameTimer;
    [SerializeField]
    private List<Transform> spownList;
    [SerializeField]
    private SelectTarget selectTargetScript;
    [SerializeField]
    private Text rollNameText;
    [SerializeField]
    private Text explainText;

    //最後に選択したターゲットID
    private int lateSelectID = -1;


    //選択先のプレイヤー情報
    private PLAYER_Info targetInfo;

    private PhotonView myPV;

    void Awake()
    {
        myPV = GetComponent<PhotonView>();
    }

    void Update()
    {
        if(mgManager.AllMembersPhase() == DataBase.Phase.NIGHT)
        {
            int searchID = selectTargetScript.SearchSelectTargetID(mgManager.MySelectObject());
            
            if (searchID != -1)
            {
                lateSelectID = searchID;
                targetInfo = mgManager.SearchByID(searchID);
                selectTargetScript.SetResultText(DataBase.Phase.NIGHT, targetInfo, mgManager.MyRoll());
                if(mgManager.MyRoll() == DataBase.Roll.DIVINER)
                {
                    selectTargetScript.HideTargets();
                }
            }
            if (mgManager.MySelectObject() == selectTargetScript.DicisionButtonColider())
            {
                selectTargetScript.ShowDicisonButton(false);
                selectTargetScript.ShowWaitText(true);
                selectTargetScript.HideTargets();
                mgManager.SetNightActDestination(lateSelectID);
                mgManager.SetPlayerPhase(DataBase.Phase.MORNING);
            }
            if(Input.GetKeyDown(KeyCode.A))
            {
                selectTargetScript.ShowDicisonButton(true);
            }
        }
    }

    /// <summary>
    /// 夜行動のセットアップ
    /// </summary>
    public void HOSTONLY_SetUp()
    {
        myPV.RPC("SyncSetUp", PhotonTargets.All);
    }

    public List<Transform> SpownList()
    {
        return spownList;
    }

    /// <summary>
    /// ホストからすべてのプレイヤーへタイマー更新の指示を行う
    /// </summary>
    /// <param name="restCount"></param>
    public void HOSTONLY_UpdateTimerText(int restCount)
    {
        if (!PhotonNetwork.isMasterClient)
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
    private void SyncSetUp()
    {
        DataBase tmpDatabase = new DataBase();
        lateSelectID = -1;
        //コンストラクタで管理
        List<PLAYER_Info> _infoList = new List<PLAYER_Info>(mgManager.AlivePlayerInfoWithoutMe());
        //人狼の除外設定
        if (mgManager.MyRoll() == DataBase.Roll.WEREWOLF)
        {
            //仲間の人狼も除外
            for (int i = 0; i < _infoList.Count; i++)
            {
                if (_infoList[i].IsWereWolf())
                {
                    _infoList.RemoveAt(i);
                    continue;
                }
            }
        }
        //カルトの除外設定
        if(mgManager.MyRoll() == DataBase.Roll.CULTLEADER)
        {
            //カルト信者は除外
            for(int i= 0;i< _infoList.Count;i++)
            {
                if(_infoList[i].IsCult())
                {
                    _infoList.RemoveAt(i);
                    continue;
                }
            }
        }
        _infoList = mgManager.ShufflePlayerInfo(_infoList);
        selectTargetScript.SetTargetsSetting(_infoList,mgManager.GameCharacterList());
        selectTargetScript.ShowSetUp();
        if (mgManager.MyRoll() == DataBase.Roll.NURSES)
        {
            selectTargetScript.SetNursesResult(mgManager.SearchByID(mgManager.ExcutionPlayerID()), (mgManager.ExcutionPlayerID() != -1));
        }
        rollNameText.text = tmpDatabase.RollName(mgManager.MyRoll());
        explainText.text = tmpDatabase.MyNightActExplain(mgManager.MyRoll());

        //TODO 生存しているカルト信者のリスト表示
    }
}
