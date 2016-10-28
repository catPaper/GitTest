using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView))]
public class SetUpPhase : Photon.MonoBehaviour {

    [SerializeField]
    private MainGameManager mgManager;
    [SerializeField]
    private Transform playerSpownPoint;
    [SerializeField]
    private GameObject vrSettingCanvas;
    [SerializeField]
    private GameObject vrSettingButton;
    [SerializeField]
    private GameObject rollCheckCanvas;
    [SerializeField]
    private Text rollName;
    [SerializeField]
    private Text rollExplain;
    [SerializeField]
    private GameObject rollCheckImage;
    [SerializeField]
    private GameObject rollCheckButton;

    private PhotonView myPV;

    void Awake()
    {
        myPV = GetComponent<PhotonView>();
        vrSettingCanvas.SetActive(false);
        rollCheckCanvas.SetActive(false);
    }

    /// <summary>
    /// 各々の役職ボードのセットアップ
    /// </summary>
    private void OwnRollBoardSetUp()
    {
        DataBase.Roll _myRoll = mgManager.MyRoll();
        DataBase tmpDatabase = new DataBase();
        rollName.text = tmpDatabase.RollName(_myRoll);
        rollExplain.text = tmpDatabase.MyRollExplain(_myRoll);
    }

    /// <summary>
    /// セットアップフェーズ時に最初に行う処理
    /// </summary>
    public void SetUpPhase_Start()
    {
        //最初は全員に同じものを表示させる
        myPV.RPC("SyncAllSetUpPhaseStart",PhotonTargets.All);
    }

    public Transform PlayerSpownPoint()
    {
        return playerSpownPoint;
    }


	// Update is called once per frame
	void Update () {
        if (mgManager.AllMembersPhase() == DataBase.Phase.VRSETTING)
        {
           
            if(mgManager.MySelectObject() == vrSettingButton)
            {
                vrSettingCanvas.SetActive(false);
                rollCheckImage.SetActive(true);
                rollCheckCanvas.SetActive(true);
            }
            
            if(mgManager.MySelectObject() == rollCheckButton)
            {
                rollCheckImage.SetActive(false);
                mgManager.SetPlayerPhase(DataBase.Phase.MORNING);
            }
           
        }
	}

    [PunRPC]
    private void SyncAllSetUpPhaseStart()
    {
        vrSettingCanvas.SetActive(true);
        OwnRollBoardSetUp();
    }
}
