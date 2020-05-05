using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Searcher : MonoBehaviour
{
    /// <summary>
    /// 探索者チェックコンポーネント
    /// </summary>
    [SerializeField]
    private SearchChecker searcherChecker = null;

    /// <summary>
    /// 移動コンポーネント
    /// </summary>
    [SerializeField]
    private SearcherMover searcherMover = null;

    /// <summary>
    /// 探索待ち時間
    /// </summary>
    private float waitTime = 4f;

    /// <summary>
    /// 目標となるプレイヤーインスタンス
    /// </summary>
    private Transform destinationPlayerTransform = null;

    /// <summary>
    /// 探索待ち残り時間
    /// </summary>
    private float remainTime = 0;

    private State currentState = State.SEARCH;

    enum State
    {
        SEARCH = 0,
        CHASE = 1,
        STOP = 2
    }

    /// <summary>
    /// プレイヤーの設定
    /// </summary>
    /// <param name="_playerTrans"></param>
    public void SetPlayerTransform(Transform _playerTrans)
    {
        destinationPlayerTransform = _playerTrans;
        searcherChecker.SetPlayerTransform(_playerTrans);
    }

    /// <summary>
    /// 動作停止
    /// </summary>
    public void Pause()
    {
        currentState = State.STOP;
        searcherMover.SearchStop();
    }

    private void Update()
    {
        if (currentState == State.STOP)
        {
            return;
        }

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

        if (currentState == State.CHASE)
        {
            chase();
        }
    }

    /// <summary>
    /// 探索中
    /// </summary>
    private void search()
    {
        RaycastHit hit;
        if (searcherChecker.SearchPlayer(out hit))
        {
            //発見
            switch (currentState)
            {
                case State.SEARCH:
                    discovery();
                    currentState = State.CHASE;
                    break;
            }
        }
        else
        {
            if (currentState == State.CHASE)
            {
                currentState = State.SEARCH;
                searcherMover.SearchStop();
            }
        }
    }

    /// <summary>
    /// 発見時
    /// </summary>
    private void discovery()
    {
        AudioManager audioManager = AudioManager.Instance;
        audioManager.PlayOneShotSE(AudioManager.eSEAudioClip.DISCOVERY);
        audioManager.SetSESpatialBlend(false);
    }

    /// <summary>
    /// 発見中
    /// </summary>
    private void chase()
    {
        //発見中はキャラに向かって走る
        searcherMover.MoveToward(destinationPlayerTransform.transform);
    }
}
