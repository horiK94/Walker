using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour
{
    [SerializeField]
    private StageManager stageManager = null;
    [SerializeField]
    private EnemyCreator enemyCreator = null;
    [SerializeField]
    private EnemyAppearanceParameter enemyAppearanceParameter = null;
    [SerializeField]
    private UIManager uIManager = null;

    /// <summary>
    /// プレイヤーのプレファブ
    /// </summary>
    [SerializeField]
    private GameObject playerPrefab = null;

    /// <summary>
    /// プレイヤーの参照
    /// </summary>
    private GameObject player = null;

    private PlayerManager playerManager = null;

    List<Vector3> candidateSearcherPositions = null;

    private List<Searcher> searcherList = new List<Searcher>();

    private List<PursuerMover> pursuerList = new List<PursuerMover>();

    private EnemyAppearanceParameter.EnemyParam currentFloorEnemyParam = null;

    private bool isGameOver = false;

    private const float WAIT_NEXT_FLOOR = 2.0f;

    /// <summary>
    /// 初期化
    /// </summary>
    private void Awake()
    {
        ScoreDataManager scoreDataManager = ScoreDataManager.Instance;

        if (scoreDataManager.Floor == 0)
        {
            //初回起動時
            AudioManager.Instance.PlayBGM(AudioManager.eBGMAudioClip.BURNING_CAVERN);
        }

        scoreDataManager.GoUpstairs();
        uIManager.SetFloor(scoreDataManager.Floor);

        //現在の階の敵情報を取得
        currentFloorEnemyParam = enemyAppearanceParameter.GetParameter(scoreDataManager.Floor);

        createStage();
        createPlayer();
        createSearchers(currentFloorEnemyParam.Searcher);
    }

    private void Update()
    {
        if (isGameOver && Input.GetKeyDown(KeyCode.R))
        {
            retryGame();
        }
    }

    private void createStage()
    {
        //ステージ生成
        stageManager.CreateStage();

        candidateSearcherPositions = stageManager.GetRoomPositions();

        //ゴール設定
        stageManager.GoalInstance.GetComponent<ChestController>().Init(() =>
        {
            arriveGoal();
        });
    }

    /// <summary>
    /// プレイヤーの生成
    /// </summary>
    private void createPlayer()
    {
        //プレイヤー生成
        Vector3 startPos = stageManager.GetStartCenterPosition();
        player = Instantiate(playerPrefab, startPos + 2 * Vector3.up, Quaternion.LookRotation(stageManager.GetGoalCenterPosition(), Vector3.up));
        playerManager = player.GetComponent<PlayerManager>();
        playerManager.SetFirstMoveAction(() =>
        {
            for (int i = 0; i < searcherList.Count; i++)
            {
                searcherList[i].Play();
            }
            createPursuers(currentFloorEnemyParam.Pursuer);
        });
        playerManager.SetOnCollisionEnemyAction(collisionEnemy);
    }

    /// <summary>
    /// 探索者達の生成
    /// </summary>
    private void createSearchers(int _num)
    {
        for (int i = 0; i < _num; i++)
        {
            createSearcher();
        }
    }

    /// <summary>
    /// 探索者の生成
    /// </summary>
    private void createSearcher()
    {
        int drewNumber = Random.Range(0, candidateSearcherPositions.Count);

        Vector3 createGoalPoint = candidateSearcherPositions[drewNumber] + Vector3.up;
        Searcher searcher = enemyCreator.CreateSearcher(createGoalPoint).GetComponent<Searcher>();
        searcher.SetPlayerTransform(player.transform);
        searcher.Pause();
        searcherList.Add(searcher);

        candidateSearcherPositions.RemoveAt(drewNumber);
    }

    /// <summary>
    /// 追跡者達の生成
    /// </summary>
    private void createPursuers(int _num)
    {
        for (int i = 0; i < _num; i++)
        {
            createPursuer(3.0f + i + 2.0f);
        }
    }

    /// <summary>
    /// 追跡者の生成
    /// </summary>
    /// <param name="_delayTime"></param>
    private void createPursuer(float _delayTime)
    {
        //追跡者の生成
        Vector3 createStartPoint = stageManager.GetStartCenterPosition() + Vector3.up;
        PursuerMover pursuerMover = enemyCreator.CreatePursuer(createStartPoint).GetComponent<PursuerMover>();
        pursuerMover.StartRecord(player.transform, _delayTime);
        pursuerList.Add(pursuerMover);
    }

    /// <summary>
    /// ゴール到着
    /// </summary>
    private void arriveGoal()
    {
        stopAllChara();

        StartCoroutine(goToNextFloor());
    }

    /// <summary>
    /// 次の階へ移動
    /// </summary>
    /// <returns></returns>
    private IEnumerator goToNextFloor()
    {
        yield return new WaitForSeconds(WAIT_NEXT_FLOOR);

        SceneManager.LoadScene("Game");
    }

    /// <summary>
    /// 敵キャラと衝突時
    /// </summary>
    private void collisionEnemy()
    {
        isGameOver = true;

        AudioManager audioManager = AudioManager.Instance;
        audioManager.StopBGM();
        audioManager.StopSE();

        audioManager.PlayOneShotSE(AudioManager.eSEAudioClip.WHISTLE);
        audioManager.SetSESpatialBlend(false);

        stopAllChara();

        uIManager.AppearRetryText();

        //現在の階の1つ下の階がクリア階となる
        naichilab.RankingLoader.Instance.SendScoreAndShowRanking(ScoreDataManager.Instance.Floor - 1);
    }

    private void retryGame()
    {
        ScoreDataManager.Instance.Reset();

        SceneManager.LoadScene("Game");
    }

    /// <summary>
    /// 全キャラ停止
    /// </summary>
    private void stopAllChara()
    {
        playerManager.Pause();

        for (int i = 0; i < searcherList.Count; i++)
        {
            searcherList[i].Pause();
        }

        for (int i = 0; i < pursuerList.Count; i++)
        {
            pursuerList[i].Pause();
        }
    }
}
