using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSearcher : MonoBehaviour
{
    /// <summary>
    /// プレイヤーインスタンス
    /// </summary>
    private Transform playerTrans = null;

    /// <summary>
    /// 探索距離
    /// </summary>
    [SerializeField]
    private float canSearchDistance = 10f;

    [SerializeField]
    private float canSearchAngle = 270f;

    /// <summary>
    /// プレイヤーの設定
    /// </summary>
    /// <param name="_playerTrans"></param>
    public void SetPlayerInstance(Transform _playerTrans)
    {
        playerTrans = _playerTrans;
    }

    /// <summary>
    /// プレイヤーの検索
    /// </summary>
    /// <param name="_raycastHit"></param>
    /// <returns></returns>
    public bool SearchPlayer(out RaycastHit _raycastHit)
    {
        if (playerTrans == null)
        {
            _raycastHit = default(RaycastHit);
            return false;
        }

        Vector3 playerVec = playerTrans.transform.position - transform.position;
        Debug.DrawRay(transform.position, playerVec, Color.green, 5f, false);
        if (Physics.Raycast(transform.position, playerVec, out _raycastHit, canSearchDistance))
        {
            //任意のコライダーと交わる時
            if (_raycastHit.collider.gameObject.GetComponent<PlayerMover>() == null)
            {
                return false;
            }
            else
            {
                return Vector3.Angle(transform.forward, playerVec) <= canSearchAngle / 2;
            }
        }
        return false;
    }
}
