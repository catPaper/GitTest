using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LIST_boardPlayerInfo : MonoBehaviour {

    [SerializeField]
    private List<BOARD_playerInfo> boardPlayerInfoList;

   

    


    /// <summary>
    /// 最初のボード設定
    /// </summary>
    /// <param name="_playerList"></param>
    public void InitPlayerList()
    {
        GameObject[] _playerList = GameObject.FindGameObjectsWithTag("Player");
        if(_playerList.Length > 12)
        {
            Debug.Log("現在その数は許容できません");
            return;
        }
        foreach(BOARD_playerInfo boardPlayerInfo in boardPlayerInfoList)
        {
            boardPlayerInfo.ShowPlayerInfoBoard(false);
        }
        DataBase tmpDatabase = new DataBase();
        for(int i= 0;i< _playerList.Length;i++)
        {
            PLAYER_Info _playerInfo = _playerList[i].GetComponent<PLAYER_Info>();
            boardPlayerInfoList[i].ShowPlayerInfoBoard(true);
            boardPlayerInfoList[i].SetPlayerInfo(_playerInfo.PlayerName(), _playerInfo.PlayerID(),tmpDatabase.RollName(_playerInfo.MyRoll()), _playerInfo.IsDead(),_playerInfo.IsCult());
        }
    }

    public void UpdateIsCult(int _playerID,bool _isCult)
    {
        BOARD_playerInfo _boardPlayerInfo = SearchPlayerIDFromBoard(_playerID);
        if(_boardPlayerInfo != null)
        {
            _boardPlayerInfo.SetIsCult(_isCult);
        }
    }

    /// <summary>
    /// プレイヤーインフォボードの生死情報を更新する
    /// </summary>
    /// <param name="_playerID"></param>
    /// <param name="_isDead"></param>
    public void UpdateDeadInfo(int _playerID,bool _isDead)
    {
        BOARD_playerInfo _boardPlayerInfo = SearchPlayerIDFromBoard(_playerID);
        if (_boardPlayerInfo != null)
        {
            _boardPlayerInfo.SetDeadInfo(_isDead);
        }
    }

    /// <summary>
    /// プレイヤーインフォボードから指定のIDを検索し、なければnullを返す
    /// </summary>
    /// <param name="_playerID"></param>
    /// <returns></returns>
    private BOARD_playerInfo SearchPlayerIDFromBoard(int _playerID)
    {
        foreach (BOARD_playerInfo boardPlayerInfo in boardPlayerInfoList)
        {
            if(boardPlayerInfo.SamePlayerID(_playerID))
            {
                return boardPlayerInfo;
            }
        }

        return null;
    }

    

    
}
