using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージ種類
/// </summary>
public enum eStageType
{
    FLOOR = 0,       //何もない(床)
    BLOCK = 1,      //ブロック(通過不可)
    CEILING = 2,    //天井(通過可能)
    START = 10,     //スタート位置
    GOAL = 11,      //ゴール位置
}

public class MazeCreator
{
    /// <summary>
    /// 迷路幅
    /// </summary>
    private int width = 0;
    /// <summary>
    /// 迷路高さ
    /// </summary>
    private int height = 0;
    /// <summary>
    /// ゴール位置インデックス((0, 0)～）
    /// </summary>
    private Vector2Int goalIndex = null;

    /// <summary>
    /// 天井ブロック生成確率(0 ～ 1)
    /// </summary>
    private float ceilingPrecentage = 0;

    /// <summary>
    /// 穴掘り候補座標
    /// </summary>
    private List<Vector2Int> digCandidate = null;
    /// <summary>
    /// 迷路情報
    /// </summary>
    private eStageType[,] mazeInfo = null;

    public eStageType[,] CreateMaze(int _width, int _height, Vector2Int _startIndex, Vector2Int _goalIndex, float _ceilingPercent)
    {
        width = _width;
        height = _height;
        goalIndex = _goalIndex;
        ceilingPrecentage = _ceilingPercent;

        //迷路に必要な要素が満たされているかチェック
        if (_width < 5 || _height < 5)
        {
            throw new ArgumentOutOfRangeException("_width, _height", "5より大きいサイズを指定して下さい");
        }
        if (_width % 2 == 0 || _height % 2 == 0)
        {
            throw new ArgumentOutOfRangeException("_width, _height", "奇数の値を指定して下さい");
        }
        if (_startIndex.x % 2 == 1 || _startIndex.y % 2 == 1)
        {
            throw new ArgumentOutOfRangeException("_startIndex", "偶数の値を指定して下さい");
        }
        if (_goalIndex.x % 2 == 1 || _goalIndex.y % 2 == 1)
        {
            throw new ArgumentOutOfRangeException("_goalIndex", "偶数の値を指定して下さい");
        }

        mazeInfo = new eStageType[_width, _height];
        digCandidate = new List<Vector2Int>();

        //すべてブロックで埋める
        for (int i = 0; i < _width; i++)
        {
            for (int k = 0; k < _height; k++)
            {
                mazeInfo[i, k] = eStageType.BLOCK;
            }
        }

        setStageType(_goalIndex, eStageType.GOAL);

        //穴掘り開始
        dig(_goalIndex);

        //スタート位置を設定
        setStageType(_startIndex, eStageType.START);

        createCeiling();
        return mazeInfo;
    }

    /// <summary>
    /// 1回に掘る距離
    /// </summary>
    private const int ONCE_DIG_MAX = 2;

    /// <summary>
    /// 引数の箇所から掘っていく
    /// </summary>
    /// <param name="_position"></param>
    private void dig(Vector2Int _position)
    {
        Vector2Int currentPos = _position;
        while (true)
        {
            //掘れる方向リストの作成
            List<Vector2Int> canDigDirectionList = new List<Vector2Int>();
            //上
            if (isDig(currentPos + Vector2Int.Up) && isDig(currentPos + Vector2Int.Up * ONCE_DIG_MAX))
            {
                canDigDirectionList.Add(Vector2Int.Up);
            }
            //下
            if (isDig(currentPos + Vector2Int.Down) && isDig(currentPos + Vector2Int.Down * ONCE_DIG_MAX))
            {
                canDigDirectionList.Add(Vector2Int.Down);
            }
            //左
            if (isDig(currentPos + Vector2Int.Left) && isDig(currentPos + Vector2Int.Left * ONCE_DIG_MAX))
            {
                canDigDirectionList.Add(Vector2Int.Left);
            }
            //右
            if (isDig(currentPos + Vector2Int.Right) && isDig(currentPos + Vector2Int.Right * ONCE_DIG_MAX))
            {
                canDigDirectionList.Add(Vector2Int.Right);
            }

            if (canDigDirectionList.Count == 0)
            {
                //掘れる方向がないなら終了
                break;
            }

            //自身を掘る
            if (currentPos != _position)
            {
                setStageType(currentPos, eStageType.FLOOR);
                //穴掘りを再開できる候補の場合は位置を保存しておく
                digCandidate.Add(currentPos);
            }

            //シード値の作成
            //ランダムに次の場所を決定
            Vector2Int nextDigPosition = canDigDirectionList[UnityEngine.Random.Range(0, canDigDirectionList.Count)];
            //決定した場所を掘る
            setStageType(currentPos + nextDigPosition, eStageType.FLOOR);
            setStageType(currentPos + nextDigPosition * ONCE_DIG_MAX, eStageType.FLOOR);

            //移動した先を現在の位置に設定
            currentPos = currentPos + nextDigPosition * ONCE_DIG_MAX;
        }

        //掘れる位置がなくなったら、穴掘り再開候補から場所を探す
        if (digCandidate.Count == 0)
        {
            //探せる箇所がなくなったので終了
            return;
        }
        //ランダムに再開場所を決定
        Vector2Int nextResumeDigPosition = digCandidate[UnityEngine.Random.Range(0, digCandidate.Count)];
        //次回穴掘り場所を穴掘り再開候補から消去
        digCandidate.Remove(nextResumeDigPosition);
        dig(nextResumeDigPosition);
    }

    /// <summary>
    /// 引数方向の位置を掘れるか
    /// </summary>
    /// <returns></returns>
    private bool isDig(Vector2Int _position)
    {
        return isDig(_position.x, _position.y);
    }

    /// <summary>
    /// 引数方向の位置を掘れるか
    /// </summary>
    /// <param name="_x"></param>
    /// <param name="_y"></param>
    /// <returns></returns>
    private bool isDig(int _x, int _y)
    {
        if (_x < 0 || _x >= width)
        {
            //エリア外のため掘れない
            return false;
        }
        if (_y < 0 || _y >= height)
        {
            //エリア外のため掘れない
            return false;
        }

        return getStageType(_x, _y) == eStageType.BLOCK;
    }

    /// <summary>
    /// ステージ情報取得
    /// </summary>
    /// <param name="_position"></param>
    /// <returns></returns>
    private eStageType getStageType(Vector2Int _position)
    {
        return getStageType(_position.x, _position.y);
    }

    /// <summary>
    /// ステージ情報取得
    /// </summary>
    /// <param name="_position"></param>
    /// <returns></returns>
    private eStageType getStageType(int _x, int _y)
    {
        if (_x < 0 || _x >= width)
        {
            //エリア外のため掘れない
            throw new ArgumentNullException();
        }
        if (_y < 0 || _y >= height)
        {
            //エリア外のため掘れない
            throw new ArgumentNullException();
        }
        return mazeInfo[_x, _y];
    }

    /// <summary>
    /// ステージ情報の設定
    /// </summary>
    /// <param name="_position"></param>
    /// <param name="_stageType"></param>
    private void setStageType(Vector2Int _position, eStageType _stageType)
    {
        setStageType(_position.x, _position.y, _stageType);
    }

    /// <summary>
    /// ステージ情報の設定
    /// </summary>
    /// <param name="_position"></param>
    /// <param name="_stageType"></param>
    private void setStageType(int _x, int _y, eStageType _stageType)
    {
        if (_x < 0 || _x >= width)
        {
            //エリア外のためセットできない
            throw new ArgumentNullException();
        }
        if (_y < 0 || _y >= height)
        {
            //エリア外のためセットできない
            throw new ArgumentNullException();
        }
        mazeInfo[_x, _y] = _stageType;
    }

    /// <summary>
    /// 通路かつxかyのどちらかが奇数の位置の時、一定の確率で天井を生成する
    /// </summary>
    private void createCeiling()
    {
        for (int i = 0; i < width; i++)
        {
            for (int k = 0; k < height; k++)
            {
                if (i % 2 == 1 && k % 2 == 1)
                {
                    // xもyも奇数の時
                    continue;
                }
                if (i % 2 == 0 && k % 2 == 0)
                {
                    // xもyも偶数の時
                    continue;
                }

                if(getStageType(i, k) != eStageType.FLOOR)
                {
                    //通路ではない
                    continue;
                }

                if(UnityEngine.Random .Range(0.0f, 1.0f) < ceilingPrecentage)
                {
                    setStageType(i, k, eStageType.CEILING);
                }
            }
        }
    }
}
