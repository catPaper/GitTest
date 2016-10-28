using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SelectTarget : MonoBehaviour {

    [SerializeField]
    private List<TargetInfo> TargetList;
    [SerializeField]
    private GameObject targetGroup;
    [SerializeField]
    private Text waitText;
    [SerializeField]
    private GameObject dicisionButton;
    [SerializeField]
    private GameObject dicisionButtonColider;
    [SerializeField]
    private Text resultText;
    [SerializeField]
    private GameObject infoBoard;

    void Awake()
    {
        ShowDicisonButton(false);
    }

    /// <summary>
    /// 他プレイヤー完了待ちテキストを表示するか
    /// </summary>
    /// <param name="_isShow"></param>
    public void ShowWaitText(bool _isShow)
    {
        waitText.gameObject.SetActive(_isShow);
    }

    /// <summary>
    /// 表示するセットアップ
    /// </summary>
    public void ShowSetUp()
    {
        targetGroup.SetActive(true);
        infoBoard.SetActive(true);
        resultText.text = "";
        waitText.gameObject.SetActive(false);
        ShowDicisonButton(false);

    }

    /// <summary>
    /// 霊媒師の結果表示
    /// </summary>
    public void SetNursesResult(PLAYER_Info _info,bool existExcutioner)
    {
        if(existExcutioner)
        {
            resultText.text = _info.PlayerName() +"は" + (_info.IsWereWolf()?"人狼":"人間") + "でした。";
        }else
        {
            resultText.text ="処刑者がいないため能力は使えませんでした。";
        }
    }

    /// <summary>
    /// 関連するすべてのオブジェクトを非表示にする
    /// </summary>
    public void AllGroupHide()
    {
        targetGroup.SetActive(false);
        infoBoard.SetActive(false);
        ShowDicisonButton(false);
    }

    /// <summary>
    /// ターゲットを選択不可にする
    /// </summary>
    public void HideTargets()
    {
        targetGroup.SetActive(false);
    }

    /// <summary>
    /// 選択しているオブジェクトがターゲットリストにふくまれているか確認し、一致した場合IDを返却
    /// 一致しなかった場合-1を返却
    /// </summary>
    /// <param name="selectObj"></param>
    /// <returns></returns>
    public int SearchSelectTargetID(GameObject selectObj)
    {
        foreach(TargetInfo ti in TargetList)
        {
            if(selectObj == ti.SelectButton())
            {
                ShowDicisonButton(true);
                return ti.TargetID();
            }
        }
        return -1;
    }

    /// <summary>
    /// あたり判定のある決定ボタンのオブジェクトを返す
    /// </summary>
    /// <returns></returns>
    public GameObject DicisionButtonColider()
    {
        return dicisionButtonColider;
    }
	
    /// <summary>
    /// 選択完了ボタンを表示/非表示する
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowDicisonButton(bool isShow)
    {
        dicisionButton.SetActive(isShow);
    }

    /// <summary>
    /// 生存プレイヤーの自分を除くターゲットをセットする
    /// </summary>
    /// <param name="_infoList"></param>
    public void SetTargetsSetting(List<PLAYER_Info> _infoList,CharacterList _gameCharaList)
    {
        //すべて非表示
        foreach(TargetInfo ti in TargetList)
        {
            ti.gameObject.SetActive(false);
        }
        //対象オブジェクトのみ値を設定し表示
        for(int i= 0;i<_infoList.Count;i++)
        {
            TargetList[i].TargetSetUp(_infoList[i].PlayerName(), _infoList[i].PlayerID());
            TargetList[i].SetCharaMesh(_gameCharaList.CharaMeshByIndex(_infoList[i].PlayerID()));
            TargetList[i].ShowTarget(true);
        }
    }

    /// <summary>
    /// ターゲット選択時リザルト表示
    /// </summary>
    public void SetResultText(DataBase.Phase _phase,PLAYER_Info _info,DataBase.Roll _myRoll)
    {
        if (_phase == DataBase.Phase.VOTETIME)
        {
            resultText.text = _info.PlayerName() + "に投票します";
        }
        if(_phase == DataBase.Phase.NIGHT)
        {
            switch (_myRoll)
            {
                case DataBase.Roll.VILLAGER:
                case DataBase.Roll.NURSES:
                case DataBase.Roll.MADMAN:
                    resultText.text = _info.PlayerName() + "を疑います";
                    break;
                case DataBase.Roll.DIVINER:
                    resultText.text = _info.PlayerName() + "は" + (_info.IsWereWolf()?"人狼":"人間") + "です。";
                    break;
                case DataBase.Roll.HUNTER:
                    resultText.text = _info.PlayerName() + "を守ります";
                    break;
                case DataBase.Roll.WEREWOLF:
                    resultText.text = _info.PlayerName() + "を襲撃します";
                    break;
                case DataBase.Roll.CULTLEADER:
                    resultText.text = _info.PlayerName() + "をカルト信者にします。";
                    break;
            }
        }
    }

}
