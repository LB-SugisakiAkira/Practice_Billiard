using System;
using UnityEngine;

public class HoleController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TargetBall"))
        {
            other.gameObject.SetActive(false);
        }
        else if (other.CompareTag("MainBall"))
        {
            // ゲームオーバー
            Debug.Log("ゲームオーバー");
        }
    }
}
