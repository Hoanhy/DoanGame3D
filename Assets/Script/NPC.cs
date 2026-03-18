using UnityEngine;
using UnityTutorial.Manager;
using TMPro; // Thư viện dùng cho TextMeshPro
using UnityTutorial.Interactables; // Để gọi script ExamDesk của bạn
using UnityTutorial.PlayerController; // Để khóa người chơi lúc nói chuyện
using UnityEngine.Events;
using UnityTutorial.Quiz; // Gọi thư viện Quiz để đọc kết quả

public class TeacherNPC : MonoBehaviour
{
    [Header("References")]
    public GameObject interactUI; // Chữ "Nhấn E để nói chuyện với Thầy"

    [Header("UI Hội thoại (Dialogue)")]
    public GameObject dialoguePanel; // Kéo Panel nền đen mờ vào đây
    public TextMeshProUGUI nameText; // Kéo Text chứa tên NPC vào đây
    public TextMeshProUGUI dialogueText; // Kéo Text chứa nội dung vào đây
    public string npcName = "Giám Thị";

    [Header("Cinematic Camera")]
    public GameObject dialogueCamera; // Kéo cái Camera góc cận vào đây

    [Header("KỊCH BẢN 1: TRƯỚC KHI THI")]
    [TextArea(2, 4)] // Giúp ô nhập chữ trong Unity to ra dễ viết hơn
    public string[] sentences; // Danh sách các câu thầy sẽ nói lần lượt

    [Header("KỊCH BẢN 2: SAU KHI THI ĐẬU")]
    public QuizManager quizManager; // Kéo file QuizManager vào đây để NPC kiểm tra điểm
    public bool isCombatCleared = false;
    [TextArea(2, 4)] public string[] sentencesPhase2;
    public UnityEvent onDialogueEndPhase2; // Cắm sự kiện dịch chuyển vào đây

    [Header("Hành động sau khi nói chuyện xong")]
    public UnityEvent onDialogueEnd;

    private int currentSentenceIndex;
    private bool isTalking = false;
    private bool isPlayerInRange = false;
    private InputManager inputManager;
    private bool wasInteractPressed = false;
    private PlayerController player; // Lưu trữ script người chơi
    private string[] currentSentences;
    private UnityEvent currentEvent;
    private bool isEndingDialogue = false;

    void Start()
    {
        inputManager = FindFirstObjectByType<InputManager>();
        if (interactUI != null) interactUI.SetActive(false);
        if (dialoguePanel != null) dialoguePanel.SetActive(false); // Ẩn khung chat lúc mới vào game
        if (dialogueCamera != null) dialogueCamera.SetActive(false); // Đảm bảo camera hội thoại bị tắt lúc mới vào game
    }

    void Update()
    {
        if (inputManager == null) return;
        bool isInteractPressed = inputManager.Interact;

        // Bấm E giờ để bắt đầu nói chuyện
        if (isInteractPressed && !wasInteractPressed && !isTalking && isPlayerInRange && this.enabled)
        {
            StartDialogue();
        }
        // Tự động lắng nghe click chuột
        if (isTalking && Input.GetMouseButtonDown(0))
        {
            NextSentence();
        }

        wasInteractPressed = isInteractPressed;
    }
    public void SetCombatCleared()
    {
        isCombatCleared = true;
    }

    private void StartDialogue()
    {
        isTalking = true;
        currentSentenceIndex = 0;

        if (interactUI != null) interactUI.SetActive(false); // Tắt chữ "Nhấn E"
        if (dialoguePanel != null) dialoguePanel.SetActive(true); // Bật khung chat lên
        if (dialogueCamera != null) dialogueCamera.SetActive(true); // Bật camera hội thoại lên

        if (nameText != null) nameText.text = npcName; // Gán tên Thầy Giáo

        bool passed = false;

        if (Level1Manager.Instance != null)
        {
            // Nếu đang ở Màn 1: Kiểm tra xem đã nhặt đủ hồ sơ chưa
            passed = Level1Manager.Instance.readyToSubmit;
        }
        else if (Level2Manager.Instance != null)
        {
            // Nếu đang ở Màn 2: Kiểm tra thi đậu hoặc đánh quái xong
            if (quizManager != null && quizManager.hasPassed) passed = true;
            if (isCombatCleared) passed = true;
        }
        else if (Level3Manager.Instance != null)
        {
            // Nếu đang ở Màn 3: Kiểm tra bảo vệ đồ án (đã gọi SetCombatCleared)
            passed = isCombatCleared;
        }
        else
        {
            // Phòng hờ các màn khác
            passed = (quizManager != null && quizManager.hasPassed) || isCombatCleared;
        }

        // Chọn kịch bản tương ứng
        if (passed && sentencesPhase2.Length > 0)
        {
            currentSentences = sentencesPhase2;
            currentEvent = onDialogueEndPhase2;
        }
        else
        {
            currentSentences = sentences;
            currentEvent = onDialogueEnd;
        }

        // Khóa nhân vật
        if (player != null)
        {
            Animator playerAnim = player.GetComponent<Animator>();
            if (playerAnim != null) playerAnim.SetFloat("Speed", 0f);

            player.enabled = false;
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null) rb.linearVelocity = Vector3.zero;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        NextSentence();
    }

    public void NextSentence()
    {
        if (!isTalking || isEndingDialogue) return; // Nếu không phải đang nói chuyện thì click không có tác dụng

        if (currentSentenceIndex < currentSentences.Length)
        {
            if (dialogueText != null)
            {
                dialogueText.text = currentSentences[currentSentenceIndex];
            }
            currentSentenceIndex++;
        }
        else
        {
            isEndingDialogue = true;
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        isTalking = false;
        if (dialoguePanel != null) dialoguePanel.SetActive(false); // Tắt khung chat
        if (dialogueCamera != null) dialogueCamera.SetActive(false); // Tắt camera hội thoại

        if (currentEvent == onDialogueEnd)
        {
            if (player != null) player.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // Gọi sự kiện (Mở bàn thi hoặc Dịch chuyển tùy thuộc vào kịch bản)
        if (currentEvent != null) currentEvent.Invoke();

        // Chỉ tắt NPC nghỉ hưu nếu đã chạy xong kịch bản 2 (thi đậu)
        if (quizManager != null && quizManager.hasPassed)
        {
            this.enabled = false;
        }
        Invoke("ResetEndingLock", 2f);
    }
    private void ResetEndingLock()
    {
        isEndingDialogue = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && this.enabled)
        {
            isPlayerInRange = true;
            player = other.GetComponent<PlayerController>();
            if (interactUI != null && !isTalking) interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            player = null;
            if (interactUI != null) interactUI.SetActive(false);
        }
    }
}