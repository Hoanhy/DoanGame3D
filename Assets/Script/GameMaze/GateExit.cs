using UnityEngine;

public class GateExit : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance.hasStudentCard)
            {
                GameManager.Instance.WinGame();
            }
            else
            {
                Debug.Log("Bạn cần nộp hồ sơ trước!");
            }
        }
    }
}