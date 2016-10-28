using UnityEngine;
using System.Collections;
using UnityEngine.UI;


[RequireComponent(typeof(PhotonView))]
public class BOARD_playerInfo :Photon.MonoBehaviour {

    [SerializeField]
    private Text playerNameText;
    [SerializeField]
    private Text rollNameText;
    [SerializeField]
    private Text deadInfoText;
    [SerializeField]
    private Text cultInfoText;

    private PhotonView myPV;



    //ボードとプレイヤーを一致させるためのプレイヤーID
    private int boardPlayerID;

    private void Awake()
    {
        myPV = GetComponent<PhotonView>();
    }

    /// <summary>
    /// インフォボードを表示させるか
    /// </summary>
    /// <param name="_isShow"></param>
	public void ShowPlayerInfoBoard(bool _isShow)
    {
        myPV.RPC("SyncShowInfoBoard", PhotonTargets.All, _isShow);
    }

    /// <summary>
    /// プレイヤー情報の初期設定
    /// </summary>
    /// <param name="_playerName"></param>
    /// <param name="_rollName"></param>
    /// <param name="_isDead"></param>
	public void SetPlayerInfo(string _playerName,int _playerID,string _rollName,bool _isDead,bool _isCult)
    {
        myPV.RPC("SyncSetPlayerInfo", PhotonTargets.All, _playerName, _playerID, _rollName);
        SetIsCult(_isCult);
        SetDeadInfo(_isDead);
    }

    /// <summary>
    /// プレイヤーIDが引数と一致してるなら真を返す
    /// </summary>
    /// <param name="_playerID"></param>
    /// <returns></returns>
    public bool SamePlayerID(int _playerID)
    {
        return (boardPlayerID == _playerID);
    }

    /// <summary>
    /// カルト信者かどうかを設定する
    /// </summary>
    /// <param name="_isCult"></param>
    public void SetIsCult(bool _isCult)
    {
        myPV.RPC("SyncCultInfo", PhotonTargets.All, _isCult);
    }

    /// <summary>
    /// 死亡情報をセットする
    /// </summary>
    /// <param name="_isDead"></param>
    public void SetDeadInfo(bool _isDead)
    {
        myPV.RPC("SyncDeadInfo", PhotonTargets.All, _isDead);
    }

    [PunRPC]
    private void SyncShowInfoBoard(bool _isShow)
    {
        gameObject.SetActive(_isShow);
    }

    [PunRPC]
    private void SyncDeadInfo(bool _isDead)
    {
        deadInfoText.text = (_isDead ? "死亡" : "生存");
        deadInfoText.color = (_isDead ? Color.red : Color.blue);
    }

    [PunRPC]
    private void SyncCultInfo(bool _isCult)
    {
        cultInfoText.text = (_isCult ? "（カルト信者）" : "");
    }

  

    [PunRPC]
    private void SyncSetPlayerInfo(string _playerName,int _playerID,string _rollName)
    {
        playerNameText.text = _playerName;
        boardPlayerID = _playerID;
        rollNameText.text = _rollName;
    }
}
