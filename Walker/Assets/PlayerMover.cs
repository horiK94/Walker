using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    /// <summary>
    /// PlayerのRigidbodyコンポーネントの参照s
    /// </summary>
    [SerializeField]
    private Rigidbody rigidbody = null;

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

    Vector3 playerMoveVec = Vector3.zero;

    private void Awake()
    {
        mainCameraTranfrom = Camera.main.transform;
    }

    private void Update()
    {
        //初期化
        Vector3 cameraMoveVec = Vector3.zero;
        playerMoveVec = Vector3.zero;

        //カメラの移動
        //TODO InputManagerを使用する形へ変更予定
        cameraMoveVec = Vector3.forward * Input.GetAxis("Vertical2") + Vector3.left * Input.GetAxis("Horizontal2");
        cameraTargetTranform.rotation = Quaternion.Euler(0, mainCameraTranfrom.rotation.eulerAngles.y, 0);
        cameraMoveNext.rotation = mainCameraTranfrom.rotation;

        cameraMoveNext.RotateAround(transform.position, cameraTargetTranform.transform.right, cameraMoveVec.z * rotateSpeed * Time.deltaTime);
        cameraMoveNext.RotateAround(transform.position, cameraTargetTranform.transform.up, cameraMoveVec.x * rotateSpeed * Time.deltaTime);
        mainCameraTranfrom.RotateAround(transform.position, cameraTargetTranform.transform.up, cameraMoveVec.x * rotateSpeed * Time.deltaTime);

        if (Mathf.Abs(cameraMoveNext.eulerAngles.x) <= maxRotateX || cameraMoveNext.eulerAngles.x >= Mathf.PI * 2 * Mathf.Rad2Deg + minRotateY)
        {
            mainCameraTranfrom.RotateAround(transform.position, cameraTargetTranform.transform.right, cameraMoveVec.z * rotateSpeed * Time.deltaTime);
        }

        //プレイヤーの移動
        playerMoveVec = mainCameraTranfrom.forward * Input.GetAxis("Vertical") + mainCameraTranfrom.right * Input.GetAxis("Horizontal");
    }

    private void FixedUpdate()
    {
        rigidbody.MovePosition(transform.position + playerMoveVec * Time.deltaTime * moveAmount);
        mainCameraTranfrom.position += new Vector3(playerMoveVec.x, 0, playerMoveVec.z) * Time.deltaTime * moveAmount;
    }
}
