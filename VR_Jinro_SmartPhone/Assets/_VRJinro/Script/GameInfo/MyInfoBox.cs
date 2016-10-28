using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class MyInfoBox : MonoBehaviour {

    [SerializeField]
    Text dayNumberText;
    [SerializeField]
    Text phaseText;
    [SerializeField]
    Text myNameText;
    [SerializeField]
    Text aliveCount;
    [SerializeField]
    Text rollListText;
    [SerializeField]
    Image skipImage;
    [SerializeField]
    Text suspectedCountText;

    //※このスクリプトは個人のしか更新しないため別のプレイヤーからアクセスしても意味がない

    /// <summary>
    /// 現在の進行日をセットする
    /// </summary>
    /// <param name="day">何日目か</param>
    public void SetDayNumber(int day)
    {

        dayNumberText.text = day + "日目";
    }

    /// <summary>
    /// 現在のペースをもとになに時間かをセットする
    /// </summary>
    /// <param name="phase"></param>
    public void SetPhase(DataBase.Phase phase)
    {
        DataBase tmpDataBase = new DataBase();
        phaseText.text = tmpDataBase.PhaseName(phase);
    }

    /// <summary>
    /// 名前をセットする
    /// </summary>
    /// <param name="name"></param>
    public void SetName(string name)
    {
        myNameText.text = name;
    }

    /// <summary>
    /// カルト信者と、名前に追加する（インフォボックスのみ)
    /// </summary>
    public void AddCultInName()
    {
        myNameText.text += "(カルト信者)";
    }

    /// <summary>
    /// 生存人数をセットする
    /// </summary>
    /// <param name="aliveMember"></param>
    public void SetAliveCount(int aliveMember)
    {
        aliveCount.text = aliveMember + "人";
    }

    /// <summary>
    /// 全役職をセットする
    /// </summary>
    /// <param name="_rollList"></param>
    public void SetRollList(string rollList)
    {
        rollListText.text = rollList;
    }

    /// <summary>
    /// 疑われている人数をセットする
    /// </summary>
    /// <param name="_suspectedCount"></param>
    public void SetSuspectedCount(int _suspectedCount)
    {
        suspectedCountText.text = _suspectedCount.ToString() + "人";
    }

    /// <summary>
    /// スキップイメージの表示の有無
    /// </summary>
    /// <param name="_isShow"></param>
    public void ShowSkipImage(bool _isShow)
    {
        skipImage.gameObject.SetActive(_isShow);
    }
}
