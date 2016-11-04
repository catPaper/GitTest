using UnityEngine;
using System.Collections;
using System;

public class PasswordGenerator {

    private DateTime dayTime;


    /// <summary>
    /// 現在の時刻のパスワードを返す
    /// </summary>
    public int ShowNowPassword()
    {
        dayTime = DateTime.Now;
        return GeneratePassWithTime(dayTime);
    }

    /// <summary>
    /// 指定された時間をもとにパスワードを表示する
    /// </summary>
    /// <param name="_thisTime"></param>
    public int ShowSelectPassword(DateTime _thisTime)
    {
        return GeneratePassWithTime(_thisTime);
    }

    /// <summary>
    /// パスワードが現在の時刻のパスワードと一致してる場合trueを返す
    /// </summary>
    /// <returns></returns>
    public bool CheckPassword(int _pass)
    {
        return _pass == ShowNowPassword();
    }

    /// <summary>
    /// 受け取った日付をもとに固有のパスワードを返す
    /// </summary>
    /// <param name="_dateTime"></param>
    /// <returns></returns>
    private int GeneratePassWithTime(DateTime _dateTime)
    {
        string passCodeStr;
        int passCode;
        int beforePass = 0;

        beforePass += (_dateTime.Hour +1) * 184;
        beforePass += _dateTime.Day * 102;
        beforePass += _dateTime.Month * 34;

        passCodeStr = Convert.ToString(beforePass, 8);

        passCode = int.Parse(passCodeStr);
        passCode += 132;
        passCode = (int)(passCode*1.37);

        return passCode;
    }
}
