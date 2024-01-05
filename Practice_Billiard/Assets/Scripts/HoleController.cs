using UnityEngine;

public class HoleController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TargetBall"))
        {
            GameManager.instance.GetBall(other.gameObject);
        }
        else if (other.CompareTag("MainBall"))
        {
            // プレイヤー交代
            // 手球を好きなとこに置ける
        }
    }
}
