using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerManager : MonoBehaviour
{
    /// <summary>
    /// プレイヤー移動コンポーネント
    /// </summary>
    [SerializeField]
    private PlayerMover playerMover = null;

    /// <summary>
    /// コリジョンマネージャー
    /// </summary>
    [SerializeField]
    private CollisionManager collisionManager = null;

    /// <summary>
    /// 移動したことがあるか
    /// </summary>
    private bool wasMoved = false;

    /// <summary>
    /// 初回移動時に呼ぶアクション
    /// </summary>
    private Action firstMoveAction = null;

    /// <summary>
    /// 敵と触れた際のアクション
    /// </summary>
    private Action onCollisionEnemyAction = null;

    /// <summary>
    /// 初回移動時のアクション設定
    /// </summary>
    /// <param name="_firstMoveAction"></param>
    public void SetFirstMoveAction(Action _firstMoveAction)
    {
        firstMoveAction = _firstMoveAction;
    }

    /// <summary>
    /// 敵衝突時のアクション設定
    /// </summary>
    /// <param name="_onCollisionEnemyAction"></param>
    public void SetOnCollisionEnemyAction(Action _onCollisionEnemyAction)
    {
        onCollisionEnemyAction = _onCollisionEnemyAction;
    }

    /// <summary>
    /// 行動停止
    /// </summary>
    public void Pause()
    {
        playerMover.Pause();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    private void Awake()
    {
        collisionManager.AddOnCollisionEnter((_collision) =>
        {
            onCollisionEnter(_collision);
        });
    }

    /// <summary>
    /// 衝突時のアクション
    /// </summary>
    /// <param name="_collision"></param>
    private void onCollisionEnter(Collision _collision)
    {
        if (_collision.gameObject.GetComponent<Enemy>() != null && onCollisionEnemyAction != null)
        {
            onCollisionEnemyAction();
        }
    }

    /// <summary>
    /// 更新
    /// </summary>
    private void Update()
    {
        if (!wasMoved && firstMoveAction != null && playerMover.IsMove)
        {
            firstMoveAction();
            wasMoved = true;
        }
    }
}
