using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    [SerializeField]
    private Animator animator = null;

    /// <summary>
    /// 移動とみなすしきい値
    /// </summary>
    [SerializeField]
    private float moveThreshold = 0.1f;
    /// <summary>
    /// キャラ移動量
    /// </summary>
    [SerializeField]
    private float moveAmount = 10f;

    /// <summary>
    /// 最大X回転角度(～90°～ 90°)
    /// </summary>
    [SerializeField]
    private float maxRotateY = 45.0f;
    /// <summary>
    /// 最小X回転角度(～90°～ 90°)
    /// </summary>
    [SerializeField]
    private float minRotateY = -20.0f;

    /// <summary>
    /// 回転速度
    /// </summary>
    [SerializeField]
    private float rotateSpeed = 80f;

    /// <summary>
    /// メインカメラトランスフォーム(キャッシュ)
    /// </summary>
    private Transform mainCameraTranfrom = null;

    /// <summary>
    /// 中心からの距離
    /// </summary>
    [SerializeField]
    private float cameraDistance = 5;

    /// <summary>
    /// 回転量Y
    /// </summary>
    private float rotateY = 0;
    /// <summary>
    /// 回転量Z
    /// </summary>
    private float rotateZ = 0;

    /// <summary>
    /// 移動方向のキャッシュ
    /// </summary>
    private Vector3 playerMoveVec = Vector3.zero;

    /// <summary>
    /// 入力量キャッシュ
    /// </summary>
    private float inputMagnitude = 0f;

    /// <summary>
    /// 物理コンポーネントのキャッシュ
    /// </summary>
    private Rigidbody rigid = null;

    /// <summary>
    /// 移動しているか
    /// </summary>
    public bool IsMove { get; private set; } = false;


    /// <summary>
    /// 初期化
    /// </summary>
    private void Awake()
    {
        mainCameraTranfrom = Camera.main.transform;
        mainCameraTranfrom.Rotate(new Vector3(0, (maxRotateY + minRotateY) / 2, 0));

        animator.SetBool("isStop", false);
        rigid = GetComponent<Rigidbody>();

        transform.LookAt(mainCameraTranfrom);
    }

    /// <summary>
    /// 更新
    /// </summary>
    private void Update()
    {
        //初期化
        Vector3 cameraMoveVec = Vector3.zero;
        playerMoveVec = Vector3.zero;

        rotateZ += Input.GetAxis("Horizontal2") * Time.deltaTime * rotateSpeed;
        rotateY += Input.GetAxis("Vertical2") * Time.deltaTime * rotateSpeed;
        rotateY = Mathf.Clamp(rotateY, minRotateY * Mathf.Deg2Rad, maxRotateY * Mathf.Deg2Rad);
        mainCameraTranfrom.position = new Vector3(-cameraDistance * Mathf.Cos(rotateZ) * Mathf.Cos(rotateY), cameraDistance * Mathf.Sin(rotateY), -cameraDistance * Mathf.Cos(rotateY) * Mathf.Sin(rotateZ)) + transform.position;
        mainCameraTranfrom.LookAt(transform);

        //プレイヤーの移動
        Vector3 inputVec = mainCameraTranfrom.forward * Input.GetAxis("Vertical") + mainCameraTranfrom.right * Input.GetAxis("Horizontal");
        inputMagnitude = inputVec.magnitude;
        playerMoveVec = inputVec.normalized;

        if (inputMagnitude > 0.1f)
        {
            transform.localRotation = Quaternion.LookRotation(new Vector3(playerMoveVec.x, 0, playerMoveVec.z));
        }
    }

    /// <summary>
    /// 同時間フレーム更新
    /// </summary>
    private void FixedUpdate()
    {
        IsMove = inputMagnitude > moveThreshold;
        animator.SetBool("isStop", !IsMove);

        if (IsMove)
        {
            rigid.MovePosition(transform.position + playerMoveVec * Time.deltaTime * moveAmount);
            mainCameraTranfrom.position += new Vector3(playerMoveVec.x, 0, playerMoveVec.z) * Time.deltaTime * moveAmount;
        }
        else
        {
            rigid.angularVelocity = Vector3.zero;
        }
    }

    /// <summary>
    /// コリジョン設定
    /// TODO: 削除予定
    /// </summary>
    /// <param name="_collision"></param>
    private void OnCollisionEnter(Collision _collision)
    {
        //床に触れたらY座標を固定する
        if(rigid.constraints != (rigid.constraints | RigidbodyConstraints.FreezePositionY))
        {
            rigid.constraints |= RigidbodyConstraints.FreezePositionY;
        }
    }
}
