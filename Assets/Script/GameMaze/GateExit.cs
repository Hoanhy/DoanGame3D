using UnityEngine;

public class GateExit : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance.hasStudentCard)
            {
                Debug.Log("Chúc mừng! Bạn đã nhập học thành công!");
                Time.timeScale = 0f;
            }
            else
            {
                Debug.Log("Bạn cần nộp hồ sơ trước!");
            }
        }
    }
}