using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class HostGameSettingCanvas : MonoBehaviour {

    [SerializeField]
    private Text playerCountText;
    [SerializeField]
    private Text noonTimeText;
    [SerializeField]
    private Text nightTimeText;
    [SerializeField]
    private Button CreateButton;
    [SerializeField]
    private GameObject hostOnly_GameInfoPrefab;

    private int noonTime = 300;
    private int nightTime = 120;

    private DataBase tmpDatabase = new DataBase();

    [Header("Read Only")]
    [SerializeField]
    private List<DataBase.Roll> rollList;

    void Awake()
    {
        CreateButton.interactable = false;
        noonTimeText.text = noonTime.ToString();
        nightTimeText.text = nightTime.ToString();
    }

    void Update()
    {
        CreateButton.interactable = CheckCreateRoom();
        UpdatePlayerCountText();
    }

    /// <summary>
    /// 昼時間を加減するボタン
    /// </summary>
    /// <param name="isAdd"></param>
    public void AddNoonTimeButton(bool isAdd)
    {
        noonTime += 30 * (isAdd ? 1 : -1);
        noonTime = Mathf.Max(30, noonTime);
        noonTimeText.text = noonTime.ToString();
    }

    /// <summary>
    /// 夜時間を加減するボタン
    /// </summary>
    /// <param name="isAdd"></param>
    public void AddNightTimeButton(bool isAdd)
    {
        nightTime += 30 * (isAdd ? 1 : -1);
        nightTime = Mathf.Max(30, nightTime);
        nightTimeText.text = nightTime.ToString();
    }

    /// <summary>
    /// プレイヤーカウントテキストの更新
    /// </summary>
    private void UpdatePlayerCountText()
    {
        //設定人数外の場合
        if(rollList.Count < 1 || rollList.Count > 12)
        {
            playerCountText.text = "作成できる人数ではありません。";
            return;
        }

        playerCountText.text = rollList.Count.ToString() + "人でプレイします";
    }

    /// <summary>
    /// 設定が正しければ、チェックボタンを表示
    /// </summary>
    /// <returns></returns>
    private bool CheckCreateRoom()
    {
        //現在最大プレイヤー数は12人まで
        if (rollList.Count < 1 || rollList.Count > 12)
        {
            return false;
        }

        //人狼が一人もいないルームは作成不能
        if(SearchRollCount(DataBase.Roll.WEREWOLF) < 1)
        {
            return false;
        }

        //人狼が全体数と同じ or それ以上ならゲーム終了してしまうため作成不能
        if(SearchRollCount(DataBase.Roll.WEREWOLF)*2 >= rollList.Count)
        {
            return false;
        }

        return true;

    }

    /// <summary>
    /// 特定の役職の登録数を返す
    /// </summary>
    /// <param name="_searchRoll"></param>
    /// <returns></returns>
    private int SearchRollCount(DataBase.Roll _searchRoll)
    {
        int rollCount = 0;
        foreach(DataBase.Roll _roll in rollList)
        {
            if (_roll == _searchRoll)
                rollCount++;
        }

        return rollCount;
    }


    /// <summary>
    /// 役職を追加
    /// </summary>
    /// <param name="_roll"></param>
    public void AddRoll(string rollName)
    {
        DataBase.Roll _roll = tmpDatabase.SearchRollName(rollName);
        Debug.Log(_roll);
        if(_roll != DataBase.Roll.UNKNOWN)
            rollList.Add(_roll);
    }
    
    /// <summary>
    /// 役職を減らす
    /// </summary>
    /// <param name="_roll"></param>
    public void DecRoll(string rollName)
    {
        DataBase.Roll _roll = tmpDatabase.SearchRollName(rollName);
        if (_roll == DataBase.Roll.UNKNOWN)
            return;

        if (SearchRollCount(_roll) > 0)
        {
            for(int i= 0;i<rollList.Count;i++)
            {
                if(rollList[i] == _roll)
                {
                    rollList.RemoveAt(i);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 作成ボタンを押下
    /// </summary>
    public void CreateProcess()
    {
        rollList.Sort();
        GameObject hostonly_GameInfo = GameObject.Instantiate(hostOnly_GameInfoPrefab);
        hostonly_GameInfo.GetComponent<HOSTONLY_GameInfo>().AssignRoll(rollList);
        hostonly_GameInfo.GetComponent<HOSTONLY_GameInfo>().InitGameTime(noonTime, nightTime);
        hostonly_GameInfo.GetComponent<HOSTONLY_GameInfo>().CreateRoom();
        GameObject.FindGameObjectWithTag("Manager").GetComponent<SettingManager>().SetHostOnly_GameInfo(hostonly_GameInfo.GetComponent<HOSTONLY_GameInfo>());
    }
    
}
