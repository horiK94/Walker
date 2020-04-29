using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ChestParticleController : MonoBehaviour
{
    /// <summary>
    /// パーティクル参照
    /// </summary>
    [SerializeField]
    private ParticleSystem[] particleSystems = null;

    /// <summary>
    /// パーティクルの再生
    /// </summary>
    public void PlayParticle()
    {
        for(int i = 0; i < particleSystems.Length; i++)
        {
            particleSystems[i].Play();
        }
    }
}
