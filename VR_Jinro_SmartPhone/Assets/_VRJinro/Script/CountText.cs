using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CountText : MonoBehaviour {
    [SerializeField]
    private Text countText;

    private int count;

    void Awake()
    {
        count = 0;
        countText.text = count.ToString();
    }
	
    public void AddCount()
    {
        count++;
        countText.text = count.ToString();
    }

    public void DecCount()
    {
        count--;
        count = Mathf.Max(0, count);
        countText.text = count.ToString();
    }
}
