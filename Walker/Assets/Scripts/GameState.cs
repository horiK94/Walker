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

    private List<Searcher> searcherList = new List<Searcher>();

    private List<PursuerMover> pursuerList = new List<PursuerMover>();

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

        createStage();
        createPlayer();
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

        //ステージ設定
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
        playerManager.SetFirstMoveAction(createEnemy);
        playerManager.SetOnCollisionEnemyAction(collisionEnemy);
    }

    /// <summary>
    /// 敵の生成
    /// </summary>
    private void createEnemy()
    {
        Vector3 createGoalPoint = stageManager.GetGoalCenterPosition() + Vector3.up;
        Searcher searcher = enemyCreator.CreateSearcher(createGoalPoint).GetComponent<Searcher>();
        searcher.SetPlayerTransform(player.transform);
        searcherList.Add(searcher);

        Vector3 createStartPoint = stageManager.GetStartCenterPosition() + Vector3.up;
        PursuerMover pursuerMover = enemyCreator.CreatePursuer(createStartPoint).GetComponent<PursuerMover>();
        pursuerMover.StartRecord(player.transform, 3.0f);
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
