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
        if (onCollisionEnterAction != null)
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
        if (onCollisionExitAction != null)
        {
            onCollisionExitAction(collision);
        }
    }

    /// <summary>
    /// 衝突時のコールバック設定
    /// </summary>
    /// <param name="_setAction"></param>
    public void AddOnCollisionEnter(Action<Collision> _setAction)
    {
        onCollisionEnterAction += _setAction;
    }

    /// <summary>
    /// 衝突時のコールバッククリア
    /// </summary>
    public void ClearOnCollisionEnterAction()
    {
        onCollisionEnterAction = null;
    }

    /// <summary>
    /// 衝突中のコールバック設定
    /// </summary>
    /// <param name="_setAction"></param>
    public void AddOnCollisionStay(Action<Collision> _setAction)
    {
        onCollisionStayAction += _setAction;
    }

    /// <summary>
    /// 衝突中のコールバッククリア
    /// </summary>
    public void ClearOnCollisionStayAction()
    {
        onCollisionStayAction = null;
    }

    /// <summary>
    /// 衝突後のコールバック設定
    /// </summary>
    /// <param name="_setAction"></param>
    public void AddOnCollsitionExit(Action<Collision> _setAction)
    {
        onCollisionExitAction += _setAction;
    }

    public void ClearOnCollisionExit()
    {
        onCollisionExitAction = null;
    }
}
