using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearcherMover : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 4.0f;

    [SerializeField]
    private Rigidbody rigid = null;

    /// <summary>
    /// 引数の位置に向かって歩く
    /// </summary>
    public void MoveToward(Transform _destinationTransform)
    {
        Vector3 destinationDirection = _destinationTransform.transform.position - transform.position;
        Vector3 movePoint = transform.position;
        movePoint += destinationDirection.normalized * Time.deltaTime * moveSpeed;

        rigid.MovePosition(movePoint);
        transform.LookAt(_destinationTransform, transform.up);
    }
}
