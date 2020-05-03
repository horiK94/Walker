using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearcherMover : MonoBehaviour
{
    [SerializeField]
    private Animator animator = null;

    [SerializeField]
    private float moveSpeed = 4.0f;

    [SerializeField]
    private Rigidbody rigid = null;

    /// <summary>
    /// サーチ中止
    /// </summary>
    public void SearchStop()
    {
        animator.SetBool("isStop", true);
        rigid.angularVelocity = Vector3.zero;
    }

    /// <summary>
    /// 引数の位置に向かって歩く
    /// </summary>
    public void MoveToward(Transform _destinationTransform)
    {
        if(animator.GetBool("isStop"))
        {
            animator.SetBool("isStop", false);
        }

        Vector3 destinationDirection = _destinationTransform.transform.position - transform.position;
        Vector3 movePoint = transform.position;
        movePoint += destinationDirection.normalized * Time.deltaTime * moveSpeed;

        rigid.MovePosition(movePoint);
        transform.LookAt(_destinationTransform, transform.up);
    }
}
