using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView))]
public class MorningPhase : MonoBehaviour {

    [SerializeField]
    private List<Transform> spownList;
    [SerializeField]
    private MainGameManager mgManager;
    [SerializeField]
    private Text raidedNameText;
    [SerializeField]
    private Text explainText;
    [SerializeField]
    private GameObject ConfirmButtonImage;
    [SerializeField]
    private GameObject ConfirmButton;
    [SerializeField]
    private GameObject raidedPlayer;
    [SerializeField]
    private Transform liveCameraPos;

    private PhotonView myPV;

    public List<Transform> SpownList()
    {
        return spownList;
    }

    void Awake()
    {
        myPV = GetComponent<PhotonView>();
    }

    void Update()
    {
        if(mgManager.AllMembersPhase() == DataBase.Phase.MORNING)
        {
            if(mgManager.MySelectObject() == ConfirmButton)
            {
                ConfirmButtonImage.SetActive(false);
                mgManager.SetPlayerPhase(DataBase.Phase.NOON);
            } 
        }
    }

    public Transform LiveCameraPos()
    {
        return liveCameraPos;
    }

    /// <summary>
    /// ボードをセットする
    /// </summary>
    /// <param name="raidedName"></param>
    /// <param name="existRaidedPlayer"></param>
    public void BoardSetUp(string raidedName,bool existRaidedPlayer)
    {
        myPV.RPC("SyncBoardSetUp", PhotonTargets.All, raidedName, existRaidedPlayer);
    }

    /// <summary>
    /// 初日のボードセットアップ
    /// </summary>
    public void FirstBoardSetUp()
    {
        myPV.RPC("SyncFirstBoardSetUp", PhotonTargets.All);
    }

    [PunRPC]
    private void SyncBoardSetUp(string raidedName,bool existRaidedPlayer)
    {
        DataBase tmpDatabase = new DataBase();
        ConfirmButtonImage.SetActive(true);
        if (existRaidedPlayer)
        {
            raidedNameText.text = raidedName;
            raidedPlayer.SetActive(true);
            //キャラメッシュの反映
        }else
        {
            raidedNameText.text = "いません";
            raidedPlayer.SetActive(false);
        }
        explainText.text = tmpDatabase.morningExplain(existRaidedPlayer);
    }

    [PunRPC]
    private void SyncFirstBoardSetUp()
    {
        DataBase tmpDatabase = new DataBase();
        ConfirmButtonImage.SetActive(true);
        raidedNameText.text = "おおかみ少年";
        raidedPlayer.SetActive(true);
        explainText.text = tmpDatabase.firstMorningExplain();
    }
}
