using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class IntroController : MonoBehaviour
{
    public Image fadingImage;

    private bool fadingIn;
    private bool fadingOut;
    private float fadingDuration;
    private float valuewhenStartingToFade;

    private float timer;

    public float fadeInOnStart = 2;

    public IntroEvents OnFadingInFinished;
    public IntroEvents OnFadingOutFinished;

    // Start is called before the first frame update
    void Start()
    {
        if (!fadingImage)
            enabled = false;

        if (fadeInOnStart > 0)
            FadeIn(fadeInOnStart);
    }

    // Update is called once per frame
    void Update()
    {
        if (fadingIn)
        {
            if(Time.time - timer < fadingDuration)
                fadingImage.color = Color.black * (fadingDuration - (Time.time - timer)) / fadingDuration * valuewhenStartingToFade;
            else
            {
                fadingImage.color = Color.black * 0;
                fadingIn = false;
                OnFadingInFinished.Invoke();
            }
        }
        else if (fadingOut)
        {
            if (Time.time - timer < fadingDuration)
                fadingImage.color = Color.black * (1 - (fadingDuration - (Time.time - timer)) / fadingDuration * (1 - valuewhenStartingToFade));
            else
            {
                fadingImage.color = Color.black * 1;
                fadingOut = false;
                OnFadingOutFinished.Invoke();
            }
        }
    }

    public void FadeIn(float seconds)
    {
        fadingIn = true;
        fadingDuration = seconds;
        valuewhenStartingToFade = fadingImage.color.a;
        timer = Time.time;
    }

    public void FadeOut(float seconds)
    {
        fadingOut = true;
        fadingDuration = seconds;
        valuewhenStartingToFade = fadingImage.color.a;
        timer = Time.time;
    }
}

[Serializable]
public class IntroEvents : UnityEvent
{

}
