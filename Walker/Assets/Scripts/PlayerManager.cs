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
    /// 移動したことがあるか
    /// </summary>
    private bool wasMoved = false;

    /// <summary>
    /// .初回移動時に呼ぶアクション
    /// </summary>
    private Action firstMoveAction = null;

    /// <summary>
    /// 初回移動時のアクション設定
    /// </summary>
    /// <param name="_firstMoveAction"></param>
    public void SetFirstMoveAction(Action _firstMoveAction)
    {
        firstMoveAction = _firstMoveAction;
    }

    /// <summary>
    /// 行動停止
    /// </summary>
    public void Pause()
    {
        playerMover.Pause();
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
