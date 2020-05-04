using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurserMover : MonoBehaviour
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
    /// 移動遅延時間
    /// </summary>
    [SerializeField]
    private float moveDelayTime = 1f;

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
    /// 移動するか設定
    /// </summary>
    public void StartMove(Transform _targetTransform)
    {
        targetTransform = _targetTransform;

        isMove = true;
        tracingQueue.Enqueue(new TracingData(moveDelayTime, targetTransform.position, targetTransform.rotation));
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
