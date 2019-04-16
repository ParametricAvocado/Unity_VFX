using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTrigger : MonoBehaviour
{
    ParticleSystem ps;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if(ps && Input.GetKeyDown(KeyCode.Space))
        {
            if (ps.isPlaying)
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
            SendMessage("SpawnDecal", SendMessageOptions.DontRequireReceiver);
            ps.Play(true);
        }
    }
}
