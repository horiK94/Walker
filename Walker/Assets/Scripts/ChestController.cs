using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ChestController : MonoBehaviour
{
    /// <summary>
    /// パーティクルコントローラー
    /// </summary>
    [SerializeField]
    private ChestParticleController particleController = null;

    /// <summary>
    /// コリジョンマネージャー
    /// </summary>
    [SerializeField]
    private CollisionManager collisionManager = null;

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="_onCollsionPlayer"></param>
    public void Init(Action _onCollsionPlayer)
    {
        collisionManager.AddOnCollisionEnter((_collision) =>
        {
            //プレイヤーか判別
            if (_collision.gameObject.GetComponent<PlayerManager>() != null)
            {
                particleController.PlayParticle();
                _onCollsionPlayer();
            }
        });
    }
}
