using System.Collections.Generic;
using UnityEngine;
using TMPro; 
using UnityTutorial.Interactables; 

namespace UnityTutorial.Quiz
{
    // Tạo cấu trúc dữ liệu cho một câu hỏi
    [System.Serializable]
    public class Question
    {
        [TextArea(2, 5)] // Giúp ô nhập text trong Unity to ra dễ nhìn hơn
        public string questionText;
        public string[] answers = new string[4]; // 4 đáp án A, B, C, D
        public int correctAnswerIndex; // Vị trí đáp án đúng (0=A, 1=B, 2=C, 3=D)
    }

    public class QuizManager : MonoBehaviour
    {
        [Header("Liên kết File khác")]
        private ExamDesk currentExamDesk; 

        [Header("Danh sách câu hỏi")]
        public List<Question> questions;

        [Header("UI Elements")]
        public TextMeshProUGUI questionTextUI; 
        public TextMeshProUGUI[] answerTextsUI; 
        public TextMeshProUGUI timerTextUI; 
        [Header("UI Kết quả")]
        public GameObject resultPanel; 
        public TextMeshProUGUI resultTextUI; 
        public TextMeshProUGUI evaluationTextUI; 

        [Header("Cài đặt")]
        public float timeLimit = 60f; 
        public int passingScore = 5; 
        public bool hasPassed = false; 

        private int currentQuestionIndex = 0;
        private int score = 0;
        private float timeRemaining;
        private bool isQuizActive = false;

        public void StartQuiz(ExamDesk desk)
        {
            currentExamDesk = desk; // Lưu lại chính xác cái ghế mà nhân vật vừa ngồi vào
            currentQuestionIndex = 0;
            score = 0;
            timeRemaining = timeLimit;
            isQuizActive = true;

            resultPanel.SetActive(false); // Tắt bảng kết quả đi
            if (questionTextUI != null)
            {
                questionTextUI.gameObject.SetActive(true);
            }

            foreach (var btnText in answerTextsUI)
            {
                if (btnText != null)
                {
                    btnText.transform.parent.gameObject.SetActive(true);
                }
            }
            DisplayQuestion(); 
        }

        private void Update()
        {
            if (isQuizActive)
            {
                // Trừ thời gian dần đi
                timeRemaining -= Time.deltaTime;

                // Hiển thị lên màn hình (Làm tròn số giây)
                timerTextUI.text = "Thời gian: " + Mathf.CeilToInt(timeRemaining).ToString() + "s";

                // Nếu hết giờ
                if (timeRemaining <= 0)
                {
                    timeRemaining = 0;
                    EndQuiz(true);
                }
            }
        }

        private void DisplayQuestion()
        {
            // Kiểm tra xem còn câu hỏi không
            if (currentQuestionIndex < questions.Count)
            {
                Question q = questions[currentQuestionIndex];
                questionTextUI.text = q.questionText; 

                // Đổi chữ cho 4 nút đáp án
                for (int i = 0; i < answerTextsUI.Length; i++)
                {
                    if (i < q.answers.Length)
                    {
                        answerTextsUI[i].text = q.answers[i];
                    }
                }
            }
            else
            {
                EndQuiz(false); 
            }
        }

        public void SelectAnswer(int answerIndex)
        {
            if (!isQuizActive) return;

            Question currentQuestion = questions[currentQuestionIndex];

            // Nếu chọn đúng đáp án
            if (answerIndex == currentQuestion.correctAnswerIndex)
            {
                score++;
                Debug.Log("Trả lời đúng!");
            }
            else
            {
                Debug.Log("Trả lời sai!");
            }

            // Chuyển sang câu tiếp theo
            currentQuestionIndex++;
            DisplayQuestion();
        }

        private void EndQuiz(bool isTimeOut)
        {
            isQuizActive = false;

            // 1. Tắt câu hỏi và đáp án
            questionTextUI.gameObject.SetActive(false);
            foreach (var btnText in answerTextsUI)
            {
                btnText.transform.parent.gameObject.SetActive(false);
            }

            // 2. Bật bảng kết quả
            resultPanel.SetActive(true);

            // 3. CHỈ HIỂN THỊ SỐ CÂU ĐÚNG (Đã bỏ phần Hết giờ/Hoàn thành)
            resultTextUI.text = "Số điểm đạt được: " + score + " Điểm";

            // 4. KIỂM TRA ĐẠT HAY KHÔNG ĐẠT
            if (evaluationTextUI != null)
            {
                if (score >= passingScore)
                {
                    evaluationTextUI.text = "ĐÁNH GIÁ: ĐẠT";
                    evaluationTextUI.color = Color.green;
                    hasPassed = true;
                }
                else
                {
                    evaluationTextUI.text = "ĐÁNH GIÁ: KHÔNG ĐẠT";
                    evaluationTextUI.color = Color.red; // Đổi chữ thành màu đỏ
                    hasPassed = false; // Thi rớt, cho phép làm lại
                }
            }
        }

        public void CloseQuiz()
        {
            if (resultPanel != null) resultPanel.SetActive(false);
            gameObject.SetActive(false);

            // Báo cho ĐÚNG cái ghế đang ngồi thả nhân vật ra
            if (currentExamDesk != null)
            {
                currentExamDesk.FinishExam();
            }
        }
    }
}