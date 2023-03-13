using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainParticlesHandler : MonoBehaviour
{
    public List<ParticleSystem> _rainParticles = new List<ParticleSystem>();
    void OnEnable()
    {
        var parent = GetComponent<ParticleSystem>();

        _rainParticles.Add(parent);

        foreach (Transform t in transform)
        {
            if (t.GetComponent<ParticleSystem>() != null)
            {
                _rainParticles.Add(t.GetComponent<ParticleSystem>());
            }
        }
    }

    public void StartParticles()
    {
        foreach (ParticleSystem p in _rainParticles)
        {
            p.Play();
        }
    }

    public void StopParticles()
    {
        foreach (ParticleSystem p in _rainParticles)
        {
            p.Stop();
        }
    }
}
