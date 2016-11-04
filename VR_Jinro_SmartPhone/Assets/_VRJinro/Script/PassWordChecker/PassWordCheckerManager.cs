using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class PassWordCheckerManager : MonoBehaviour {

    [SerializeField]
    private Button checkNowPasswordButton;
    [SerializeField]
    private Text showPassWordText;

    private PasswordGenerator passGenerator;

    void Awake()
    {
        passGenerator = new PasswordGenerator();
    }

    private void Start()
    {
        //最初は現在時刻のパスワードを表示
        showCurrentPassword();

        if(checkNowPasswordButton != null)
        {
            checkNowPasswordButton.onClick.AddListener(showCurrentPassword);
        }
    }

    /// <summary>
    /// 現在時刻のパスワードを反映
    /// </summary>
    private void showCurrentPassword()
    {
        showPassWordText.text = "現在の時刻のパスワードは" + passGenerator.ShowNowPassword();
    }
	
	
}
