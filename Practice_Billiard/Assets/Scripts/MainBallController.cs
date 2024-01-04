using System;
using UnityEngine;

public class MainBallController : MonoBehaviour
{
    [SerializeField] private LineRenderer pullLine;
    [SerializeField] private Rigidbody mainBallRigid;
    [SerializeField] private float hitPower = 5.0f;
    [SerializeField] private float maxPower = 10.0f;

    private Vector3 clickPosition = Vector3.zero;
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    /// <summary>
    /// 引っ張り線のベクトルを取得
    /// </summary>
    /// <returns></returns>
    private Vector3 GetPullVector()
    {
        var viewPointVector = mainCamera.ScreenToViewportPoint(Input.mousePosition - clickPosition);
        viewPointVector *= 3.0f;     // マウスカーソルと実際の線がちょっとずれるので補正
        return new Vector3(viewPointVector.x, 0, viewPointVector.y);
    }

    /// <summary>
    /// ドラッグ開始
    /// </summary>
    private void OnMouseDown()
    {
        // ドラッグ開始位置を記録
        clickPosition = Input.mousePosition;

        // 引っ張り線を有効化
        pullLine.enabled = true;
        pullLine.SetPosition(0, mainBallRigid.position);
        pullLine.SetPosition(1, mainBallRigid.position);
    }

    /// <summary>
    /// ドラッグ中
    /// </summary>
    private void OnMouseDrag()
    {
        // 動いてる間は打てない
        if (!mainBallRigid.IsSleeping()) return;

        // 引っ張り線の長さと位置を修正
        pullLine.SetPosition(1, mainBallRigid.position + GetPullVector());
    }

    /// <summary>
    /// ドラッグ終了
    /// </summary>
    private void OnMouseUp()
    {
        // 動いてる間は打てない
        if (!mainBallRigid.IsSleeping()) return;

        // 引っ張り線を非表示
        pullLine.enabled = false;

        // ボールを射出
        var force = Vector3.ClampMagnitude((GetPullVector() * hitPower) * -1, maxPower);
        mainBallRigid.AddForce(force, ForceMode.Impulse);
    }
}
