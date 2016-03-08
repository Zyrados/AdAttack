using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Fader : MonoBehaviour {

    public float FadeSpeed = 2f;
    public static bool FadeOUT;

    private bool sceneStarting = true;
    private Image FaderIMG;

    void Awake()
    {
       
    }

    void Start()
    {
        FaderIMG = GetComponent<Image>();
        FadeIn();
    }

    private void FadeOut()
    {
        FaderIMG.CrossFadeAlpha(255f, 200f, false);
    }
        
    void LateUpdate()
    {
        if (FadeOUT)
        {
            FadeOut();
        }
    }

    private void FadeIn()
    {
        FaderIMG.CrossFadeAlpha(0f, FadeSpeed, false);
    }
}
