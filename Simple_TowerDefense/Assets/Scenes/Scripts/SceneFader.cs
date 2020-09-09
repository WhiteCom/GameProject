﻿using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;


public class SceneFader : MonoBehaviour
{
    public Image img;
    public AnimationCurve curve;

    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    public void FadeTo(string scene)
    {
        StartCoroutine(FadeOut(scene));
    }

    IEnumerator FadeIn()
    {
        float t = 1f;

        while(t > 0f)
        {
            //Decresing Time value for fading animation
            t -= Time.deltaTime;
            float a = curve.Evaluate(t);
            img.color = new Color (0f, 0f, 0f, a); //control alpha channel
            yield return 0;
        }
    }

    IEnumerator FadeOut(string scene)
    {
        float t = 0f;

        while (t < 1f)
        {
            //Incresing Time value for fading animation
            t += Time.deltaTime;
            float a = curve.Evaluate(t);
            img.color = new Color(0f, 0f, 0f, a); //control alpha channel
            yield return 0;
        }

        SceneManager.LoadScene(scene);
    }
}
