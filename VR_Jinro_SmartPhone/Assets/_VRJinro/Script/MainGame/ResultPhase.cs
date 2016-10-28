using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(PhotonView))]
public class ResultPhase : Photon.MonoBehaviour {
    [SerializeField]
    MainGameManager mgManager;
    [SerializeField]
    private Text winCampText;
    [SerializeField]
    private GameObject endButton;
    [SerializeField]
    private List<Transform> spownList;

    public List<Transform> SpownList()
    {
        return spownList;
    }

    private PhotonView myPV;
    
    private void Awake()
    {
        myPV = GetComponent<PhotonView>();
    }

    void Update()
    {
        if(mgManager.AllMembersPhase() == DataBase.Phase.RESULT)
        {
            if(mgManager.MySelectObject() == endButton)
            {
                mgManager.DisconnectProcess();
                GetComponent<SceneLoad>().LoadScene("Setting");
            }
        }
    }

    /// <summary>
    /// ボードのセットアップを行う
    /// </summary>
    /// <param name="_winCamp"></param>
    public void HOSTONLY_BoardSetUp(DataBase.Camp _winCamp)
    {
        myPV.RPC("SyncBoardSetUp", PhotonTargets.All, _winCamp);
    }

    [PunRPC]
    private void SyncBoardSetUp(DataBase.Camp _winCamp)
    {
        DataBase tmpDatabase = new DataBase();
        winCampText.text = tmpDatabase.CampName(_winCamp);
    }
}
