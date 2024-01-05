using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    private List<string> targetBallList = new List<string>();    // 盤上にあるボール一覧
    private List<string> getBallList = new List<string>();       // 今落としたボール一覧
    private List<string> playerList = new List<string>();        // プレイヤー一覧

    private int turnCount = 0;              // 突くのが何回目か
    private int currentPlayerIndex = 0;     // 今ターンのプレイヤー
    private int nextBallNumber = 1;         // 次落とすべきボールの番号

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

        // ターン情報初期化
        turnCount = 0;
        currentPlayerIndex = 0;
        nextBallNumber = 1;
    }

    // ホールにボールが落ちた時の処理
    public void GetBall(GameObject ballObject)
    {
        ballObject.SetActive(false);
        getBallList.Add(ballObject.name);
    }

    // ターン終了時処理
    private void OnEndTurn()
    {
        isMainBallMoving = false;

        // 落としたボールを表示
        foreach (var ball in getBallList)
        {
            Debug.Log(ball);
        }
        getBallList.Clear();

        // プレイヤー交代
        TurnNextPlayer();

        // 次あてるべき的球を赤枠で表示
    }

    // プレイヤー交代
    private void TurnNextPlayer()
    {
        currentPlayerIndex++;
        if (currentPlayerIndex >= playerList.Count) currentPlayerIndex = 0;
        Debug.Log($"Next player is {playerList[currentPlayerIndex]}");
        //UI表示
    }

    // リセット処理
    public void Reset()
    {
        //ボールを初期位置に戻す

        InitializeInformation();
    }

}
