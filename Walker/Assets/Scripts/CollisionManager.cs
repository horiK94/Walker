using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    /// <summary>
    /// 衝突時のコールバック
    /// </summary>
    private Action<Collision> onCollisionEnterAction = null;

    /// <summary>
    /// 衝突中のコールバック
    /// </summary>
    private Action<Collision> onCollisionStayAction = null;

    /// <summary>
    /// 衝突後のコールバック
    /// </summary>
    private Action<Collision> onCollisionExitAction = null;

    /// <summary>
    /// 衝突時
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<PlayerMover>() != null)
        {
            onCollisionEnterAction(collision);
        }
    }
    /// <summary>
    /// 衝突中
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionStay(Collision collision)
    {
        if (onCollisionStayAction != null)
        {
            onCollisionStayAction(collision);
        }
    }

    /// <summary>
    /// 衝突後
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionExit(Collision collision)
    {
        if(onCollisionExitAction != null)
        {
            onCollisionExitAction(collision);
        }
    }

    /// <summary>
    /// 衝突時のコールバック設定
    /// </summary>
    /// <param name="_setAction"></param>
    public void SetOnCollisionEnter(Action<Collision> _setAction)
    {
        onCollisionEnterAction = _setAction;
    }

    /// <summary>
    /// 衝突中のコールバック設定
    /// </summary>
    /// <param name="_setAction"></param>
    public void SetOnCollisionStay(Action<Collision> _setAction)
    {
        onCollisionStayAction = _setAction;
    }

    /// <summary>
    /// 衝突後のコールバック設定
    /// </summary>
    /// <param name="_setAction"></param>
    public void SetOnCollsitionExit(Action<Collision> _setAction)
    {
        onCollisionExitAction = _setAction;
    }
}
