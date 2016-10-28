using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PhotonView))]
public class CharacterList : MonoBehaviour {


    [Header("Resourcesプレファブより参照すること")]
    [SerializeField]
    private List<GameObject> playCharaMeshList;


    private PhotonView myPV;
    //リストのシャッフルを行うため、ゲーム用に別にリストを用意
    [Header("ReadOnly")]
    [SerializeField]
    private List<string> gameCharaMeshList;

    private void Awake()
    {
        myPV = GetComponent<PhotonView>();
        gameCharaMeshList = new List<string>();
        foreach(GameObject playCharaMesh in playCharaMeshList)
        {
            gameCharaMeshList.Add(playCharaMesh.name);
        }
    }

    /// <summary>
    /// リストをデータベースの既定の回数分シャッフルする
    /// </summary>
    public void HOSTONLY_ShuffleList()
    {
        DataBase tmpDatabase = new DataBase();
        int tmpIndex,tmpIndex2;
        string tmpObjectName;
        for(int i= 0;i<tmpDatabase.ShuffleNumber;i++)
        {
            tmpIndex = Random.Range(0, gameCharaMeshList.Count);
            tmpIndex2 = Random.Range(0, gameCharaMeshList.Count);
            //かぶったら別のIndexにするが、再試行は5回まで
            for(int j=0;j<5;j++)
            {
                if (tmpIndex != tmpIndex2)
                    break;

                tmpIndex2 = Random.Range(0, gameCharaMeshList.Count);
            }
            tmpObjectName = gameCharaMeshList[tmpIndex];
            gameCharaMeshList[tmpIndex] = gameCharaMeshList[tmpIndex2];
            gameCharaMeshList[tmpIndex2] = tmpObjectName;
        }
    }

    /// <summary>
    /// メッシュリストを全キャラで共有する
    /// </summary>
    public void HOSTONLY_LinkMeshListAllPlayer()
    {
        myPV.RPC("SyncCharaMeshList", PhotonTargets.All, gameCharaMeshList.ToArray());
    }

    /// <summary>
    /// 指定Indexのキャラプレファブの名前を返す
    /// </summary>
    /// <param name="_index"></param>
    /// <returns></returns>
    public string CharaMeshByIndex(int _index)
    {
        return gameCharaMeshList[_index];
    }

    [PunRPC]
    private void SyncCharaMeshList(string[] _charaList)
    {
        gameCharaMeshList = new List<string>();
        gameCharaMeshList.AddRange(_charaList);
    }
}
