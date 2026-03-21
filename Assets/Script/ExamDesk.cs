using UnityEngine;
using UnityTutorial.PlayerController; // Gọi namespace chứa PlayerController của bạn

namespace UnityTutorial.Interactables
{
    public class ExamDesk : MonoBehaviour
    {
        [Header("UI Elements")]
        public GameObject promptUI; // Kéo thả UI Text "Nhấn E để thi" vào đây
        public GameObject quizUI;   // Kéo thả UI Panel Câu hỏi vào đây

        [Header("Âm thanh Quiz")]
        public AudioSource bgmSource; // Kéo cái loa BGM_Manager vào đây
        public AudioClip quizMusic;   // Kéo bài nhạc Quiz vào đây

        [Header("Transform Setup")]
        public Transform sitPoint;  // Kéo thả vị trí ngồi (Empty GameObject) vào đây

        private bool isUnlocked = false;
        private bool isPlayerNear = false;
        private bool isTakingExam = false;
        private GameObject playerObject;
        private PlayerController.PlayerController playerController;
        public UnityTutorial.Quiz.QuizManager quizManager;
        private UnityTutorial.Manager.InputManager inputManager;
        private bool wasInteractPressed = false; // Tránh tình trạng giữ đè nút E

        private void Start()
        {
            // Tự động tìm InputManager trong map
            inputManager = FindFirstObjectByType<UnityTutorial.Manager.InputManager>();

            // Đảm bảo UI được tắt khi bắt đầu game
            if (promptUI != null) promptUI.SetActive(false);
            if (quizUI != null) quizUI.SetActive(false);
            
        }

        private void Update()
        {
            if (inputManager == null) return;

            // Nếu đã thi ĐẬU rồi thì không cho bấm E nữa
            if (quizManager != null && quizManager.hasPassed) return;

            // Kiểm tra xem nút Interact có đang được bấm không
            bool isInteractPressed = inputManager.Interact;

            if (isUnlocked && isPlayerNear && !isTakingExam && isInteractPressed && !wasInteractPressed)
            {
                StartExam();
            }

            // Lưu lại trạng thái của nút cho khung hình (frame) tiếp theo
            wasInteractPressed = isInteractPressed;
        }

        private void OnTriggerEnter(Collider other)
        {
            // Nếu vật chạm vào là Player và không phải đang thi
            if (other.CompareTag("Player") && !isTakingExam)
            {
                // Nếu đã thi ĐẬU rồi thì không thèm hiện UI nữa, thoát luôn
                if (quizManager != null && quizManager.hasPassed) return;

                isPlayerNear = true;
                playerObject = other.gameObject;
                playerController = playerObject.GetComponent<PlayerController.PlayerController>();

                // Hiện dòng chữ "Nhấn E để thi"
                if (isUnlocked && promptUI != null) promptUI.SetActive(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerNear = false;
                playerObject = null;
                playerController = null;

                // Tắt dòng chữ khi người chơi đi ra xa
                if (promptUI != null) promptUI.SetActive(false);
            }
        }

        public void UnlockDesk()
        {
            isUnlocked = true;
            Debug.Log("Hệ thống: Bàn thi đã được mở khóa!");

            // Nếu người chơi đang đứng sẵn ở bàn thì hiện luôn chữ "Nhấn E"
            if (isPlayerNear && promptUI != null && (quizManager == null || !quizManager.hasPassed))
            {
                promptUI.SetActive(true);
            }
        }
        private void StartExam()
        {
            isTakingExam = true;
            if (bgmSource != null && quizMusic != null)
            {
                bgmSource.gameObject.SetActive(true);
                bgmSource.enabled = true;
                bgmSource.clip = quizMusic;
                bgmSource.Play();
            }
            // 1. Ẩn dòng chữ nhắc nhở
            if (promptUI != null) promptUI.SetActive(false);

            // 2. Vô hiệu hóa điều khiển để nhân vật không thể chạy loanh quanh lúc đang thi
            if (playerController != null)
            {
                playerController.enabled = false;

                // Set vận tốc về 0 để nhân vật không bị trôi đi
                Rigidbody rb = playerObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                }
            }

            // 3. Dịch chuyển nhân vật vào đúng vị trí và hướng của ghế
            if (sitPoint != null && playerObject != null)
            {
                playerObject.transform.position = sitPoint.position;
                playerObject.transform.rotation = sitPoint.rotation;

                // (Tùy chọn) Nếu bạn có Animator, có thể gọi Animation ngồi ở đây
                // playerObject.GetComponent<Animator>().SetBool("isSitting", true);
            }

            // 4. Mở giao diện bài thi (Quiz UI)
            if (quizUI != null) quizUI.SetActive(true);

            // Mở khóa chuột để người chơi có thể click chọn đáp án (nếu game của bạn đang khóa chuột)
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (quizManager != null)
            {
                quizManager.StartQuiz(this); // Chữ "this" truyền chính cái ghế này sang cho QuizManager
            }
        }

        // Bạn có thể gọi hàm này từ một nút "Nộp bài" hoặc "Thoát" trên Quiz UI
        public void FinishExam()
        {
            isTakingExam = false;
            if (bgmSource != null && bgmSource.clip == quizMusic)
            {
                bgmSource.Stop();
                // Xóa đĩa đi để lát qua phòng 2 bỏ đĩa mới vào không bị lỗi
                bgmSource.clip = null;
            }
            // Tắt UI bài thi
            if (quizUI != null) quizUI.SetActive(false);

            // Bật lại điều khiển cho người chơi
            if (playerController != null) playerController.enabled = true;

            // Khóa lại chuột nếu game của bạn là góc nhìn thứ 3/thứ 1
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Nếu thi rớt và vẫn đang đứng ở bàn thì hiện lại chữ Nhấn E
            bool canShowPrompt = true;
            if (quizManager != null && quizManager.hasPassed)
            {
                canShowPrompt = false;
            }

            if (isPlayerNear && canShowPrompt)
            {
                if (isUnlocked && promptUI != null) promptUI.SetActive(true);
            }
        }
    }
}