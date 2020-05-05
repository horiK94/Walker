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
    /// 床レイヤー番号
    /// </summary>
    private const int FLOOR_LAYER_ID = 12;

    /// <summary>
    /// ステージの全長(外枠、壁、部屋の全大きさ)
    /// </summary>
    private Vector2Int stageSize = null;

    /// <summary>
    /// 各要素のサイズを考慮しないステージ全体情報
    /// </summary>
    private eStageType[,] mazeDataNotConsiderElementsSize = null;

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
    /// コライダーオブジェクト
    /// </summary>
    [SerializeField]
    private GameObject colliderPrefab = null;

    /// <summary>
    /// ゴール位置に置かれるプレファブ
    /// </summary>
    [SerializeField]
    private GameObject goalPrefab = null;

    /// <summary>
    /// ゴールインスタンス
    /// </summary>
    public GameObject GoalInstance { get; private set; }

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
        mazeDataNotConsiderElementsSize = mazeCreator.CreateMaze(STAGE_SIZE_NOT_CONSIDER_ELEMENTS_SIZE, START_POINT, GOAL_POINT, ceilingPercentage);
        mazeData = convertMazeDataToStageSize(mazeDataNotConsiderElementsSize);

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

        //コライダーの作成
        makeCollider();
        //ゴール位置にゴールプレファブを置く
        GoalInstance = Instantiate(goalPrefab, GetGoalCenterPosition() + Vector3.up, Quaternion.identity, parentTransform) as GameObject;
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
    public Vector3 GetGoalCenterPosition()
    {
        return getPurposeCenterPosition(eStageType.GOAL);
    }

    /// <summary>
    /// ステージ内側位置か
    /// </summary>
    /// <param name="_checkPoint">(0, 0) ～</param>
    /// <returns></returns>
    private bool isInside(Vector2Int _checkPoint)
    {
        return _checkPoint.x >= 0 && _checkPoint.x < stageSize.x && _checkPoint.y >= 0 && _checkPoint.y < stageSize.y;
    }

    /// <summary>
    /// 特定のオブジェクトの中心点を返す
    /// </summary>
    /// <param name="_purposeType"></param>
    /// <returns></returns>
    private Vector3 getPurposeCenterPosition(eStageType _purposeType)
    {
        Vector3 sumGoalPosition = Vector3.zero;
        int purposePositionCount = 0;
        for (int i = 0; i < mazeData.GetLength(0); i++)
        {
            for (int k = 0; k < mazeData.GetLength(1); k++)
            {
                if (mazeData[i, k] == _purposeType)
                {
                    sumGoalPosition += new Vector3(i, 0, k);
                    purposePositionCount++;
                }
            }
        }

        if (purposePositionCount == 0)
        {
            return Vector3.zero;
        }
        return sumGoalPosition / purposePositionCount;
    }

    /// <summary>
    /// スタートと、ゴール以外の部屋の位置を返す
    /// </summary>
    /// <returns></returns>
    public List<Vector3> GetRoomPositions()
    {
        Vector2Int floorSize = new Vector2Int(STAGE_SIZE_NOT_CONSIDER_ELEMENTS_SIZE.x / 2 + 1, STAGE_SIZE_NOT_CONSIDER_ELEMENTS_SIZE.y / 2 + 1);

        List<Vector3> candidatePositions = new List<Vector3>();
        Vector3 candidatePos = Vector3.zero;
        for (int i = 0; i < floorSize.x; i++)
        {
            candidatePos = new Vector3((ROOM_SIZE.x - 1) / 2.0f + OUTER_WALL_THICKNESS + i * (ROOM_SIZE.x + WALL_CANDIDATE_THICKNESS), 0, (ROOM_SIZE.y - 1) / 2.0f + OUTER_WALL_THICKNESS);
            for (int j = 0; j < floorSize.y; j++)
            {
                if (mazeDataNotConsiderElementsSize[2 * i, 2 * j] != eStageType.START && mazeDataNotConsiderElementsSize[2 * i, 2 * j] != eStageType.GOAL)
                {
                    candidatePositions.Add(candidatePos);
                }

                candidatePos += new Vector3(0, 0, ROOM_SIZE.y + WALL_CANDIDATE_THICKNESS);
            }
        }

        return candidatePositions;
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
            }
        }
    }

    /// <summary>
    /// コライダーの作成
    /// </summary>
    private void makeCollider()
    {
        //床のコライダー作成
        GameObject floorColliderObj = Instantiate(colliderPrefab, parentTransform) as GameObject;
        floorColliderObj.transform.position = new Vector3(stageSize.x, 0, stageSize.y) / 2;
        floorColliderObj.GetComponent<BoxCollider>().size = new Vector3(stageSize.x, 1, stageSize.y);
        floorColliderObj.layer = FLOOR_LAYER_ID;

        //壁等のコライダー作成
        var colliderPosList = calcColliderPositionList();
        for (int i = 0; i < colliderPosList.Count; i++)
        {
            Vector2Int[] posInfo = colliderPosList[i];
            Vector2Int startPos = posInfo[0];
            Vector2Int goalPos = posInfo[1];

            Vector3 centerPos = getCenterPosition(startPos, goalPos);
            Vector3 size = getColliderSize(startPos, goalPos);

            GameObject colliderObj = Instantiate(colliderPrefab, parentTransform) as GameObject;
            colliderObj.transform.position = centerPos;
            colliderObj.GetComponent<BoxCollider>().size = size;
        }
    }

    /// <summary>
    /// 引数のタイプがコライダーをアタッチするオブジェクトか
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private bool isCollisionType(eStageType type)
    {
        return type == eStageType.BLOCK || type == eStageType.OUTER_WALL;
    }

    /// <summary>
    /// コライダーを置く場所を受け取る
    /// </summary>
    /// <returns></returns>
    private List<Vector2Int[]> calcColliderPositionList()
    {
        List<Vector2Int[]> colliderPosList = new List<Vector2Int[]>();
        for (int i = 0; i < mazeData.GetLength(0); i++)
        {
            bool isCheck = false;
            Vector2Int checkStartPos = null;
            Vector2Int checkGoalPos = null;
            for (int k = 0; k < mazeData.GetLength(1); k++)
            {
                eStageType type = mazeData[i, k];
                if (!isCheck && isCollisionType(type))
                {
                    //ブロック
                    isCheck = true;
                    checkStartPos = new Vector2Int(i, k);
                    continue;
                }

                if (isCheck && isCollisionType(type) && k != mazeData.GetLength(1) - 1)
                {
                    //調査開始後にブロックが続いている時
                    checkGoalPos = new Vector2Int(i, k);
                    continue;
                }

                if (isCheck && (!isCollisionType(type) || k == mazeData.GetLength(1) - 1))
                {
                    //調査開始後にブロックでないマスに来た時
                    if (checkGoalPos != null)
                    {
                        //ここでcheckGoalPosがnullでなかったら2マス以上ブロックでないマスが続いていることになる
                        colliderPosList.Add(new Vector2Int[] { checkStartPos, checkGoalPos });
                    }
                    isCheck = false;
                    checkGoalPos = null;
                }
            }
        }

        for (int i = 0; i < mazeData.GetLength(0); i++)
        {
            bool isCheck = false;
            Vector2Int checkStartPos = null;
            Vector2Int checkGoalPos = null;
            for (int k = 0; k < mazeData.GetLength(1); k++)
            {
                eStageType type = mazeData[k, i];
                if (!isCheck && isCollisionType(type))
                {
                    //ブロック
                    isCheck = true;
                    checkStartPos = new Vector2Int(k, i);
                    continue;
                }

                if (isCheck && isCollisionType(type) && k != mazeData.GetLength(1) - 1)
                {
                    //調査開始後にブロックが続いている時
                    checkGoalPos = new Vector2Int(k, i);
                    continue;
                }

                if (isCheck && (!isCollisionType(type) || k == mazeData.GetLength(1) - 1))
                {
                    //調査開始後にブロックでないマスに来た時
                    if (checkGoalPos != null)
                    {
                        //ここでcheckGoalPosがnullでなかったら2マス以上ブロックでないマスが続いていることになる
                        colliderPosList.Add(new Vector2Int[] { checkStartPos, checkGoalPos });
                    }
                    isCheck = false;
                    checkGoalPos = null;
                }
            }
        }

        return colliderPosList;
    }

    /// <summary>
    /// 中心座標を返す
    /// </summary>
    /// <param name="_startPos"></param>
    /// <param name="_goalPos"></param>
    /// <returns></returns>
    private Vector3 getCenterPosition(Vector2Int _startPos, Vector2Int _goalPos)
    {
        Vector2 centerPos = (_startPos + _goalPos) / 2;
        return new Vector3(centerPos.x, (CEILING_HEIGHT + 1) / 2.0f, centerPos.y);
    }

    /// <summary>
    /// _startPosから_goalPosまで網羅するコライダーの大きさを返す
    /// </summary>
    /// <param name="_startPos"></param>
    /// <param name="_goalPos"></param>
    /// <returns></returns>
    private Vector3 getColliderSize(Vector2Int _startPos, Vector2Int _goalPos)
    {
        return new Vector3(Mathf.Abs(_startPos.x - _goalPos.x) + 1, CEILING_HEIGHT, Mathf.Abs(_startPos.y - _goalPos.y) + 1);
    }
}
