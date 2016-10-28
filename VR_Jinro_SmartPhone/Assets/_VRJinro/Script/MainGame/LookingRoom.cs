using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class LookingRoom : MonoBehaviour {


    [SerializeField]
    private GameObject liveCamera;
    [SerializeField]
    private Transform deadPos;

    //TODO 観戦ルームの役職オブジェクトリストの管理

    private PhotonView myPV;

    void Awake()
    {
        myPV = GetComponent<PhotonView>();
    } 

    public Transform DeadPos()
    {
        return deadPos;
    }

    /// <summary>
    /// 全シーンのライブカメラを移動させる
    /// </summary>
    public void LiveCameraMove(Transform _setPos)
    {
        liveCamera.transform.position = _setPos.position;
        liveCamera.transform.rotation = _setPos.rotation;
    }
}
