using UnityEngine;

public class MainBallController : MonoBehaviour
{
    [SerializeField] private GameObject mainBall;
    [SerializeField] private GameObject pullLine;
    [SerializeField] private float hitPower = 0.8f;
    [SerializeField] private RectTransform imageRect;

    private Rigidbody mainBallRigid;
    private Transform mainBallTransform;

    private Vector3 clickPosition = new Vector3();
    private void Start()
    {
        mainBallRigid = mainBall.GetComponent<Rigidbody>();
        mainBallTransform = mainBall.GetComponent<Transform>();

        pullLine.SetActive(false);
    }

    private void Update()
    {
        if (mainBall.activeSelf)
        {
            // マウスクリック開始
            if (Input.GetMouseButtonDown(0))
            {
                // 開始位置を保管
                clickPosition = Input.mousePosition;

                // 方向線を表示
                pullLine.SetActive(true);
                ResizePullLine(mainBallTransform.position);
            }
            // マウスクリック中
            else if (Input.GetMouseButton(0))
            {
                // 引っ張り線を書き直す
                ResizePullLine(clickPosition);
            }
            // クリックを離した
            else if (Input.GetMouseButtonUp(0))
            {
                // 開始位置と終了位置から方向を算出
                var defPosition = clickPosition - Input.mousePosition;
                var addVector = new Vector3(defPosition.x, 0, defPosition.y);

                // ボールを打ち出す
                mainBallRigid.AddForce(addVector * hitPower);

                // 線を非表示
                pullLine.SetActive(false);
            }
        }
    }

    private void ResizePullLine(Vector3 basePosition)
    {
        // 角度を算出
        var defPosition = basePosition - Input.mousePosition;
        var angle = Mathf.Atan2(defPosition.x, defPosition.y) * Mathf.Rad2Deg;
        var quaternion = Quaternion.Euler(new Vector3(0, angle, 0));

        // 距離を算出
        var distance = Vector3.Distance(clickPosition, mainBallTransform.position) / 50;

        // 引っ張り線を書き直す
        pullLine.transform.localRotation = quaternion;
        pullLine.transform.position = mainBallTransform.position;
        // imageRect.sizeDelta = new Vector2(distance, imageRect.sizeDelta.y);
    }
}
