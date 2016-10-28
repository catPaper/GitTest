using UnityEngine;
using System.Collections;

public class Clicker : MonoBehaviour {

    [SerializeField]
    private Camera useCamera;

    private Ray ray;
    private RaycastHit hit;

    private GameObject clickedObject;

   

    private void Update()
    {
       
        if (useCamera.gameObject.activeSelf)
        {
            ray = useCamera.ScreenPointToRay(Input.mousePosition);
            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(ray, out hit, 100))
                {
                    clickedObject = hit.collider.gameObject;
                    Debug.Log("Hit:" + clickedObject.name);
                }
            }
        }
        if (clickedObject == null)
            return;

        if(clickedObject.tag == "SkipTarget")
        {
            PLAYER_Info _playerInfo = GetComponent<PLAYER_Info>();
            if (!_playerInfo.Skip())
                _playerInfo.SkipProcess(true);
        }
        //アイトラッキング　ray = new Ray(useCameraTransform.position, useCameraTransform.rotation * Vector3.forward);
    }

    public GameObject ClickedObject()
    {
        return clickedObject;
    }

    /// <summary>
    /// クリック判定するオブジェクトをセットする
    /// </summary>
    /// <param name="_targetObject"></param>
	public void SetClickedObject(GameObject _targetObject)
    {
        Debug.Log("Set!");
        clickedObject = _targetObject;
    }
}
