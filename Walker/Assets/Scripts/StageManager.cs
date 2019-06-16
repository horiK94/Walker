using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StageManager : MonoBehaviour
{
    /// <summary>
    /// ステージサイズ(各要素の大きさは考慮しない)
    /// </summary>
    private readonly Vector2Int STAGE_SIZE_NOT_CONSIDER_ELEMENTS_SIZE = new Vector2Int(9, 9);
    /// <summary>
    /// ステージの偶数座標上に置かれる部屋のサイズ
    /// </summary>
    private readonly Vector2Int ROOM_SIZE = new Vector2Int(2, 2);
    /// <summary>
    /// ステージの座標のいずれかが奇数の座標に置かれる壁の厚さ(部屋のサイズに左右されない方の大きさ)
    /// </summary>
    private const int WALL_CANDIDATE_THICKNESS = 1;

    /// <summary>
    /// 外枠の壁の厚さ(基本1で設定)
    /// </summary>
    private const int OUTER_WALL_THICKNESS = 1;
    /// <summary>
    /// 0が床としたときの壁の高さ
    /// </summary>
    private const int CEILING_HEIGHT = 2;

    /// <summary>
    /// ステージ開始位置
    /// </summary>
    private readonly Vector2Int START_POINT = new Vector2Int(8, 8);
    /// <summary>
    /// ステージゴール位置
    /// </summary>
    private readonly Vector2Int GOAL_POINT = new Vector2Int(0, 0);

    /// <summary>
    /// ステージの全長(外枠、壁、部屋の全大きさ)
    /// </summary>
    private Vector2Int stageSize = null;

    /// <summary>
    /// ステージ全体情報
    /// </summary>
    private eStageType[,] mazeData = null;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float ceilingPercentage = 0f;

    /// <summary>
    /// ステージ親オブジェクト
    /// </summary>
    [SerializeField]
    private Transform parentTransform = null;

    /// <summary>
    /// 床プレファブ
    /// </summary>
    [SerializeField]
    private GameObject floorPregfab = null;

    /// <summary>
    /// 壁・床プレファブ
    /// </summary>
    [SerializeField]
    private GameObject wallPrefab = null;

    /// <summary>
    /// 外枠プレファブ
    /// </summary>
    [SerializeField]
    private GameObject wallOutPrefab = null;

    /// <summary>
    /// ゴール位置に置かれるプレファブ
    /// </summary>
    [SerializeField]
    private GameObject goalPrefab = null;

    /// <summary>
    /// 初期化
    /// </summary>
    private void Awake()
    {
        //基本情報の設定
        Vector2Int roomCount = new Vector2Int(STAGE_SIZE_NOT_CONSIDER_ELEMENTS_SIZE.x / 2 + 1, STAGE_SIZE_NOT_CONSIDER_ELEMENTS_SIZE.y / 2 + 1);
        Vector2Int wallCandidate = new Vector2Int(STAGE_SIZE_NOT_CONSIDER_ELEMENTS_SIZE.x - roomCount.x, STAGE_SIZE_NOT_CONSIDER_ELEMENTS_SIZE.y - roomCount.y);
        stageSize = new Vector2Int(
            roomCount.x * ROOM_SIZE.x + wallCandidate.x * WALL_CANDIDATE_THICKNESS + 2 * OUTER_WALL_THICKNESS,
            roomCount.y * ROOM_SIZE.y + wallCandidate.y * WALL_CANDIDATE_THICKNESS + 2 * OUTER_WALL_THICKNESS);
    }

    /// <summary>
    /// STAGE_SIZE_NOT_CONSIDER_ELEMENTS_SIZEをstageSizeのステージデータに入れ直す
    /// </summary>
    /// <param name="_mazeData"></param>
    /// <returns></returns>
    private eStageType[,] convertMazeDataToStageSize(eStageType[,] _mazeData)
    {
        //調査開始箇所
        Vector2Int checkStartPoint = new Vector2Int(OUTER_WALL_THICKNESS, OUTER_WALL_THICKNESS);
        //調査終了箇所
        Vector2Int checkFinishPoint = new Vector2Int(OUTER_WALL_THICKNESS, OUTER_WALL_THICKNESS);
        //次回調査開始箇所
        Vector2Int nextCheckStartPoint = new Vector2Int(OUTER_WALL_THICKNESS, OUTER_WALL_THICKNESS);

        //返り値用ステージデータ
        eStageType[,] mazeDataToStageSize = new eStageType[stageSize.x, stageSize.y];

        for (int k = 0; k < STAGE_SIZE_NOT_CONSIDER_ELEMENTS_SIZE.y; k++)
        {
            for (int i = 0; i < STAGE_SIZE_NOT_CONSIDER_ELEMENTS_SIZE.x; i++)
            {
                //調査開始位置を次回調査箇所に合わせる
                checkStartPoint = nextCheckStartPoint;
                //調査終了箇所も開始位置に合わせておく
                checkFinishPoint = checkStartPoint;

                eStageType type = _mazeData[i, k];
                if (i % 2 == 0 && k % 2 == 0)
                {
                    //床
                    Vector2Int floorSize = ROOM_SIZE;
                    //開始位置に対して終了位置は部屋サイズからx, yともに1ずつ小さい箇所
                    checkFinishPoint += (ROOM_SIZE - new Vector2Int(1, 1));
                    //次回調査開始箇所の設定
                    nextCheckStartPoint += new Vector2Int(ROOM_SIZE.x, 0);
                }
                else if (k % 2 == 0)
                {
                    //壁 or 床 or 屋根
                    //開始位置に対して終了位置は「壁の厚さ×部屋のy方向へのサイズ」からx, yともに1ずつ小さい箇所
                    checkFinishPoint += new Vector2Int(WALL_CANDIDATE_THICKNESS - 1, ROOM_SIZE.y - 1);
                    //次回調査開始箇所の設定
                    nextCheckStartPoint += new Vector2Int(WALL_CANDIDATE_THICKNESS, 0);
                }
                else if (i % 2 == 0)
                {
                    //壁 or 床 or 屋根
                    //開始位置に対して終了位置は「部屋のx方向へのサイズ×壁の厚さ」からx, yともに1ずつ小さい箇所
                    checkFinishPoint += new Vector2Int(ROOM_SIZE.x - 1, WALL_CANDIDATE_THICKNESS - 1);
                    //次回調査開始箇所の設定
                    nextCheckStartPoint += new Vector2Int(ROOM_SIZE.x, 0);
                }
                else
                {
                    //iもkも奇数の時は必ず壁
                    //開始位置に対して終了位置は「壁の厚さ×壁の厚さ」からx, yともに1ずつ小さい箇所
                    checkFinishPoint += new Vector2Int(WALL_CANDIDATE_THICKNESS - 1, WALL_CANDIDATE_THICKNESS - 1);
                    //次回調査開始箇所の設定
                    nextCheckStartPoint += new Vector2Int(WALL_CANDIDATE_THICKNESS, 0);
                }
                if (nextCheckStartPoint.x >= stageSize.x - 1)
                {
                    //これ以上x座標にステージデータを入れる予定がない場合は次の行へ移動
                    nextCheckStartPoint = new Vector2Int(1, checkFinishPoint.y + 1);
                }
                mazeDataToStageSize = setStageTypeForRange(mazeDataToStageSize, type, checkStartPoint, checkFinishPoint);
            }
        }
        for (int i = 0; i < stageSize.x; i++)
        {
            for (int k = 0; k < stageSize.y; k++)
            {
                if (i == 0 || k == 0 || i == stageSize.x - 1 || k == stageSize.y - 1)
                {
                    //見えない外壁の設置
                    mazeDataToStageSize[i, k] = eStageType.OUTER_WALL;
                }
            }
        }
        return mazeDataToStageSize;
    }

    /// <summary>
    /// _startPointから_endPointの範囲を_setTypeにする
    /// </summary>
    /// <param name="_mazeData"></param>
    /// <param name="_setType"></param>
    /// <param name="_startPoint"></param>
    /// <param name="_endPoint"></param>
    /// <returns></returns>
    private eStageType[,] setStageTypeForRange(eStageType[,] _mazeData, eStageType _setType, Vector2Int _startPoint, Vector2Int _endPoint)
    {
        eStageType[,] data = _mazeData;
        if (_endPoint.x > stageSize.x)
        {
            throw new ArgumentOutOfRangeException("_endPoint", "xの値が不正です");
        }
        if (_endPoint.y > stageSize.y)
        {
            throw new ArgumentOutOfRangeException("_endPoint", "xの値が不正です");
        }
        if (_startPoint.x > _endPoint.x)
        {
            throw new ArgumentOutOfRangeException("_startPoint, _endPoint", "xの大小関係が不正です");
        }
        if (_startPoint.y > _endPoint.y)
        {
            throw new ArgumentOutOfRangeException("_startPoint, _endPoint", "yの大小関係が不正です");
        }
        for (int i = _startPoint.x; i <= _endPoint.x; i++)
        {
            for (int k = _startPoint.y; k <= _endPoint.y; k++)
            {
                data[i, k] = _setType;
            }
        }
        return data;
    }

    /// <summary>
    /// ステージの生成
    /// </summary>
    public void CreateStage()
    {
        //ステージデータ作成
        MazeCreator mazeCreator = new MazeCreator();
        eStageType[,] mazeDataNotConsiderElementsSize = mazeCreator.CreateMaze(STAGE_SIZE_NOT_CONSIDER_ELEMENTS_SIZE, START_POINT, GOAL_POINT, ceilingPercentage);
        eStageType[,] maze = convertMazeDataToStageSize(mazeDataNotConsiderElementsSize);
        mazeData = maze;

        //ステージの生成
        createStageObject(mazeData);
    }

    /// <summary>
    /// ステージの生成
    /// </summary>
    /// <param name="_mazeData"></param>
    private void createStageObject(eStageType[,] _mazeData)
    {
        //ステージの生成
        for (int i = 0; i < _mazeData.GetLength(0); i++)
        {
            for (int k = 0; k < _mazeData.GetLength(1); k++)
            {
                createStageObjectToPoint(i, k, _mazeData[i, k]);
            }
        }
        //ゴール位置にゴールプレファブを置く
        Instantiate(goalPrefab, getGoalCenterPosition() + Vector3.up, Quaternion.identity, parentTransform);
    }

    /// <summary>
    /// スタート地点の中心点を返す
    /// </summary>
    /// <returns></returns>
    public Vector3 GetStartCenterPosition()
    {
        return getPurposeCenterPosition(eStageType.START);
    }

    /// <summary>
    /// ゴール地点の中心点を返す
    /// </summary>
    /// <returns></returns>
    private Vector3 getGoalCenterPosition()
    {
        return getPurposeCenterPosition(eStageType.GOAL);
    }

    /// <summary>
    /// 特定のオブジェクトの中心点を返す
    /// </summary>
    /// <param name="_purposeType"></param>
    /// <returns></returns>
    private Vector3 getPurposeCenterPosition(eStageType _purposeType)
    {
        Vector3 sumGoalPosition = Vector3.zero;
        int goalPositionCount = 0;
        for (int i = 0; i < mazeData.GetLength(0); i++)
        {
            for (int k = 0; k < mazeData.GetLength(1); k++)
            {
                if (mazeData[i, k] == _purposeType)
                {
                    sumGoalPosition += new Vector3(i, 0, k);
                    goalPositionCount++;
                }
            }
        }
        return sumGoalPosition / goalPositionCount;
    }

    /// <summary>
    /// 引数の位置にオブジェクトを置く
    /// </summary>
    /// <param name="_x"></param>
    /// <param name="_y"></param>
    /// <param name="_type"></param>
    private void createStageObjectToPoint(int _x, int _y, eStageType _type)
    {
        //(0, 0, 0)の位置に_x = 0, _y = 0のプレファブが置かれるようにする
        //outer_wallを除く一番下は床
        if (_type != eStageType.OUTER_WALL)
        {
            Instantiate(floorPregfab, new Vector3(_x, 0, _y), Quaternion.identity, parentTransform);
        }

        if (_type == eStageType.FLOOR
            || _type == eStageType.START
            || _type == eStageType.GOAL)
        {
            //床の場合はこれ以上上に何も生成しないので終了
            return;
        }

        for (int i = 1; i <= CEILING_HEIGHT; i++)
        {
            switch (_type)
            {
                case eStageType.CEILING:
                    if (i == CEILING_HEIGHT)
                    {
                        //天井のときは一番高い高さのときのみ生成する
                        Instantiate(wallPrefab, new Vector3(_x, i, _y), Quaternion.identity, parentTransform);
                    }
                    break;
                case eStageType.BLOCK:
                    //ブロックのときは一番高い高さまで生成する
                    Instantiate(wallPrefab, new Vector3(_x, i, _y), Quaternion.identity, parentTransform);
                    break;
                case eStageType.OUTER_WALL:
                    //外枠のときは一番高い高さまで生成する
                    Instantiate(wallOutPrefab, new Vector3(_x, i, _y), Quaternion.identity, parentTransform);
                    break;
            }
        }
    }
}
