using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour {

	[SerializeField]
    private Text minuteText;
    [SerializeField]
    private Text secondText;


    /// <summary>
    /// 残り時間をもとに分、秒でテキストを更新する
    /// </summary>
    /// <param name="restCount">残り時間</param>
    public void UpdateTimerText(int restCount)
    {
        minuteText.text = (restCount / 60).ToString();
        secondText.text = (restCount % 60).ToString();
    }
}
