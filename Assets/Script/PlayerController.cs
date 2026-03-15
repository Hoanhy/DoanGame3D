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
        private Animator animator;

        private const float walkSpeed = 5f;
        private const float runSpeed = 10f;
        private const float rotationSpeed = 10f;

        // ===== PLAYER HEALTH =====
        [Header("Player Health")]
        public float maxHP = 100f;
        public float currentHP;
        private bool isDead = false;

        [Header("Combat System")]
        public GameObject weaponModel; // Mô hình cây bút
        public float attackDamage = 20f; // Lực chém
        public float attackCooldown = 1f; // Chém 1s / nhát
        public Vector3 attackArea = new Vector3(1f, 1f, 1.5f); // Tầm xa
        public Transform attackPoint; // Tâm chém
        public LayerMask enemyLayer; // Lớp quái vật
        private bool isArmed = false;
        private bool isAttacking = false;
        private float lastAttackTime = 0f;
        private bool wasEquipPressed = false;

        [Header("UI & Effects")]
        public UnityEngine.UI.Slider healthBarSlider;

        public void Start()
        {
            playerRigidbody = GetComponent<Rigidbody>();
            inputManager = FindFirstObjectByType<InputManager>();
            animator = GetComponent<Animator>();
            currentHP = maxHP;
            // Khóa chuột vào giữa màn hình
            Cursor.lockState = CursorLockMode.Locked;
            // Tàng hình con chuột
            Cursor.visible = false;
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

        public void Update()
        {
            if (isDead) return;

            // XỬ LÝ RÚT/CẤT VŨ KHÍ (Nút Equip)
            bool isEquipPressed = inputManager.Equip;
            if (isEquipPressed && !wasEquipPressed && !isAttacking)
            {
                ToggleWeapon();
            }
            wasEquipPressed = isEquipPressed;

            // XỬ LÝ TẤN CÔNG (Nút Attack)
            if (inputManager.Attack && isArmed && !isAttacking && Time.time >= lastAttackTime + attackCooldown)
            {
                PerformAttack();
            }
        }

        public void FixedUpdate()
        {
            if (isDead || isAttacking) return;
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

            if (animator != null)
            {
                animator.SetFloat("Speed", targetSpeed);
            }

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

        private void ToggleWeapon()
        {
            isArmed = !isArmed;

            // Bật/tắt mô hình cây bút trên tay
            if (weaponModel != null) weaponModel.SetActive(isArmed);

            // Kích hoạt tư thế chiến đấu trong Animator
            if (animator != null) animator.SetBool("IsArmed", isArmed);

            Debug.Log(isArmed ? "Đã rút bút!" : "Đã cất bút!");
        }

        private void PerformAttack()
        {
            isAttacking = true;
            lastAttackTime = Time.time;

            // Phanh gấp nhân vật lại khi vung vũ khí
            playerRigidbody.linearVelocity = Vector3.zero;

            // Phát hoạt ảnh chém
            if (animator != null) animator.SetTrigger("Attack");

            // Quét vùng sát thương
            if (attackPoint != null)
            {
                Collider[] hitEnemies = Physics.OverlapBox(attackPoint.position, attackArea / 2f, attackPoint.rotation, enemyLayer);
                foreach (Collider enemy in hitEnemies)
                {
                    Debug.Log("Chém trúng: " + enemy.name);
                    // Sau này bạn gọi code trừ máu quái vật ở đây
                    // Tìm script EnemyBase gắn trên con quái vật vừa chém trúng
                    EnemyBase enemyStats = enemy.GetComponent<EnemyBase>();

                    // Nếu tìm thấy script, gọi hàm TakeDamage và truyền lực chém vào
                    if (enemyStats != null)
                    {
                        enemyStats.TakeDamage(attackDamage);
                    }
                }
            }

            // Kết thúc nhát chém sau 0.5 giây (Hoặc chỉnh lại cho khớp độ dài Animation)
            Invoke(nameof(ResetAttack), 0.5f);
        }

        private void ResetAttack()
        {
            isAttacking = false;
        }

        // Vẽ vòng tròn đỏ để căn chỉnh tầm đánh trong cửa sổ Scene
        private void OnDrawGizmosSelected()
        {
            if (attackPoint == null) return;
            Gizmos.color = Color.red;

            // Xoay cái khung vẽ Gizmos theo hướng của AttackPoint
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(attackPoint.position, attackPoint.rotation, Vector3.one);
            Gizmos.matrix = rotationMatrix;

            // Vẽ hình hộp chữ nhật
            Gizmos.DrawWireCube(Vector3.zero, attackArea);
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
            if (animator != null)
            {
                animator.SetTrigger("Die");
            }
            // FindFirstObjectByType<GameManager>().ShowGameOverScreen();
        }
    }
}