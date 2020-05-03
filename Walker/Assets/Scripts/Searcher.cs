using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Searcher : MonoBehaviour
{
    [SerializeField]
    private PlayerSearcher playerSearcher = null;

    [SerializeField]
    private SearcherMover searcherMover = null; 

    [SerializeField]
    private float waitTime = 4f;

    /// <summary>
    /// 目標となるプレイヤーインスタンス
    /// </summary>
    private Transform destinationPlayerTransform = null;

    private float remainTime = 0;

    private State currentState = State.SEARCH;

    enum State
    {
        SEARCH = 0,
        DISCOVERY = 1,
    }

    /// <summary>
    /// プレイヤーの設定
    /// </summary>
    /// <param name="_playerTrans"></param>
    public void SetPlayerInstance(Transform _playerTrans)
    {
        destinationPlayerTransform = _playerTrans;
        playerSearcher.SetPlayerInstance(_playerTrans);
    }

    private void Update()
    {
        remainTime -= Time.deltaTime;
        if (remainTime < 0)
        {
            search();
            if (currentState == State.SEARCH)
            {
                remainTime += waitTime;
                return;
            }
        }

        if (currentState == State.DISCOVERY)
        {
            discovery();
        }
    }

    private void search()
    {
        RaycastHit hit;
        if (playerSearcher.SearchPlayer(out hit))
        {
            //発見
            switch (currentState)
            {
                case State.SEARCH:
                    currentState = State.DISCOVERY;
                    Debug.Log("<color=yellow>発見</color>");
                    break;
            }
        }
        else
        {
            if (currentState == State.DISCOVERY)
            {
                currentState = State.SEARCH;
            }
        }
    }

    private void discovery()
    {
        //発見中はキャラに向かって走る
        searcherMover.MoveToward(destinationPlayerTransform.transform);
    }
}
