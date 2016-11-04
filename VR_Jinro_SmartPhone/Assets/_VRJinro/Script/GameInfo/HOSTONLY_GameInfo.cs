using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HOSTONLY_GameInfo : Photon.MonoBehaviour {

    private int noonTime = 90;  //Second
    private int nightTime = 60; //Second
    //終了時間
    private float noonEndTime;
    private float nightEndTime;
    private int elapsedDayCount = 0;
    [Header("Read Only.")]
    [SerializeField]
    private List<GameObject> playerList = new List<GameObject>();

    private List<DataBase.Roll> gameRollList = new List<DataBase.Roll>();

    private RoomOptions roomOptions;

    /// <summary>
    /// RoomCreateに部屋を作るプロセスを渡す
    /// </summary>
    public void CreateRoom()
    {
        roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 12;
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        GameObject.FindGameObjectWithTag("Manager").GetComponent<RoomCreate>().CreateRoom(roomOptions,gameRollList.Count);
    }

    /// <summary>
    /// 部屋をオープンにするか
    /// </summary>
    public void IsOpenRoom(bool _isOpen)
    {
        roomOptions.IsOpen = _isOpen;
        roomOptions.IsVisible = _isOpen;
    }
   
	private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    /// <summary>
    /// ゲーム開始時のセッティングを行う(全員そろってから)
    /// </summary>
    public void StartSetting()
    {
        //TestAssignRollToPlayer(); //Debug用
        AssignRollToPlayer();
    }

    /// <summary>
    /// ホストの設定で昼時間と夜時間をセットする
    /// </summary>
    /// <param name="_noonTime"></param>
    /// <param name="_nightTime"></param>
    public void InitGameTime(int _noonTime,int _nightTime)
    {
        noonTime = _noonTime;
        nightTime = _nightTime;
    }

    /// <summary>
    /// 作成したロールリストを登録する
    /// </summary>
    /// <param name="_rollList"></param>
    public void AssignRoll(List<DataBase.Roll> _rollList)
    {
        gameRollList = new List<DataBase.Roll>(_rollList);
    }

    /// <summary>
    /// プレイヤープレファブをリストにセットする
    /// </summary>
    public void SetPlayerPrefabs()
    {
        playerList = new List<GameObject>();
        GameObject[] playerPrefabs = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject go in playerPrefabs)
        {
            playerList.Add(go);
        }
    }

    /// <summary>
    /// 生存している人狼の数を返す
    /// </summary>
    /// <returns></returns>
    public int AliveWereWolfCount()
    {
        int wereWolfCount = 0;
        PLAYER_Info _info;
        foreach(GameObject _player in playerList)
        {
            _info = _player.GetComponent<PLAYER_Info>();
            if (_info.IsWereWolf() && !_info.IsDead())
                wereWolfCount++;
        }

        return wereWolfCount;
    }

    /// <summary>
    /// Debug用、指定の役職を割り当てる
    /// </summary>
    private void TestAssignRollToPlayer()
    {
        playerList[0].GetComponent<PLAYER_Info>().AssignRoll(DataBase.Roll.WEREWOLF);
        playerList[1].GetComponent<PLAYER_Info>().AssignRoll(DataBase.Roll.NURSES);
        playerList[2].GetComponent<PLAYER_Info>().AssignRoll(DataBase.Roll.HUNTER);
        for(int i= 3;i< playerList.Count; i++)
        {
            playerList[i].GetComponent<PLAYER_Info>().AssignRoll(DataBase.Roll.MADMAN);
        }
    }

    /// <summary>
    /// 役職をプレイヤーに割り当てる
    /// </summary>
    private void AssignRollToPlayer()
    {
        //TODO 役職リストをランダムにシャッフルし、各プレイヤーにRPCを利用して役職を割り当てる（GamePlayerPrefabから一致するプレファブに対してRPCコールをするようにうながす）
        int frstIndex, scndIndex;
        DataBase.Roll tmpRoll;
        for (int i= 0;i< 50;i++)
        {
            frstIndex = Random.Range(0, gameRollList.Count);
            scndIndex = Random.Range(0, gameRollList.Count);
            //重複回避するが無限ループをさけるため最大5回まで
            for(int j= 0;j<5;i++)
            {
                if (frstIndex != scndIndex)
                    break;

                scndIndex = Random.Range(0, gameRollList.Count);
            }
            tmpRoll = gameRollList[frstIndex];
            gameRollList[frstIndex] = gameRollList[scndIndex];
            gameRollList[scndIndex] = tmpRoll;
        }
        //役職の割り当て
        for(int i= 0;i<gameRollList.Count;i++)
        {
            playerList[i].GetComponent<PLAYER_Info>().AssignRoll(gameRollList[i]);
        }
    }

   

    /// <summary>
    /// 現在の生存プレイヤー数を返す
    /// </summary>
    /// <returns></returns>
    public int AliveMemberNumber()
    {
        int aliveCount = 0;
        foreach(GameObject go in playerList)
        {
            if (go != null)
            {
                PLAYER_Info _Info = go.GetComponent<PLAYER_Info>();
                if (!_Info.IsDead())
                    aliveCount++;
            }
        }
        return aliveCount;
    }

    /// <summary>
    /// 現在の生存プレイヤーのプレイヤーインフォを返す
    /// </summary>
    /// <returns></returns>
    public List<PLAYER_Info> AliveAllMemberInfo()
    {
        List<PLAYER_Info> _InfoList = new List<PLAYER_Info>();

        foreach (GameObject go in playerList)
        {
            if (go != null)
            {
                PLAYER_Info _Info = go.GetComponent<PLAYER_Info>();
                if (!_Info.IsDead())
                    _InfoList.Add(_Info);
            }
        }

        return _InfoList;
    }

    /// <summary>
    /// 現在の死亡プレイヤーのプレイヤーインフォを返す
    /// </summary>
    /// <returns></returns>
    public List<PLAYER_Info> DeadAllMemberInfo()
    {
        List<PLAYER_Info> _infoList = new List<PLAYER_Info>();

        foreach(GameObject go in playerList)
        {
            if(go != null)
            {
                PLAYER_Info _info = go.GetComponent<PLAYER_Info>();
                if (_info.IsDead())
                    _infoList.Add(_info);
            }
        }

        return _infoList;
    }

    /// <summary>
    /// すべての生存者がカルト信者だった場合Trueを返す
    /// </summary>
    /// <returns></returns>
    public bool IsAllAliveMemberCult()
    {
        List<PLAYER_Info> _aliveList = AliveAllMemberInfo();

        //カルトリーダーが生存していなければ、falseを返す
        bool existCultLeader = false;
        foreach(PLAYER_Info _aliveInfo in _aliveList)
        {
            if (_aliveInfo.MyRoll() == DataBase.Roll.CULTLEADER)
                existCultLeader = true;
        }
        if (!existCultLeader)
            return false;

        int memberCount = _aliveList.Count;
        int cultCount = 0;
        foreach(PLAYER_Info _aliveInfo in _aliveList)
        {
            if(_aliveInfo.IsCult())
            {
                cultCount++;
            }
        }

        if (memberCount == cultCount)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// すべてのメンバー（リストの中の生存メンバー)が同じフェイズにいるか
    /// </summary>
    /// <param name="_phase"></param>
    /// <returns></returns>
    public bool IsAllMemberSamePhase(DataBase.Phase _phase)
    {
        PLAYER_Info _info;
        foreach (GameObject go in playerList)
        {
            _info = go.GetComponent<PLAYER_Info>();
            //一人でも異なっていた場合、偽を返す(※死亡者を除く
            if (_info.MyPhase() != _phase && !_info.IsDead())
                return false;
        }
        return true;
    }

    /// <summary>
    /// 経過日数を返す
    /// </summary>
    /// <returns>経過日数</returns>
    public int ElapsedDayCount()
    {
        return elapsedDayCount;
    }
   
    /// <summary>
    /// 経過日数を一日増やす
    /// </summary>
    public void AddElapsedDayCount()
    {
        elapsedDayCount++;
    }

    /// <summary>
    /// 昼の残り時間を返却
    /// </summary>
    /// <returns></returns>
    public int NoonRestTime()
    {
        int restTime = (int)Mathf.Max(0, (noonEndTime - Time.time));
        return restTime;
    }

    /// <summary>
    /// 夜の残り時間を返却
    /// </summary>
    /// <returns></returns>
    public int NightRestTime()
    {
        int restTime = (int)Mathf.Max(0, (nightEndTime - Time.time));
        return restTime;
    }

    /// <summary>
    /// 昼の相談時間の終了予定時刻
    /// </summary>
    public void SetNoonEndTime()
    {
        noonEndTime = Time.time + noonTime;
    }

    /// <summary>
    /// 夜の行動時間終了予定時刻
    /// </summary>
    public void SetNightEndTime()
    {
        nightEndTime = Time.time + nightTime;
    }

    /// <summary>
    /// 昼の相談時間を強制終了する
    /// </summary>
    public void SkipNoonTime()
    {
        noonEndTime = Time.time;
    }

    /// <summary>
    /// 夜の相談時間を終了する
    /// </summary>
    public void SkipNightTime()
    {
        nightEndTime = Time.time;
    }

    
}
