using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 投票、占い、襲撃等で選択するターゲットの情報
/// </summary>
public class TargetInfo : MonoBehaviour {

    [SerializeField]
    private Text targetNameText;
    [SerializeField]
    private Transform targetHeadPos;
    [SerializeField]
    private GameObject selectButton;

    private int targetID;

    /// <summary>
    /// ターゲット情報を設定する
    /// </summary>
    /// <param name="_name"></param>
    /// <param name="_id"></param>
    public void TargetSetUp(string _name,int _id)
    {
        SetTargetName(_name);
        SetTargetID(_id);
    }

    /// <summary>
    /// ターゲットネームタグのネームをセットする
    /// </summary>
    /// <param name="name"></param>
    private void SetTargetName(string _name)
    {
        targetNameText.text = _name;
    }

    /// <summary>
    /// ターゲットのIDをセットする
    /// </summary>
    /// <param name="_id"></param>
    private void SetTargetID(int _id)
    {
        targetID = _id;
    }

    /// <summary>
    /// ターゲットを選択するためのボタンを返却する
    /// </summary>
    /// <returns></returns>
    public GameObject SelectButton()
    {
        return selectButton;
    }

    /// <summary>
    /// ターゲットIDの返却
    /// </summary>
    /// <returns></returns>
    public int TargetID()
    {
        return targetID;
    }

    /// <summary>
    /// ターゲットを表示/非表示する
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowTarget(bool isShow)
    {
        gameObject.SetActive(isShow);
    }

    /// <summary>
    /// キャラメッシュをセットする
    /// </summary>
    public void SetCharaMesh(string _charaName)
    {
        //まずは子オブジェクトがあれば削除
        foreach(Transform childObject in targetHeadPos.transform)
        {
            Destroy(childObject.gameObject);
        }

        GameObject head = Instantiate(Resources.Load(_charaName), targetHeadPos) as GameObject;
        head.transform.localPosition = Vector3.zero;
        head.transform.eulerAngles = new Vector3(0, head.transform.eulerAngles.y + 180f, 0);
    }
}
