using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView))]
public class EveningPhase : Photon.MonoBehaviour {
    [SerializeField]
    private MainGameManager mgManager;
    [SerializeField]
    private List<Transform> spownList;
    [SerializeField]
    private Text excutionNameText;
    [SerializeField]
    private GameObject ExcutionPlayer;
    [SerializeField]
    private GameObject ConfirmButtonImage;
    [SerializeField]
    private GameObject ConfirmButton;
    [SerializeField]
    private Transform liveCameraPos;

    

    private PhotonView myPV;

    public List<Transform> SpownList()
    {
        return spownList;
    }

    void Awake()
    {
        myPV =  GetComponent<PhotonView>();
    }

    void Update()
    {
        if (mgManager.AllMembersPhase() == DataBase.Phase.EVENING)
        {
            if (ConfirmButton == mgManager.MySelectObject())
            {
                ConfirmButtonImage.SetActive(false);
                mgManager.SetPlayerPhase(DataBase.Phase.NIGHT);
            }
        }
    }

    public Transform LiveCameraPos()
    {
        return liveCameraPos;
    }

    public void ExcutionInfoSetUp()
    {
        string excusionName = "";
        if (mgManager.ExcutionPlayerID() != -1)
            excusionName = mgManager.SearchByID(mgManager.ExcutionPlayerID()).PlayerName();

        myPV.RPC("SyncExcutionInfo", PhotonTargets.All, excusionName);
    }

    [PunRPC]
    private void SyncExcutionInfo(string excutionName)
    {
        ConfirmButtonImage.SetActive(true);
        if(mgManager.ExcutionPlayerID() != -1)
        {
            excutionNameText.text = excutionName;
            ExcutionPlayer.SetActive(true);
            //TODO 処刑者のヘッドメッシュの設定
        }
        else
        {
            excutionNameText.text = "いません";
            ExcutionPlayer.SetActive(false);
        }
    }
}
