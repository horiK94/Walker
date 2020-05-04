using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PursuerMover : MonoBehaviour
{
    /// <summary>
    /// 軌跡情報
    /// </summary>
    public class TracingData
    {
        /// <summary>
        /// 記録時間
        /// </summary>
        public float Time { get; } = 0f;

        /// <summary>
        /// 位置
        /// </summary>
        public Vector3 Position { get; } = Vector3.zero;

        /// <summary>
        /// 回転
        /// </summary>
        public Quaternion Rotate { get; } = Quaternion.identity;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="_time"></param>
        /// <param name="_position"></param>
        /// <param name="_rotate"></param>
        public TracingData(float _time, Vector3 _position, Quaternion _rotate)
        {
            Time = _time;
            Position = _position;
            Rotate = _rotate;
        }
    }

    /// <summary>
    /// 軌跡情報キュー
    /// </summary>
    Queue<TracingData> tracingQueue = new Queue<TracingData>();

    /// <summary>
    /// rigidbodyコンポーネント
    /// </summary>
    [SerializeField]
    private Rigidbody rigid = null;

    /// <summary>
    /// アニメーション
    /// </summary>
    [SerializeField]
    private Animator animator = null;

    /// <summary>
    /// キャラ表示ルートオブジェクト
    /// </summary>
    [SerializeField]
    private GameObject charaLooksRoot = null;

    /// <summary>
    /// 移動遅延時間
    /// </summary>
    private float moveDelayTime = 1f;

    /// <summary>
    /// 時間
    /// </summary>
    private float time = 0;

    /// <summary>
    /// 移動を行うか
    /// </summary>
    private bool isMove = false;

    /// <summary>
    /// 目標Transformキャッシュ
    /// </summary>
    Transform targetTransform = null;

    /// <summary>
    /// 前フレーム情報
    /// </summary>
    TracingData beforeFrame = null;

    /// <summary>
    /// 後フレーム情報
    /// </summary>
    TracingData afterFrame = null;

    /// <summary>
    /// 移動の記録開始
    /// </summary>
    public void StartRecord(Transform _targetTransform, float _moveDelay)
    {
        targetTransform = _targetTransform;
        moveDelayTime = _moveDelay;

        isMove = true;
        tracingQueue.Enqueue(new TracingData(moveDelayTime, targetTransform.position, targetTransform.rotation));

        charaLooksRoot.SetActive(false);
    }

    /// <summary>
    /// 行動停止
    /// </summary>
    public void Pause()
    {
        isMove = false;
        animator.SetBool("isStop", true);
        rigid.angularVelocity = Vector3.zero;
    }

    /// <summary>
    /// 更新
    /// </summary>
    private void Update()
    {
        if (!isMove)
        {
            return;
        }

        time += Time.deltaTime;
        tracingQueue.Enqueue(new TracingData(moveDelayTime + time, targetTransform.position, targetTransform.rotation));

        if (time < moveDelayTime || tracingQueue.Count <= 1)
        {
            //移動しない
            return;
        }

        if (!charaLooksRoot.activeSelf)
        {
            charaLooksRoot.SetActive(true);
        }

        if (animator.GetBool("isStop"))
        {
            animator.SetBool("isStop", false);
        }

        if (beforeFrame == null)
        {
            beforeFrame = tracingQueue.Dequeue();
            afterFrame = tracingQueue.Dequeue();
        }

        while (afterFrame.Time < time)
        {
            beforeFrame = afterFrame;
            afterFrame = tracingQueue.Dequeue();
        }

        rigid.MovePosition(Vector3.Lerp(beforeFrame.Position, afterFrame.Position, (time - beforeFrame.Time) / (afterFrame.Time - beforeFrame.Time)));
        rigid.MoveRotation(Quaternion.Lerp(beforeFrame.Rotate, afterFrame.Rotate, (time - beforeFrame.Time) / (afterFrame.Time - beforeFrame.Time)));
    }
}
