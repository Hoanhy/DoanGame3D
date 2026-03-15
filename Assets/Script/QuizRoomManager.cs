using UnityEngine;
using UnityTutorial.Interactables; // Để gọi ExamDesk

public class QuizRoomManager : MonoBehaviour
{
    // Hàm này sẽ tự động tìm tất cả bàn thi và mở khóa
    public void UnlockAllDesks()
    {
        ExamDesk[] allDesks = Object.FindObjectsByType<ExamDesk>(FindObjectsSortMode.None);
        foreach (ExamDesk desk in allDesks)
        {
            desk.UnlockDesk();
        }
        Debug.Log("Hệ thống: Đã mở khóa thành công " + allDesks.Length + " bàn thi!");
    }
}