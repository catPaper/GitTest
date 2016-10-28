using UnityEngine;
using System.Collections;

/// <summary>
/// カメラのローテを頭と同期する
/// </summary>
public class CameraRoteSyncScript : MonoBehaviour {
    [SerializeField]
    private GameObject myHead;

    private PhotonView myHeadPhotonView;

    void Awake()
    {
        myHeadPhotonView = myHead.GetComponent<PhotonView>();
    }
    
	
	// Update is called once per frame
	void Update () {
	    if(myHeadPhotonView.isMine)
        {
#if (UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX)
            myHead.transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.parent.localEulerAngles.y, 0);
#endif
#if (UNITY_ANDROID || UNITY_IOS || UNITY_EDITOR)
            myHead.transform.rotation = transform.rotation;
#endif
        }
	}
}
