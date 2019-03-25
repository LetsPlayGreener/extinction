using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerEffectHandler : MonoBehaviour
{
    private ParticleSystem particles;

    private ParticleSystem.MainModule main;

    // Start is called before the first frame update
    void Start()
    {
        particles = GetComponent<ParticleSystem>();
        if (!particles)
            enabled = false;
    }

    public void PlayCorrectEffect()
    {
        main = particles.main;
        main.startColor = Color.green;
        particles.Play();
    }

    public void PlayWrongEffect()
    {
        main = particles.main;
        main.startColor = Color.red;
        particles.Play();
    }
}
