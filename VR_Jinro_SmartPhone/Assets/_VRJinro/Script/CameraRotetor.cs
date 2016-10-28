using UnityEngine;
using System.Collections;

/// <summary>
/// PC版でのカメラ操作を管理
/// </summary>
[RequireComponent(typeof(PhotonView))]
public class CameraRotetor : MonoBehaviour {

    [SerializeField]
    private Camera myCamera;
    [SerializeField]
    private GameObject myCameraParent;

    private Vector2 oldCursorPos;
    private Vector2 diff;
    private float moveDistance = 0.5f;
    private PhotonView myPV;
 

    void Awake()
    {
        myPV = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (!myPV.isMine)
            return;

        if(Input.GetKeyDown(KeyCode.Space))
        {
            oldCursorPos = Input.mousePosition;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            diff = ((Vector2)Input.mousePosition - oldCursorPos) * Time.deltaTime * moveDistance;
            myCamera.transform.localRotation *= Quaternion.Euler(-diff.y, 0, 0);
            myCameraParent.transform.localRotation *= Quaternion.Euler(0, diff.x, 0);
           
        }

    }
}
