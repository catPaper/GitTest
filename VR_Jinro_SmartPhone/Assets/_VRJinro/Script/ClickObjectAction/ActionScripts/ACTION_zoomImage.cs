using UnityEngine;
using System.Collections;

public class ACTION_zoomImage : ClickObjectAction {

    private static int MaxZoomValue= 2;
    private Vector3 defaultScale;

    [SerializeField]
    private Canvas zoomCanvas;

    void Awake()
    {
        defaultScale = zoomCanvas.gameObject.transform.localScale;
    }

    /// <summary>
    /// イメージのズームを行う
    /// </summary>
    /// <param name="percent"></param>
    public override void ActionFromPercent(float percent)
    {
        if(percent == 0)
        {
            zoomCanvas.sortingOrder = 0;
        }else
        {
            zoomCanvas.sortingOrder = 1;
        }
        zoomCanvas.gameObject.transform.localScale = defaultScale * ((MaxZoomValue - 1) * percent + 1);
    }

}
