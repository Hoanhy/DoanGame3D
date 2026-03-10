using UnityEngine;

public class NPCSubmit : MonoBehaviour
{
    bool playerNear = false;

    void Update()
    {
        if (playerNear && Input.GetKeyDown(KeyCode.E))
        {
            if (GameManager.Instance.hasStudentCard)
            {
                Debug.Log("Bạn đã nộp hồ sơ rồi!");
                return;
            }

            if (GameManager.Instance.currentDocuments >= 4)
            {
                Debug.Log("Bạn đã nộp hồ sơ thành công!");
                Debug.Log("Bạn nhận được thẻ sinh viên");

                GameManager.Instance.hasStudentCard = true;
            }
            else
            {
                Debug.Log("Bạn chưa nhặt đủ hồ sơ!");
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = true;
            Debug.Log("Nhấn E để nộp hồ sơ");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = false;
        }
    }
}