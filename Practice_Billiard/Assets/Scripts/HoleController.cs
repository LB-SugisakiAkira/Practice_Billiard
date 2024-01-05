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
            gameObject.SetActive(false);
            GameManager.instance.FreeBall();
        }
    }
}
