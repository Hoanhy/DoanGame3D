using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTutorial.Manager;

namespace UnityTutorial.PlayerController
{
    public class PlayerController : MonoBehaviour
    {
        private Rigidbody playerRigidbody;
        private InputManager inputManager;
        private Transform cameraTransform;

        private const float walkSpeed = 5f;
        private const float runSpeed = 10f;
        private const float rotationSpeed = 10f;

        // ===== PLAYER HEALTH =====
        [Header("Player Health")]
        public float maxHP = 100f;
        public float currentHP;

        [Header("UI & Effects")]
        public UnityEngine.UI.Slider healthBarSlider;

        private bool isDead = false;

        public void Start()
        {
            playerRigidbody = GetComponent<Rigidbody>();
            inputManager = FindFirstObjectByType<InputManager>();
            currentHP = maxHP;
            UpdateHealthUI(); // Gọi hàm cập nhật UI

            // Tìm Camera chính trong scene
            if (Camera.main != null)
            {
                cameraTransform = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning("Không tìm thấy Camera có tag 'MainCamera'.");
            }
        }


        public void FixedUpdate()
        {
            // 1. Tính toán hướng di chuyển dựa trên Camera trước
            Vector3 moveDirection = GetCameraRelativeMovementDirection();

            // 2. Truyền hướng đó vào hàm Move và hàm HandleRotation
            Move(moveDirection);
            HandleRotation(moveDirection);
        }

        private Vector3 GetCameraRelativeMovementDirection()
        {
            if (cameraTransform == null || inputManager.Move == Vector2.zero)
                return Vector3.zero;

            // Lấy vector chỉ hướng trước mặt và bên phải của Camera
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;

            // Bỏ qua trục Y (làm phẳng hướng chiếu xuống mặt đất) để nhân vật không bay lên hay chui xuống đất
            camForward.y = 0f;
            camRight.y = 0f;

            camForward.Normalize();
            camRight.Normalize();

            // Kết hợp Input với hướng của Camera
            // Nút W/S (Move.y) sẽ đi theo hướng camForward
            // Nút A/D (Move.x) sẽ đi theo hướng camRight
            Vector3 direction = camForward * inputManager.Move.y + camRight * inputManager.Move.x;

            // Tránh việc đi chéo bị nhanh hơn đi thẳng
            if (direction.magnitude > 1f)
            {
                direction.Normalize();
            }

            return direction;
        }
        private void Move(Vector3 moveDirection)
        {
            float targetSpeed = inputManager.Run ? runSpeed : walkSpeed;
            if (inputManager.Move == Vector2.zero) targetSpeed = 0f;

            // Tính toán vận tốc mục tiêu dựa trên hướng Camera đã tính
            Vector3 targetVelocity = moveDirection * targetSpeed;

            // Tính toán sự chênh lệch vận tốc
            float xVelDifference = targetVelocity.x - playerRigidbody.linearVelocity.x;
            float zVelDifference = targetVelocity.z - playerRigidbody.linearVelocity.z;

            // Áp dụng lực vào Rigidbody
            playerRigidbody.AddForce(new Vector3(xVelDifference, 0f, zVelDifference), ForceMode.VelocityChange);
        }

        private void HandleRotation(Vector3 moveDirection)
        {
            // Chỉ xoay khi có vector hướng di chuyển
            if (moveDirection != Vector3.zero)
            {
                // Tính toán góc nhìn hướng về phía moveDirection
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);

                // Nội suy xoay mượt mà
                playerRigidbody.MoveRotation(Quaternion.Slerp(playerRigidbody.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
            }
        }

        // ===== DAMAGE SYSTEM =====
        public void TakeDamage(float damage)
        {
            if (isDead) return; // Nếu chết rồi thì không nhận sát thương nữa

            currentHP -= damage;

            // Đảm bảo máu không bị âm
            if (currentHP < 0) currentHP = 0;

            Debug.Log("Player bị đánh! Máu còn: " + currentHP);
            UpdateHealthUI();

            if (currentHP <= 0)
            {
                Die();
            }
        }
        private void UpdateHealthUI()
        {
            // Nếu bạn có Slider thanh máu, cập nhật nó ở đây
            // if (healthBarSlider != null) 
            // {
            //     healthBarSlider.value = currentHP / maxHP; 
            // }
        }
        private void Die()
        {
            if (isDead) return;
            isDead = true;
            Debug.Log("Player Dead!");

            // 1. Tắt script di chuyển để không điều khiển được nữa
            this.enabled = false;

            // 2. Tắt va chạm để quái vật không đánh xác chết
            GetComponent<Collider>().enabled = false;
            if (playerRigidbody != null) playerRigidbody.isKinematic = true;

            // 3. Chỗ này sau này bạn gọi Animation ngã xuống hoặc hiện UI Game Over
            // GetComponent<Animator>().SetTrigger("Die");
            // FindFirstObjectByType<GameManager>().ShowGameOverScreen();
        }
    }
}