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
    /// 角度変更待ち時間
    /// </summary>
    private float rotateWaitTime = 1f;

    /// <summary>
    /// 目標となるプレイヤーインスタンス
    /// </summary>
    private Transform destinationPlayerTransform = null;

    /// <summary>
    /// 角度変更待ち時間
    /// </summary>
    private float waitTime = 0;

    private State currentState = State.STOP;

    private float LAP_ANGLE = 360.0f;

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

    /// <summary>
    /// 動作開始
    /// </summary>
    public void Play()
    {
        currentState = State.SEARCH;
    }

    private void Update()
    {
        if (currentState == State.STOP)
        {
            return;
        }

        search();
        if (currentState == State.SEARCH)
        {
            waitTime -= Time.deltaTime;
            if (waitTime < 0)
            {
                //ランダムで角度を変える
                searcherMover.SetRotate(Random.Range(0, LAP_ANGLE));
                waitTime += rotateWaitTime;
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
        audioManager.SetSESpatialBlend(false);
        audioManager.PlayOneShotSE(AudioManager.eSEAudioClip.DISCOVERY);
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
