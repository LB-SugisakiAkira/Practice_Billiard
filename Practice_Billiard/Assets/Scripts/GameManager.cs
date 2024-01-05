using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    [SerializeField] private Text playerTest;
    [SerializeField] private Text turnText;
    [SerializeField] private Text remainText;
    [SerializeField] private Text nextBallText;

    private List<string> targetBallList = new List<string>();    // 盤上にあるボール一覧
    private List<string> getBallList = new List<string>();       // 今落としたボール一覧
    private List<string> playerList = new List<string>();        // プレイヤー一覧

    public int nextBallNumber { get; private set; } = 1;         // 次落とすべきボールの番号
    private int turnCount = 1;                                   // 突くのが何回目か
    private int currentPlayerIndex = 0;                          // 今ターンのプレイヤー

    public bool isMainBallMoving { get; private set; } = false;
    public Subject<Unit> ballStartSubject = new Subject<Unit>();
    public Subject<Unit> ballStopSubject = new Subject<Unit>();
    public IObservable<Unit> BallStartObservable => ballStartSubject;
    public IObservable<Unit> BallStopObservable => ballStopSubject;


    private void Awake()
    {
        // シングルトン化処理
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        // プレイヤーリスト作成
        for (int i = 0; i < 3; i++)
        {
            playerList.Add($"PLAYER{i + 1}");
        }

        // ステータス初期化
        InitializeInformation();

        // 表示初期化
        RedisplayInformation();

        BallStartObservable.Subscribe(_ => isMainBallMoving = true).AddTo(this);
        BallStopObservable.Subscribe(_ => OnEndTurn()).AddTo(this);
    }

    private void InitializeInformation()
    {
        // ボール一覧作成
        var balls = GameObject.FindGameObjectsWithTag("TargetBall");
        targetBallList.Clear();
        foreach (var ballObject in balls)
        {
            targetBallList.Add(ballObject.name);
        }
        targetBallList.Sort();

        // ターン情報初期化
        turnCount = 1;
        currentPlayerIndex = 0;
        nextBallNumber = 1;
    }

    // ホールにボールが落ちた時の処理
    public void GetBall(GameObject ballObject)
    {
        ballObject.layer = 3;   // 下に落とすために当たり判定をなくす
        ballObject.GetComponent<Collider>();

        getBallList.Add(ballObject.name);
    }

    // ターン終了時処理
    private void OnEndTurn()
    {
        isMainBallMoving = false;
        turnCount++;

        foreach (var ball in getBallList)
        {
            // 落としたボールを表示
            Debug.Log(ball);

            // 残りのリストから削除
            var deleteIndex = targetBallList.IndexOf(ball);
            targetBallList.RemoveAt(deleteIndex);
        }
        getBallList.Clear();

        // プレイヤー交代
        TurnNextPlayer();

        // ターン情報再表示
        RedisplayInformation();
    }

    /// <summary>
    /// プレイヤー交代
    /// </summary>
    private void TurnNextPlayer()
    {
        currentPlayerIndex++;
        if (currentPlayerIndex >= playerList.Count) currentPlayerIndex = 0;
        Debug.Log($"Next player is {playerList[currentPlayerIndex]}");
    }

    /// <summary>
    /// プレイヤー、ターン数、残りの玉の表示を再描画する
    /// </summary>
    private void RedisplayInformation()
    {
        playerTest.text = $"Player : {playerList[currentPlayerIndex]}";
        turnText.text = $"Turn : {turnCount}";

        // 残りの玉
        var remainBall = targetBallList.Aggregate("Remain : ", (current, ball) => current + ConvertCircleNumber(ball));
        remainText.text = remainBall;

        // nextBallText.text = $"Player : {playerList[currentPlayerIndex]}";
    }

    /// <summary>
    /// Ball_?の文字列を〇付きの数字の文字に置換する
    /// </summary>
    /// <param name="ballName"></param>
    /// <returns></returns>
    private string ConvertCircleNumber(string ballName)
    {
        var number = ballName switch
        {
            "Ball_1" => "①",
            "Ball_2" => "②",
            "Ball_3" => "③",
            "Ball_4" => "④",
            "Ball_5" => "⑤",
            "Ball_6" => "⑥",
            "Ball_7" => "⑦",
            "Ball_8" => "⑧",
            "Ball_9" => "⑨",
            _ => string.Empty
        };

        return number;
    }

    /// <summary>
    /// ファール時の処理
    /// </summary>
    public void FreeBall()
    {
        OnEndTurn();

        //ボールを好きな位置におけるようにする
    }

    // リセット処理
    public void Reset()
    {
        //ボールを初期位置に戻す

        InitializeInformation();
    }

}
