using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ACTION_GaugeControl : ClickObjectAction {

    [SerializeField]
    private Image gaugeImage;

    void Awake()
    {
        gaugeImage.fillAmount = 0;
    }

    /// <summary>
    /// ゲージの操作を行う
    /// </summary>
    /// <param name="percent"></param>
    public override void ActionFromPercent(float percent)
    {
        gaugeImage.fillAmount = percent;
    }


}
