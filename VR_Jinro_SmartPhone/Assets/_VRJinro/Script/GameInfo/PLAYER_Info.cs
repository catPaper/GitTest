using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView))]
public class PLAYER_Info : Photon.MonoBehaviour {


    [SerializeField]
    private Camera myCamera;
    [SerializeField]
    private GameObject MyHead;
    [SerializeField]
    private GameObject MyBody;
    [SerializeField]
    private MyInfoBox myInfoBox;
    [SerializeField]
    private Clicker myClicker;
    [SerializeField]
    private GvrViewer myGvrViewer;
    [SerializeField]
    private GvrReticle myGvrReticle;
    
   


    [Header("Read Only")]
    [SerializeField]
    private string playerName;
    [SerializeField]
    private int playerID;
    [SerializeField]
    private bool isDead = false;
    [SerializeField]
    private DataBase.Phase myPhase = DataBase.Phase.BRIEFINGROOM;
    [SerializeField]
    private GameObject namePlate;
    [SerializeField]
    private Text nameText;
   

    //投票先のID
    private int votingDestinationID;

    //カルト教徒か否か
    private bool isCult;

    //夜の選択先
    [Header("Debug")]
    [SerializeField]
    private int nightActDestination;

    //変数参照用
    private DataBase tmpDataBase = new DataBase();

    //自分を疑っている人数
    private int suspectedCount;

    //時間のスキップに賛成するか（昼の時間で過半数がSkipを選択した場合、スキップ処理）
    private bool isSkip = false;


    //このスクリプトを含むプレイヤープレファブは各々のプレイヤーが自分でPhotonNetwork.Instantiateで生成する各所有物である


    [SerializeField]
    private DataBase.Roll myRoll;

    private bool isVRMode;

    private PhotonView myPV;

    private void Awake()
    {
        nightActDestination = -1;
        isVRMode = false;
        ShowSkipImage(false);
        DontDestroyOnLoad(this.gameObject);
        myPV = GetComponent<PhotonView>();
        SyncShowMyHeadMesh(false);
        SyncShowMyBodyMesh(false);
        myGvrViewer.enabled = false;
        myGvrReticle.enabled = false;
        myCamera.enabled = false;
        ShowInfoBox(false);
        namePlate.SetActive(false);
        isCult = false;

        if (myPV.isMine)
        {
            myGvrViewer.gameObject.SetActive(true);
        }
    }

    public void SetVRMode(bool _isVRMode)
    {
        isVRMode = _isVRMode;
    }

    public GameObject MyClickObject()
    {
        return myClicker.ClickedObject();
    }

    public string PlayerName()
    {
        return playerName;
    }

    public int PlayerID()
    {
        return playerID;
    }

    public DataBase.Roll MyRoll()
    {
        return myRoll;
    }

    /// <summary>
    /// スキップが有効か返す
    /// </summary>
    /// <returns></returns>
    public bool  Skip()
    {
        return isSkip;
    }

    /// <summary>
    /// スキップ賛成処理を行う
    /// </summary>
    public void SkipProcess(bool isSkip)
    {
        SetSkip(isSkip);
        ShowSkipImage(!isSkip);
    }

    /// <summary>
    /// スキップイメージの表示の有無
    /// </summary>
    /// <param name="_isShow"></param>
    private void ShowSkipImage(bool _isShow)
    {
        myInfoBox.ShowSkipImage(_isShow);
    }

    private void SetSkip(bool _isSkip)
    {
        myPV.RPC("SyncSkip", PhotonTargets.All, _isSkip);
    }

    /// <summary>
    /// 疑われている人数をセットする
    /// </summary>
    public void SetSuspectedCount(int _suspectedCount)
    {
        myInfoBox.SetSuspectedCount(_suspectedCount);
    }

    /// <summary>
    /// 自分のフォトンプレイヤー情報をここに記述する
    /// </summary>
    public void AssignPlayerInfo()
    {
        if(myPV.isMine)
            myPV.RPC("SyncPlayerInfo", PhotonTargets.All,PhotonNetwork.player.name,PhotonNetwork.player.ID);
    }

    /// <summary>
    /// 自分より後から参加したものへ情報を共有する
    /// </summary>
    /// <param name="newPlayer"></param>
    public void SendPlayerInfo(PhotonPlayer newPlayer)
    {
        if (myPV.isMine)
            myPV.RPC("SyncPlayerInfo", newPlayer, PhotonNetwork.player.name, PhotonNetwork.player.ID);
    }

    /// <summary>
    /// 自分の現在のフェイズを全プレイヤーに共有する
    /// </summary>
    /// <param name="_phase"></param>
    public void SetMyPhase(DataBase.Phase _phase)
    {
        if (myPV.isMine)
            myPV.RPC("SyncMyPhase", PhotonTargets.All, _phase);
    }

    public DataBase.Phase MyPhase()
    {
        return myPhase;
    }

    /// <summary>
    /// ネームプレートが表示されているか
    /// </summary>
    /// <returns></returns>
    public bool ShowNamePlate()
    {
        return namePlate.activeSelf;
    }

    /// <summary>
    /// カメラが有効かどうか  
    /// </summary>
    /// <returns></returns>
    public bool IsActiveMyCamera()
    {
        return myCamera.enabled;
    }

    /// <summary>
    /// カメラを有効にするかどうか(自分のゲームシーンのみ)
    /// </summary>
    /// <param name="isActive"></param>
    public void ActiveMyCamera(bool isActive)
    {
        if(myPV.isMine)
        {
            myCamera.enabled = isActive;

            if(isActive)
            {
                myGvrViewer.enabled = true;
                
            }
            else
            {
                myGvrViewer.enabled = false;
                myGvrReticle.enabled = false;
            }
        
        }  
    }

    /// <summary>
    /// 受け取った名前をもとにヘッドメッシュをセットする
    /// </summary>
    /// <param name="_prefabName"></param>
    public void SetMyHeadMesh(string _prefabName)
    {
        myPV.RPC("SyncSetHeadMesh", PhotonTargets.All, _prefabName);
    }

    public void ReCenter()
    {
        myGvrViewer.Recenter();
        Vector3 defaultCameraPos = myCamera.transform.localEulerAngles;
        myCamera.transform.localEulerAngles = new Vector3(defaultCameraPos.x, 0, defaultCameraPos.z);
        myCamera.transform.parent.localEulerAngles = Vector3.zero;
#if (UNITY_ANDROID || UNITY_IOS)
                myGvrViewer.VRModeEnabled = isVRMode;
                myGvrReticle.enabled = true;
#endif
#if (UNITY_EDITOR)
        myGvrViewer.VRModeEnabled = false;
        myGvrReticle.enabled = true;
#endif
#if (UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX)
        myCamera.transform.localEulerAngles = Vector3.zero;
#endif
    }



    /// <summary>
    /// 割り当てられたロール(すべてのプレイヤーの自分のプレファブを更新)
    /// </summary>
    /// <param name="roll"></param>
    public void AssignRoll(DataBase.Roll roll)
    {
        myPV.RPC("SyncMyRoll", PhotonTargets.All,roll);
        //カルトリーダーの場合自分をカルト信者として設定する
        if (myRoll == DataBase.Roll.CULTLEADER)
            ToCult();
    }

    /// <summary>
    /// すべてのプレイヤー画面(自分以外)で頭を表示させるか
    /// </summary>
    /// <param name="_show"></param>
    public void ShowHeadMesh(bool _show)
    {
        MyHead.SetActive(false);
        myPV.RPC("SyncShowMyHeadMesh", PhotonTargets.Others, _show);        
    }

    /// <summary>
    /// すべてのプレイヤー画面(自分以外)で体を表示させるか
    /// </summary>
    /// <param name="_show"></param>
    public void ShowBodyMesh(bool _show)
    {
        MyBody.SetActive(false);
        myPV.RPC("SyncShowMyBodyMesh", PhotonTargets.Others, _show);
    }

    /// <summary>
    /// すべてのプレイヤー画面(自分以外)で名前タグを表示させるか
    /// </summary>
    /// <param name="_show"></param>
    public void ShowNamePlate(bool _show)
    {
        namePlate.SetActive(false);
        myPV.RPC("SyncShowNamePlate", PhotonTargets.Others, _show);
    }

    /// <summary>
    /// ネームプレートを見やすいように自分の方に向ける
    /// </summary>
    /// <param name="myPos"></param>
    public void SetNamePlateLookPos(Transform myPos)
    {
        namePlate.transform.LookAt(myPos);
        namePlate.transform.eulerAngles = new Vector3(0, namePlate.transform.eulerAngles.y, 0);
    }

    /// <summary>
    /// すべてのプレイヤーをフェードして移動させる
    /// </summary>
    /// <param name="_pos"></param>
    public void MoveAndFade(Transform _pos)
    {
        StartCoroutine("MoveAndFadeCoroutine", _pos);
    }

    /// <summary>
    /// 人狼かどうか
    /// </summary>
    /// <returns>true=人狼 / false=人間</returns>
    public bool IsWereWolf()
    {
        return (myRoll == DataBase.Roll.WEREWOLF);
    }

    /// <summary>
    /// 死亡しているかどうか
    /// </summary>
    /// <returns>true=死亡 / false=生存</returns>
    public bool IsDead()
    {
        return isDead;
    }

    /// <summary>
    /// 生存/死亡情報を全プレイヤーの自オブジェクトで共有する
    /// </summary>
    /// <param name="isDead"></param>
    private void SetMyDeadInfo(bool _isDead)
    {
        if(myPV.isMine)
        {
            myPV.RPC("SyncMyDeadInfo",PhotonTargets.All,_isDead);
        }
    }

    /// <summary>
    /// 死亡時の処理
    /// </summary>
    public void DeadProcess()
    {
        SetMyDeadInfo(true);
        ShowHeadMesh(false);
        ShowBodyMesh(false);
        ShowNamePlate(false);
    }

    /// <summary>
    /// インフォボックスを表示/非表示にする　※普通、自分の所有しているプレイヤーに対して行う
    /// </summary>
    /// <param name="isShow">表示するか</param>
    public void ShowInfoBox(bool isShow)
    {
        if (!myPV.isMine)
        {
            Debug.Log("他のプレイヤーのインフォボックスを表示/非表示しています。");
        }
        myInfoBox.gameObject.SetActive(isShow);
    }

    /// <summary>
    /// 初回のインフォボックス設定 (生存カウントはMainGameManagerにて行う)
    /// </summary>
    public void FirstInfoBoxSetting()
    {
        SetNameToInfoBox();
        SetNameToNamePlate();
        SetDayCountToInfoBox(0);
        SetPhaseToInfoBox(DataBase.Phase.VRSETTING);
    }

    /// <summary>
    /// プレイヤー名をにセットする
    /// </summary>
    private void SetNameToNamePlate()
    {
        myPV.RPC("SyncNamePlate", PhotonTargets.All, playerName);
    }

   

    /// <summary>
    /// プレイヤー名をインフォボックスにセットする
    /// </summary>
    private void SetNameToInfoBox()
    {
        myInfoBox.SetName(playerName);
    }

    /// <summary>
    /// 現在の経過日数をインフォボックスにセットする
    /// <param name="elapsedDay">経過日数</param>
    /// </summary>
    public void SetDayCountToInfoBox(int elapsedDay)
    {
        myInfoBox.SetDayNumber(elapsedDay);
    }

    /// <summary>
    /// 現在の時間帯（フェーズ）をインフォボックスにセットする
    /// </summary>
    /// <param name="phase"></param>
    public void SetPhaseToInfoBox(DataBase.Phase phase)
    {
        myInfoBox.SetPhase(phase);
    }

    /// <summary>
    /// 役職をインフォボックスにセットする
    /// </summary>
    /// <param name="_rollList"></param>
    public void SetRollToInfoBox(string _rollListText)
    {
        myInfoBox.SetRollList(_rollListText);
    }


    /// <summary>
    /// 生存人数をインフォボックスにセットする
    /// </summary>
    /// <param name="aliveMember"></param>
    public void SetAliveCountToInfoBox(int aliveMember)
    {
        myInfoBox.SetAliveCount(aliveMember);
    }

   
    /// <summary>
    /// 投票先
    /// </summary>
    /// <returns></returns>
    public int VotingDestinationID()
    {
        return votingDestinationID;
    }

    /// <summary>
    /// 夜の選択先
    /// </summary>
    /// <returns></returns>
    public int NightActDestination()
    {
        return nightActDestination;
    }

    /// <summary>
    /// 投票先を無効票にする
    /// </summary>
    public void InvalidVostingDestinationID()
    {
        myPV.RPC("SyncVotingDestinationID", PhotonTargets.All, -1);
    }
   
    /// <summary>
    /// 投票情報を全プレイヤーで共有する
    /// </summary>
    /// <param name="_id"></param>
    public void SetVotingDestinationID(int _id)
    {
        myPV.RPC("SyncVotingDestinationID",PhotonTargets.All, _id);
    }

    /// <summary>
    /// 夜の行動先を無効値にする
    /// </summary>
    public void InvalidNightActDestination()
    {
        myPV.RPC("SyncNightActDestination", PhotonTargets.All, -1);
    }

    /// <summary>
    /// 夜の行動先を全プレイヤーで共有する
    /// </summary>
    /// <param name="_targetID"></param>
    public void SetNightActDestination(int _targetID)
    {
        myPV.RPC("SyncNightActDestination", PhotonTargets.All, _targetID);
    }

    /// <summary>
    /// 夜の行動が疑うのみのプレイヤーか（霊媒師も含む）
    /// </summary>
    /// <returns></returns>
    public bool IsNoSkillPlayer()
    {
        switch(myRoll)
        {
            case DataBase.Roll.VILLAGER:
            case DataBase.Roll.NURSES:
            case DataBase.Roll.MADMAN:
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// カルト教徒になり、その情報を全プレイヤーシーンに反映させる
    /// </summary>
    public  void ToCult()
    {
        myPV.RPC("SyncToCult", PhotonTargets.All);
    }

    /// <summary>
    /// カルト信者かどうか
    /// </summary>
    /// <returns></returns>
    public bool IsCult()
    {
        return isCult;
    }

    [PunRPC]
    private void SyncNamePlate(string _name)
    {
        nameText.text = _name;
    }

    [PunRPC]
    private void SyncMyDeadInfo(bool _isDead)
    {
        isDead = _isDead;
    }

    [PunRPC]
    private void SyncMyPhase(DataBase.Phase _myPhase)
    {
        myPhase = _myPhase;
    }

    [PunRPC]
    private void SyncPlayerInfo(string _name, int _id)
    {
        playerName = _name;
        playerID = _id;
    }

    [PunRPC]
    private void SyncMyRoll(DataBase.Roll roll)
    {
        myRoll = roll;
    }

   

    [PunRPC]
    private void SyncShowMyHeadMesh(bool _show)
    {
        MyHead.SetActive(_show);
    }

    [PunRPC]
    private void SyncShowMyBodyMesh(bool _show)
    {
        MyBody.SetActive(_show);
    }

    [PunRPC]
    private void SyncShowNamePlate(bool _show)
    {
        namePlate.SetActive(_show);
    }

    [PunRPC]
    private void SyncVotingDestinationID(int _voteID)
    {
        votingDestinationID = _voteID;
    }

    [PunRPC]
    private void SyncSkip(bool _isSkip)
    {
        isSkip = _isSkip;
    }

    [PunRPC]
    private void SyncNightActDestination(int _targetID)
    {
        nightActDestination = _targetID;
    }

    [PunRPC]
    private void SyncSetHeadMesh(string _headMeshName)
    {
        GameObject _myHeadMesh = Instantiate((GameObject)Resources.Load(_headMeshName), MyHead.transform.position, Quaternion.identity) as GameObject;
        _myHeadMesh.transform.SetParent(MyHead.transform);
        _myHeadMesh.transform.position = MyHead.transform.position;
    }

    [PunRPC]
    private void SyncToCult()
    {
        isCult = true;
        myInfoBox.AddCultInName();
    }

    IEnumerator MoveAndFadeCoroutine(Transform _pos)
    {
        if (myPV.isMine)
        {
            GetComponent<Fade>().fadeMode = Fade.FadeMode.FADEOUT;
            GetComponent<Fade>().FadeStart();
           
        }
        yield return new WaitForSeconds(tmpDataBase.FadeTime);
        if (myPV.isMine)
        {
            GameObject.FindGameObjectWithTag("Manager").GetComponent<MainGameManager>().SetLightDirection(myPhase);
            //タイマー時計をむかせる
            foreach (GameObject _timerClock in GameObject.FindGameObjectsWithTag("TimeClock"))
            {
                _timerClock.transform.LookAt(_pos);
                _timerClock.transform.localEulerAngles = new Vector3(0,_timerClock.transform.eulerAngles.y, 0);
            }
            gameObject.transform.position = _pos.position;
            gameObject.transform.rotation = _pos.rotation;
            GetComponent<Fade>().fadeMode = Fade.FadeMode.FADEIN;

#if (UNITY_ANDROID || UNITY_IOS || UNITY_EDITOR)
            ReCenter();
#endif

        }
        yield return new WaitForSeconds(tmpDataBase.FadeTime);
        if(myPV.isMine)
        {
            //ネームプレートをすべてにむかせる
            foreach (GameObject _player in GameObject.FindGameObjectsWithTag("Player"))
            {
                _player.GetComponent<PLAYER_Info>().SetNamePlateLookPos(_pos);
            }
            GetComponent<Fade>().fadeMode = Fade.FadeMode.DEFAULT;
            GetComponent<Fade>().FadeEnd();

            ActiveMyCamera(true);

        }
    }
}
