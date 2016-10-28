using UnityEngine;
using System.Collections;

public class Fade : MonoBehaviour {

    [SerializeField]
    private GameObject fadeImage;

    private Material fadeMaterial;

    private float fadeSpeed = 1.5f;

    public enum FadeMode
    {
        DEFAULT,
        FADEIN,
        FADEOUT
    }

    public FadeMode fadeMode;

    void Awake()
    {
        fadeMaterial = fadeImage.GetComponent<Renderer>().material;
        fadeImage.SetActive(false);
    }

    public void FadeStart()
    {
        switch (fadeMode)
        {
            case FadeMode.FADEOUT:
                fadeMaterial.color = new Color(0, 0, 0, 0);
                break;
            case FadeMode.FADEIN:
                fadeMaterial.color = new Color(0, 0, 0, 1);
                break;
            default:
                break;
        }
        fadeImage.SetActive(true);
    }

    public void FadeEnd()
    {
        fadeImage.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
	    switch(fadeMode)
        {
            case FadeMode.FADEOUT:
                if(fadeMaterial.color.a <= 1)
                    fadeMaterial.color += new Color(0, 0, 0, Time.deltaTime / fadeSpeed);
                break;
            case FadeMode.FADEIN:
                if (fadeMaterial.color.a >= 0)
                    fadeMaterial.color -= new Color(0, 0, 0, Time.deltaTime / fadeSpeed);
                break;
            default:
                break;
        }
	}
}
