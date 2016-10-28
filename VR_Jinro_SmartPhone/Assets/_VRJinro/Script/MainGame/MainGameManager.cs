using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PhotonView))]
public class MainGameManager : Photon.MonoBehaviour {

    [SerializeField]
    private CharacterList charaListScript;
    [SerializeField]
    private SetUpPhase setUpPhase;
    [SerializeField]
    private MorningPhase morningPhase;
    [SerializeField]
    private NoonPhase noonPhase;
    [SerializeField]
    private EveningPhase eveningPhase;
    [SerializeField]
    private NightPhase nightPhase;
    [SerializeField]
    private ResultPhase resultPhase;
    [SerializeField]
    private LookingRoom lookingRoom;
    [SerializeField]
    private Light Sun;
    [SerializeField]
    private LIST_boardPlayerInfo boardPlayerInfoList;
    [SerializeField]
    private LIST_boardPlayerInfo boardPlayerInfoListForResult;

    private bool isHost;

    private DataBase.Camp winCamp;

    private GameObject myPlayerPrefab;
    private PLAYER_Info myPlayerInfo;

    /// <summary>
    /// 自分を除いた生存プレイヤーリスト
    /// </summary>
    private List<PLAYER_Info> alivePlayerListWithOutMe;

    private PhotonView myPV;

    //ホストオンリーユーズ
    private HOSTONLY_GameInfo hostOnly_GameInfo;

    private DataBase.Phase allMembersPhase;

    //NoonTimeのスキップ処理の負荷を抑えるのに使用
    private float OldTime;

    //直近の処刑者のID
    [Header("Debug")]
    [SerializeField]
    private int excutionPlayerID;


    private void Awake()
    {
        allMembersPhase = DataBase.Phase.BRIEFINGROOM;
        myPV = GetComponent<PhotonView>();

        isHost = PhotonNetwork.isMasterClient;
        if (isHost)
            hostOnly_GameInfo = GameObject.FindGameObjectWithTag("GameInfo").GetComponent<HOSTONLY_GameInfo>();

        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (go.GetComponent<PhotonView>().isMine)
            {
                myPlayerPrefab = go;
                myPlayerInfo = go.GetComponent<PLAYER_Info>();
                break;
            }
        }
        myPlayerPrefab.transform.position = setUpPhase.PlayerSpownPoint().position;
        myPlayerPrefab.transform.rotation = setUpPhase.PlayerSpownPoint().rotation;

        
        AlivePlayerInfoFirstSetUp();
        //初回起動時にはVRセッティングフェーズに移行
        myPlayerInfo.SetMyPhase(DataBase.Phase.VRSETTING);

        if(isHost)
        {
            hostOnly_GameInfo.SetPlayerPrefabs();
        }
        
        myPlayerInfo.FirstInfoBoxSetting();
        myPlayerInfo.ShowInfoBox(true);
    }

   

    private void Update()
    {
        if (isHost)
        {
            //Phaseは全体のPhaseを中心に移行する
            switch(allMembersPhase)
            {
                case DataBase.Phase.BRIEFINGROOM:
                    //全員が次のシーンに行った場合、シーン全体のフェーズをVRSETTINGへ移行
                    if (hostOnly_GameInfo.IsAllMemberSamePhase(DataBase.Phase.VRSETTING))
                    {
                        myPV.RPC("SyncExcutionPlayerID", PhotonTargets.All, -1);
                        myPV.RPC("SyncAliveCount", PhotonTargets.All, hostOnly_GameInfo.AliveMemberNumber());
  
                                                                      
                        myPV.RPC("SyncAllMembersPhase", PhotonTargets.All, DataBase.Phase.VRSETTING);
                        SetLightDirection(DataBase.Phase.BRIEFINGROOM);
                        hostOnly_GameInfo.StartSetting();
                        HOSTONLY_SetRollList();
                        boardPlayerInfoList.InitPlayerList();
                        boardPlayerInfoListForResult.InitPlayerList();
                        setUpPhase.SetUpPhase_Start();
                        HOSTONLY_SetHeadMesh();
                    }
                    break;
                case DataBase.Phase.VRSETTING:
                case DataBase.Phase.NIGHT:
                    //全員が次のシーンに行った場合、シーン全体をMORNINGへ移行、終了条件を満たしていた場合RESULTへ移行
                    if (hostOnly_GameInfo.IsAllMemberSamePhase(DataBase.Phase.MORNING))
                    {
                        //夜行動反映処理
                        if (allMembersPhase == DataBase.Phase.NIGHT)
                        {
                            HOSTONLY_NightActProcess();
                        }
                        else
                        {
                            morningPhase.FirstBoardSetUp();
                        }

                        HOSTONLY_SceneMoveSetting(DataBase.Phase.MORNING, true);
                    }
                    break;
                case DataBase.Phase.MORNING:
                    //全員が次のシーンに行った場合、シーン全体をNOONへ移行
                    if (hostOnly_GameInfo.IsAllMemberSamePhase(DataBase.Phase.NOON))
                    {
                        //ゲーム終了チェック
                        if (HOSTONLY_CheckGameEnd())
                        {
                            HOSTONLY_ResultSetting(winCamp);
                            break;
                        }
                        noonPhase.HOSTONLY_FirstSetUp();
                        //スキップまで５秒のマージンをとる
                        OldTime = Time.time + 5;
                        myPV.RPC("SyncShowSkipImage", PhotonTargets.All, true);
                        hostOnly_GameInfo.SetNoonEndTime();
                        //0.2秒おきに更新（処理負荷回避）
                        InvokeRepeating("HOSTONLY_InvokeRepeatTimerCount", 0, 0.2f);
                        HOSTONLY_SceneMoveSetting(DataBase.Phase.NOON, true);
                    }
                    break;
                case DataBase.Phase.NOON:
                    //全員が次のシーンに行った場合、シーン全体をVOTETIMEへ移行
                    if (hostOnly_GameInfo.IsAllMemberSamePhase(DataBase.Phase.VOTETIME))
                    {
                        noonPhase.HOSTONLY_DelaySecondSetUp();
                        myPV.RPC("SyncShowSkipImage", PhotonTargets.All, false);
                        myPV.RPC("SyncAllPlayerRecieveVoteReset", PhotonTargets.All);
                        HOSTONLY_SceneMoveSetting(DataBase.Phase.VOTETIME, false);
                    }
                    break;
                case DataBase.Phase.VOTETIME:
                    //全員が次のシーンに行った場合、シーン全体をEVENINGへ移行
                    if (hostOnly_GameInfo.IsAllMemberSamePhase(DataBase.Phase.EVENING))
                    {
                        HOSTONLY_AggregateVote();
                        myPV.RPC("SyncIfExcusion", PhotonTargets.All);
                        eveningPhase.ExcutionInfoSetUp();
                        myPV.RPC("SyncAliveCount", PhotonTargets.All, hostOnly_GameInfo.AliveMemberNumber());
                        HOSTONLY_SceneMoveSetting(DataBase.Phase.EVENING, true);
                    }
                    break;
                case DataBase.Phase.EVENING:
                    //全員が次のシーンに行った場合、シーン全体をNIGHTへ移行
                    if (hostOnly_GameInfo.IsAllMemberSamePhase(DataBase.Phase.NIGHT))
                    {
                        //ゲーム終了チェック
                        if (HOSTONLY_CheckGameEnd())
                        {
                            HOSTONLY_ResultSetting(winCamp);
                            break;
                        }
                        myPV.RPC("SyncAllPlayerNightActReset", PhotonTargets.All);
                        nightPhase.HOSTONLY_SetUp();
                        hostOnly_GameInfo.SetNightEndTime();
                        InvokeRepeating("HOSTONLY_InvokeRepeatTimerCount", 0, 0.2f);
                        HOSTONLY_SceneMoveSetting(DataBase.Phase.NIGHT, false);
                    }
                    break;
            }
        }

        switch (allMembersPhase)
        {
            case DataBase.Phase.VRSETTING:
                if (!myPlayerInfo.IsActiveMyCamera())
                {
#if (UNITY_ANDROID || UNITY_IOS || UNITY_EDITOR)
                    myPlayerInfo.ReCenter();
#endif
                    myPlayerInfo.ActiveMyCamera(true);
                    
                }
                break;
            case DataBase.Phase.NOON:
                if(isHost)
                {
                    //Debug用
                    if(Input.GetKeyDown(KeyCode.S))
                    {
                        hostOnly_GameInfo.SkipNoonTime();
                    }
                    if(Time.time > OldTime)
                    {
                        int skipCount = 0;
                        OldTime = Time.time + 1;    //１秒後にまた呼び出し
                        foreach(PLAYER_Info _info in hostOnly_GameInfo.AliveAllMemberInfo())
                        {
                            if (_info.Skip())
                                skipCount++;
                        }
                        //過半数を超えたらスキップ
                        if (skipCount * 2 >= hostOnly_GameInfo.AliveAllMemberInfo().Count)
                            hostOnly_GameInfo.SkipNoonTime();
                    }
                }
                break;
            case DataBase.Phase.NIGHT:
                if(isHost)
                {
                    //Debug用
                    if(Input.GetKeyDown(KeyCode.S))
                    {
                        hostOnly_GameInfo.SkipNightTime();
                    }
                }
                break;

        }

       
    }

    /// <summary>
    /// ヘッドメッシュのセット処理
    /// </summary>
    private void HOSTONLY_SetHeadMesh()
    {
        charaListScript.HOSTONLY_ShuffleList();
        charaListScript.HOSTONLY_LinkMeshListAllPlayer();
        myPV.RPC("SyncSetHeadMesh", PhotonTargets.All);
    }

    /// <summary>
    /// 全生存（夜時点）プレイヤーの行動を反映させる
    /// </summary>
    private void HOSTONLY_NightActProcess()
    {
       
        List<int> jinroSelectTargetIDList = new List<int>();
        List<int> hunterProtectedTargetIDList = new List<int>();
        foreach (PLAYER_Info _info in hostOnly_GameInfo.AliveAllMemberInfo())
        {
            if (_info.NightActDestination() != -1)
            {
                if (_info.IsWereWolf())
                    jinroSelectTargetIDList.Add(_info.NightActDestination()); Debug.Log("JinroExist");
                if (_info.MyRoll() == DataBase.Roll.HUNTER)
                    hunterProtectedTargetIDList.Add(_info.NightActDestination());
                if (_info.MyRoll() == DataBase.Roll.CULTLEADER)
                {
                    myPV.RPC("SyncToCultIfSelect", PhotonTargets.All, _info.NightActDestination());
                    boardPlayerInfoList.UpdateIsCult(_info.NightActDestination(), true);
                    boardPlayerInfoListForResult.UpdateIsCult(_info.NightActDestination(), true);
                }
            }
        }
        //襲撃対象リスト
        jinroSelectTargetIDList = SelectMostCommonID(jinroSelectTargetIDList);
        //対象をランダムに選択
        int JinroSelectIndex = Random.Range(0, jinroSelectTargetIDList.Count);
        int raidedID = -1;
        if(jinroSelectTargetIDList.Count != 0)
            raidedID = jinroSelectTargetIDList[JinroSelectIndex];

        //狩人に守られていた場合は襲撃失敗(通常１人だが特殊ルールも考え、全員が別々に守れる
        bool isSaved = false;
        foreach(int hunterProtectID in hunterProtectedTargetIDList)
        {
            if (hunterProtectID == raidedID)
            {
                isSaved = true;
                break;
            }
        }
        if(!isSaved)
        {
            myPV.RPC("SyncDeadIfRaided",PhotonTargets.All, raidedID);
            boardPlayerInfoList.UpdateDeadInfo(raidedID, true);
            boardPlayerInfoListForResult.UpdateDeadInfo(raidedID, true);
        }
        //ボードの更新
        string raidedName = SearchByID(raidedID).PlayerName();
        morningPhase.BoardSetUp(raidedName, !isSaved);
        myPV.RPC("SyncSuspectedCountEachPlayer", PhotonTargets.All);
        //各プレイヤーのインフォボード更新
        myPV.RPC("SyncAliveCount", PhotonTargets.All, hostOnly_GameInfo.AliveMemberNumber());
    }

    /// <summary>
    /// 直近の処刑者IDを返す
    /// </summary>
    /// <returns></returns>
    public int ExcutionPlayerID()
    {
        return excutionPlayerID;
    }

    /// <summary>
    /// 生存プレイヤーインフォの最初のセットアップ
    /// </summary>
    private void AlivePlayerInfoFirstSetUp()
    {
        alivePlayerListWithOutMe = new List<PLAYER_Info>();
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
        {
            if(go != myPlayerPrefab)
                alivePlayerListWithOutMe.Add(go.GetComponent<PLAYER_Info>());
        }
    }

    /// <summary>
    /// 自分を除く生存プレイヤーを返す
    /// </summary>
    /// <returns></returns>
    public List<PLAYER_Info> AlivePlayerInfoWithoutMe()
    {
        SetAlivePlayerInfoWithoutMe();
        return alivePlayerListWithOutMe;
    }

    /// <summary>
    /// 自分を除く生存プレイヤーをセットする(死亡者、切断者をリストから除く）
    /// </summary>
    public void SetAlivePlayerInfoWithoutMe()
    {
        for(int i= 0;i<alivePlayerListWithOutMe.Count;i++)
        {
            if(alivePlayerListWithOutMe[i] ==null)
            {
                alivePlayerListWithOutMe.RemoveAt(i);
                continue;
            }
            if(alivePlayerListWithOutMe[i].IsDead())
            {
                alivePlayerListWithOutMe.RemoveAt(i);
                continue;
            }
        }
    }

    /// <summary>
    ///　全プレイヤーからID検索をかける
    /// </summary>
    /// <param name="_id"></param>
    /// <returns></returns>
    public PLAYER_Info SearchByID(int _id)
    {
        foreach( GameObject _infoObject in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (_infoObject.GetComponent<PLAYER_Info>().PlayerID() == _id)
                return _infoObject.GetComponent<PLAYER_Info>();
        }

        Debug.Log("Not found..." + gameObject.name);
        return null;
    }

    public int MyID()
    {
        return myPlayerInfo.PlayerID();
    }

    /// <summary>
    /// 処刑者を決定する    //今回は二人以上同じ数だった場合処刑無し　今後は決選投票を実装予定
    /// </summary>
    public void HOSTONLY_AggregateVote()
    {
        List<int> recieveVoteList = new List<int>();
        List<PLAYER_Info> alivePlayersList = hostOnly_GameInfo.AliveAllMemberInfo();
        //投票の集計 (recieveVoteListの配列番号と alivePlayersListの配列番号は1:1対応
        foreach (PLAYER_Info recievePlayer_info in alivePlayersList)
        {
            recieveVoteList.Add(0);
            foreach(PLAYER_Info votePlayer_info in alivePlayersList)
            {
                //投票先が一致した場合投票
                if (votePlayer_info.VotingDestinationID() == recievePlayer_info.PlayerID())
                    recieveVoteList[recieveVoteList.Count - 1]++;
            }
        }

        int maxVote = 0;

        //最大受票数の検索
        foreach(int recieveVote in recieveVoteList)
        {
            if (recieveVote > maxVote)
                maxVote = recieveVote;
        }

        //同じ投票数の数
        int tieCount = 0;
        PLAYER_Info targetInfo = new PLAYER_Info();
        //処刑者の決定
        for(int i= 0;i<recieveVoteList.Count;i++)
        {
            if(recieveVoteList[i] == maxVote)
            {
                targetInfo = alivePlayersList[i];
                tieCount++;
            }
            if(tieCount > 1)
            {
                //現在はかぶったら処刑無し
                myPV.RPC("SyncExcutionPlayerID", PhotonTargets.All, -1);
                return;
            }
        }

        //観戦ボードの更新
        boardPlayerInfoList.UpdateDeadInfo(targetInfo.PlayerID(), true);
        boardPlayerInfoListForResult.UpdateDeadInfo(targetInfo.PlayerID(), true);
        myPV.RPC("SyncExcutionPlayerID", PhotonTargets.All,targetInfo.PlayerID());
    }

    public void HOSTONLY_SceneMoveSetting(DataBase.Phase _phase, bool isShowHeadMesh)
    {
        if(!myPV.isMine)
        {
            Debug.Log("ホスト以外がアクセスしています");
            return;
        }
        //経過日数の更新
        if(_phase == DataBase.Phase.MORNING)
        {
            hostOnly_GameInfo.AddElapsedDayCount();
            myPV.RPC("SyncDayCount", PhotonTargets.All,hostOnly_GameInfo.ElapsedDayCount());
        }
        myPV.RPC("SyncAllMembersPhase", PhotonTargets.All, _phase);
        myPV.RPC("SyncPhase", PhotonTargets.All, _phase);
        //全プレイヤーの移動および表示(死亡者は観戦ルームへ移動
        myPV.RPC("SyncAllPlayerMoveAndFade", PhotonTargets.All);
        HOSTONLY_SetLiveCamera();
        myPV.RPC("SyncAllAlivePlayerShowHeadAndBody", PhotonTargets.All, isShowHeadMesh);
        myPV.RPC("SyncAllAlivePlayerShowNamePlate", PhotonTargets.All, isShowHeadMesh);  //※頭と一緒に表示させている
    }

    /// <summary>
    /// 観戦ルームのインフォボードの死亡者情報更新
    /// </summary>
    public void HOSTONLY_UpdateDeadInfoToLookingRoomInfoBoard()
    {
        foreach(PLAYER_Info playerInfo in hostOnly_GameInfo.DeadAllMemberInfo())
        {
            boardPlayerInfoList.UpdateDeadInfo(playerInfo.PlayerID(), true);
            boardPlayerInfoListForResult.UpdateDeadInfo(playerInfo.PlayerID(), true);
        }
    }

   /// <summary>
   /// ライブカメラを所定位置にセットする
   /// </summary>
    public void HOSTONLY_SetLiveCamera()
    {
        myPV.RPC("SyncSetLiveCamera", PhotonTargets.All);
    }

    /// <summary>
    /// 投票先を反映させる
    /// </summary>
    /// <param name="_id"></param>
    public void SetVotingDestination(int _id)
    {
        myPlayerInfo.SetVotingDestinationID(_id);
    }

    /// <summary>
    /// 夜の行動先を反映させる
    /// </summary>
    /// <param name="_id"></param>
    public void SetNightActDestination(int _id)
    {
        myPlayerInfo.SetNightActDestination(_id);
    }

    public DataBase.Roll MyRoll()
    {
        return myPlayerInfo.MyRoll();
    }

    /// <summary>
    /// 選択中のオブジェクトを返却
    /// </summary>
    /// <returns></returns>
    public GameObject MySelectObject()
    {
        return myPlayerInfo.MyClickObject();
    }

    /// <summary>
    /// 全体のフェーズを返却
    /// </summary>
    /// <returns></returns>
    public DataBase.Phase AllMembersPhase()
    {
        return allMembersPhase;
    }

    /// <summary>
    /// 場面転換時にフェード処理を行う　現在はブラックアウトのみ
    /// </summary>
    /// <param name="_fade"></param>
    public void Fade(bool _fade)
    {
        myPlayerInfo.ActiveMyCamera(_fade);
    }

    /// <summary>
    /// プレイヤーのフェーズをセットし、ほかのプレイヤーシーンにも反映させる
    /// </summary>
    /// <param name="_phase"></param>
    public void SetPlayerPhase(DataBase.Phase _phase)
    {
        myPlayerInfo.SetMyPhase(_phase);
    }

    /// <summary>
    /// 一秒ごとに残り時間を更新させるホスト専用関数
    /// </summary>
    private void HOSTONLY_InvokeRepeatTimerCount()
    {
        if(allMembersPhase == DataBase.Phase.NOON)
        {
            int noonRestTime = hostOnly_GameInfo.NoonRestTime();
            noonPhase.HOSTONLY_UpdateTimerText(noonRestTime);
            if (noonRestTime <= 0)
            {
                myPV.RPC("SyncAllPlayerPhase", PhotonTargets.All, DataBase.Phase.VOTETIME);
                CancelInvoke("HOSTONLY_InvokeRepeatTimerCount");
            }
        }
        if(allMembersPhase == DataBase.Phase.NIGHT)
        {
            int nightRestTime = hostOnly_GameInfo.NightRestTime();
            nightPhase.HOSTONLY_UpdateTimerText(nightRestTime);
            if (nightRestTime <= 0)
            {
                myPV.RPC("SyncAllPlayerPhase", PhotonTargets.All, DataBase.Phase.MORNING);
                CancelInvoke("HOSTONLY_InvokeRepeatTimerCount");
            }
        }
    }

    /// <summary>
    /// 役職一覧をインフォボックスにセットする
    /// </summary>
    private void HOSTONLY_SetRollList()
    {
        List<DataBase.Roll> _rollList = new List<DataBase.Roll>();
        foreach(PLAYER_Info _info in hostOnly_GameInfo.AliveAllMemberInfo())
        {
            _rollList.Add(_info.MyRoll());
        }
        _rollList.Sort();
        DataBase tmpDatabase = new DataBase();
        string rollListText = tmpDatabase.RollName(_rollList[0]) + "    ";
        for (int i = 1; i < _rollList.Count; i++)
        {
            if (i % 4 == 0) rollListText += '\n';

            rollListText += tmpDatabase.RollName(_rollList[i]) + "    ";
        }
        myPV.RPC("SyncRollList", PhotonTargets.All,rollListText);
    }

    /// <summary>
    /// プレイヤーインフォリストをシャッフルする
    /// </summary>
    /// <returns></returns>
    public List<PLAYER_Info> ShufflePlayerInfo(List<PLAYER_Info> origin_Info)
    {
        if(origin_Info.Count <=1)
        {
            return origin_Info;
        }
        PLAYER_Info tmp_Info;
        int firstRandIndex, secondRandIndex;
        for(int i= 0;i<20;i++)
        {
            
            firstRandIndex = Random.Range(0, origin_Info.Count);
            do
            {
                secondRandIndex = Random.Range(0, origin_Info.Count);
            } while (firstRandIndex == secondRandIndex);

            tmp_Info = origin_Info[firstRandIndex];
            origin_Info[firstRandIndex] = origin_Info[secondRandIndex];
            origin_Info[secondRandIndex] = tmp_Info;
        }

        return origin_Info;
    }

    /// <summary>
    /// 最も多いIDを返す
    /// </summary>
    /// <param name="_idList"></param>
    /// <returns></returns>
    private List<int> SelectMostCommonID(List<int> _idList)
    {
        if (_idList.Count == 1)
            return _idList;

        int IDCount = 0;
        int currentIDCount;
        List<int> mostCommonIDList = new List<int>();
        for(int i= 0;i<_idList.Count;i++)
        {
            currentIDCount = 1;
            for(int j = 1;j<_idList.Count; j++)
            {
                if(_idList[i] == _idList[j])
                {
                    currentIDCount++;
                }
            }
            if (currentIDCount == IDCount)
            {
                mostCommonIDList.Add(_idList[i]);
            }
            if (currentIDCount > IDCount)
            {
                mostCommonIDList.Clear();
                mostCommonIDList.Add(_idList[i]);
                IDCount = currentIDCount;
            }
        }

        return mostCommonIDList;
    }

    /// <summary>
    /// ゲームの終了条件を満たしているかチェック
    /// </summary>
    /// <returns></returns>
    public bool HOSTONLY_CheckGameEnd()
    {
        //生存者全員がカルト信者だった場合、カルトリーダーの勝利(カルトリーダーが少なくとも一人生存)
        if(hostOnly_GameInfo.IsAllAliveMemberCult())
        {
            winCamp = DataBase.Camp.CULT;
            return true;
        }
        //人狼が全体の半分を占めてるかそれ以上だった場合人狼陣営勝利
        if (hostOnly_GameInfo.AliveWereWolfCount() * 2 >= hostOnly_GameInfo.AliveMemberNumber())
        {
            winCamp = DataBase.Camp.WEREWOLF;
            return true;
        }
        else if (hostOnly_GameInfo.AliveWereWolfCount() == 0)
        {
            winCamp = DataBase.Camp.VILLAGE;
            return true;
        }
        else
            return false;
    }

    /// <summary>
    /// リザルト処理を行う
    /// </summary>
    public void HOSTONLY_ResultSetting(DataBase.Camp _winCamp)
    {
        HOSTONLY_UpdateDeadInfoToLookingRoomInfoBoard();
        resultPhase.HOSTONLY_BoardSetUp(_winCamp);
        HOSTONLY_SceneMoveSetting(DataBase.Phase.RESULT, false);
    }

    /// <summary>
    /// ネットワーク切断時の処理
    /// </summary>
    public void DisconnectProcess()
    {
        if(isHost)
        {
            Destroy(hostOnly_GameInfo.gameObject);
        }
        PhotonNetwork.Destroy(myPlayerInfo.gameObject);
        PhotonNetwork.Disconnect();
    }

    /// <summary>
    /// ゲーム中のキャラクターリストを返却
    /// </summary>
    /// <returns></returns>
    public CharacterList GameCharacterList()
    {
        return charaListScript;
    }

    /// <summary>
    /// 太陽の向きをセットする
    /// </summary>
    public void SetLightDirection(DataBase.Phase _phase)
    {
        DataBase.DayTime _dayTime;

        switch(_phase)
        {
            case DataBase.Phase.BRIEFINGROOM:
            case DataBase.Phase.VRSETTING:
            case DataBase.Phase.MORNING:
                _dayTime = DataBase.DayTime.MORNING;
                break;
            case DataBase.Phase.NOON:
                _dayTime = DataBase.DayTime.NOON;
                break;
            case DataBase.Phase.EVENING:
                _dayTime = DataBase.DayTime.EVENING;
                break;
            case DataBase.Phase.NIGHT:
                _dayTime = DataBase.DayTime.NIGHT;
                break;
            default:
                _dayTime = DataBase.DayTime.MORNING;
                break;
        }

        Vector3 defaulutPos = Sun.gameObject.transform.eulerAngles;
        Sun.gameObject.transform.eulerAngles = new Vector3((int)_dayTime, defaulutPos.y, defaulutPos.z);
    }

    [PunRPC]
    private void SyncSetLiveCamera()
    {
        switch (allMembersPhase)
        {
            case DataBase.Phase.MORNING:
                lookingRoom.LiveCameraMove(morningPhase.LiveCameraPos());
                break;
            case DataBase.Phase.NOON:
                lookingRoom.LiveCameraMove(noonPhase.LiveCameraPos());
                break;
            case DataBase.Phase.EVENING:
                lookingRoom.LiveCameraMove(eveningPhase.LiveCameraPos());
                break;
            default:
                Debug.Log("設定されていないフェーズでカメラがセットされようとしています");
                break;
        }
    }

    [PunRPC]
    private void SyncAllPlayerMoveAndFade()
    {
        List<Transform> _spownPoints = new List<Transform>();

        switch(AllMembersPhase())
        {
            case DataBase.Phase.MORNING:
                _spownPoints = morningPhase.SpownList();
                break;
            case DataBase.Phase.NOON:
                _spownPoints = noonPhase.SpownList();
                break;
            case DataBase.Phase.VOTETIME:
                _spownPoints = noonPhase.VoteSpownPoint();
                break;
            case DataBase.Phase.EVENING:
                _spownPoints = eveningPhase.SpownList();
                break;
            case DataBase.Phase.NIGHT:
                _spownPoints = nightPhase.SpownList();
                break;
            case DataBase.Phase.RESULT:
                _spownPoints = resultPhase.SpownList();
                break;
        }

        if (!myPlayerInfo.IsDead() || (allMembersPhase == DataBase.Phase.RESULT))
        {
            if (_spownPoints.Count == 1)
            {
                myPlayerInfo.MoveAndFade(_spownPoints[0]);
            }
            else{
                myPlayerInfo.MoveAndFade(_spownPoints[myPlayerInfo.PlayerID()-1]);
            }
        }else
        {
            myPlayerInfo.MoveAndFade(lookingRoom.DeadPos());
        }
    }

    [PunRPC]
    private void SyncAllPlayerPhase(DataBase.Phase _phase)
    {
        myPlayerInfo.SetMyPhase(_phase);
    }

    [PunRPC]
    private void SyncAllMembersPhase(DataBase.Phase _phase)
    {
        allMembersPhase = _phase;
    }

    [PunRPC]
    private void SyncAllAlivePlayerShowHeadAndBody(bool isShow)
    {
        if (!myPlayerInfo.IsDead())
        {
            myPlayerInfo.ShowHeadMesh(isShow);
            myPlayerInfo.ShowBodyMesh(isShow);
        }
    }

    [PunRPC]
    private void SyncAllAlivePlayerShowNamePlate(bool isShow)
    {
        if (!myPlayerInfo.IsDead())
            myPlayerInfo.ShowNamePlate(isShow);
    }

    [PunRPC]
    private void SyncDayCount(int _dayCount)
    {
        myPlayerInfo.SetDayCountToInfoBox(_dayCount);
    }

    [PunRPC]
    private void SyncPhase(DataBase.Phase _phase)
    {
        myPlayerInfo.SetPhaseToInfoBox(_phase);
    }
	
    [PunRPC]
    private void SyncAliveCount(int _aliveCount)
    {
        myPlayerInfo.SetAliveCountToInfoBox(_aliveCount);
    } 
   
    [PunRPC]
    private void SyncAllPlayerRecieveVoteReset()
    {
        myPlayerInfo.InvalidVostingDestinationID();
    }

    [PunRPC]
    private void SyncAllPlayerNightActReset()
    {
        myPlayerInfo.InvalidNightActDestination();
    }


    [PunRPC]
    private void SyncExcutionPlayerID(int _id)
    {
        excutionPlayerID = _id;
    }

    [PunRPC]
    private void SyncIfExcusion()
    {
        if(myPlayerInfo.PlayerID() == excutionPlayerID)
            myPlayerInfo.DeadProcess();
    }

    [PunRPC]
    private void SyncToCultIfSelect(int _id)
    {
        if (myPlayerInfo.PlayerID() == _id)
            myPlayerInfo.ToCult();
    }

    [PunRPC]
    private void SyncDeadIfRaided(int _targetID)
    {
        if (_targetID == myPlayerInfo.PlayerID())
            myPlayerInfo.DeadProcess();
    }

    [PunRPC]
    private void SyncRollList(string _rollListText)
    {
        myPlayerInfo.SetRollToInfoBox(_rollListText);
    }

    [PunRPC]
    private void SyncSuspectedCountEachPlayer()
    {
        int suspectedCount = 0;
        foreach(PLAYER_Info _playerInfo in alivePlayerListWithOutMe)
        {
            if (_playerInfo.IsNoSkillPlayer())
            {
                if (_playerInfo.NightActDestination() == myPlayerInfo.PlayerID())
                    suspectedCount++;
            }
        }

        myPlayerInfo.SetSuspectedCount(suspectedCount);
    }

    [PunRPC]
    private void SyncShowSkipImage(bool _isShow)
    {
        myPlayerInfo.SkipProcess(!_isShow);
    }

    [PunRPC]
    private void SyncSetHeadMesh()
    {
        myPlayerInfo.SetMyHeadMesh(charaListScript.CharaMeshByIndex(myPlayerInfo.PlayerID()));
    }

}
