using UnityEngine;
using System.Collections;

public class CanvasManager : MonoBehaviour {

    [SerializeField]
    private Canvas userNameCanvas;
    [SerializeField]
    private Canvas settingCanvas;
    [SerializeField]
    private Canvas roomCanvas;


    void Awake()
    {
        //最初に設定するキャンバスを表示する
        OpenUserNameCanvas();
    }

    /// <summary>
    /// すべてのキャンバスを非表示にする
    /// </summary>
    void AllHideCanvas()
    {
        userNameCanvas.enabled = false;
        settingCanvas.enabled = false;
        roomCanvas.enabled = false;
    }

    /// <summary>
    /// ユーザー名設定キャンバスを表示する
    /// </summary>
	public void OpenUserNameCanvas()
    {
        AllHideCanvas();
        userNameCanvas.enabled = true; 
    }

    /// <summary>
    /// ホスト側のゲーム設定キャンバスを表示する
    /// </summary>
    public void OpenSettingCanvas()
    {
        AllHideCanvas();
        settingCanvas.enabled = true;
    }

    /// <summary>
    /// 集合ルームキャンバスを表示する
    /// </summary>
    public void OpenRoomCanvas()
    {
        AllHideCanvas();
        roomCanvas.enabled = true;
    }
}
