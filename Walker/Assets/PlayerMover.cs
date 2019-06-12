using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    [SerializeField]
    private Animator animator = null;

    /// <summary>
    /// キャラ移動量
    /// </summary>
    [SerializeField]
    private float moveAmount = 10f;
    /// <summary>
    /// 回転量
    /// </summary>
    [SerializeField]
    private float roteateSpeed = 3f;

    /// <summary>
    /// カメラターゲットトランスフォーム
    /// </summary>
    [SerializeField]
    private Transform cameraTargetTranform = null;

    /// <summary>
    /// カメラ移動予定オブジェクト
    /// </summary>
    [SerializeField]
    private Transform cameraMoveNext = null;

    /// <summary>
    /// 最大X回転角度(～90°～ 90°)
    /// </summary>
    [SerializeField]
    private float maxRotateX = 45.0f;
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
    /// 回転角度
    /// </summary>
    float rotateAngle = 0;

    [SerializeField]
    private float cameraDistance = 5;
    float rotateY = 0;
    float rotateZ = 0;

    Vector3 playerMoveVec = Vector3.zero;

    private void Awake()
    {
        mainCameraTranfrom = Camera.main.transform;
        animator.SetBool("isStop", false);
    }

    private void Update()
    {
        //初期化
        Vector3 cameraMoveVec = Vector3.zero;
        playerMoveVec = Vector3.zero;

        rotateZ += Input.GetAxis("Horizontal2") * Time.deltaTime * rotateSpeed;
        rotateY += Input.GetAxis("Vertical2") * Time.deltaTime * rotateSpeed;
        mainCameraTranfrom.position = new Vector3(cameraDistance * Mathf.Cos(rotateZ) * Mathf.Cos(rotateY), -cameraDistance * Mathf.Sin(rotateY), cameraDistance * Mathf.Sin(rotateZ)) + transform.position;
        mainCameraTranfrom.LookAt(transform);

        //プレイヤーの移動
        playerMoveVec = mainCameraTranfrom.forward * Input.GetAxis("Vertical") + mainCameraTranfrom.right * Input.GetAxis("Horizontal");
        if (playerMoveVec.magnitude > 0.1f)
        {
            transform.localRotation = Quaternion.LookRotation(new Vector3(playerMoveVec.x, 0, playerMoveVec.z));
        }
    }

    private void FixedUpdate()
    {
        GetComponent<Rigidbody>().MovePosition(transform.position + playerMoveVec * Time.deltaTime * moveAmount);
        mainCameraTranfrom.position += new Vector3(playerMoveVec.x, 0, playerMoveVec.z) * Time.deltaTime * moveAmount;
        if (playerMoveVec.magnitude > 0.1f)
        {
            animator.SetBool("isStop", false);
        }
        else
        {
            animator.SetBool("isStop", true);
        }
    }
}
