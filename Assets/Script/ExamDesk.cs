using UnityEngine;
using UnityTutorial.PlayerController; 

namespace UnityTutorial.Interactables
{
    public class ExamDesk : MonoBehaviour
    {
        [Header("UI Elements")]
        public GameObject promptUI; 
        public GameObject quizUI;   

        [Header("Âm thanh Quiz")]
        public AudioSource bgmSource; 
        public AudioClip quizMusic;   

        [Header("Transform Setup")]
        public Transform sitPoint;  

        private bool isUnlocked = false;
        private bool isPlayerNear = false;
        private bool isTakingExam = false;
        private GameObject playerObject;
        private PlayerController.PlayerController playerController;
        public UnityTutorial.Quiz.QuizManager quizManager;
        private UnityTutorial.Manager.InputManager inputManager;
        private bool wasInteractPressed = false;

        private void Start()
        {
            // Tự động tìm InputManager trong map
            inputManager = FindFirstObjectByType<UnityTutorial.Manager.InputManager>();

            if (promptUI != null) promptUI.SetActive(false);
            if (quizUI != null) quizUI.SetActive(false);
            
        }

        private void Update()
        {
            if (inputManager == null) return;

            if (quizManager != null && quizManager.hasPassed) return;

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
                if (quizManager != null && quizManager.hasPassed) return;

                isPlayerNear = true;
                playerObject = other.gameObject;
                playerController = playerObject.GetComponent<PlayerController.PlayerController>();

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
            if (promptUI != null) promptUI.SetActive(false);

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

            //Dịch chuyển nhân vật vào đúng vị trí và hướng của ghế
            if (sitPoint != null && playerObject != null)
            {
                playerObject.transform.position = sitPoint.position;
                playerObject.transform.rotation = sitPoint.rotation;

            }

            if (quizUI != null) quizUI.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (quizManager != null)
            {
                quizManager.StartQuiz(this);
            }
        }

        public void FinishExam()
        {
            isTakingExam = false;
            if (bgmSource != null && bgmSource.clip == quizMusic)
            {
                bgmSource.Stop();
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