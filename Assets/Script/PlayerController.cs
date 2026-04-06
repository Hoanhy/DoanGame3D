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

        [Header("Cài đặt Di chuyển")]
        public float walkSpeed = 7f;
        public float runSpeed = 14f;
        public float rotationSpeed = 10f;

        [Header("Player Health")]
        public float maxHP = 100f;
        public float currentHP;
        private bool isDead = false;

        [Header("Player HP UI")]
        public PlayerHPBar hpBar;

        [Header("Combat System")]
        public GameObject weaponModel;
        public float attackDamage = 20f;
        public float attackCooldown = 1f;
        public Vector3 attackArea = new Vector3(1f, 1f, 1.5f);
        public Transform attackPoint;
        public LayerMask enemyLayer;

        private bool isArmed = false;
        private bool isAttacking = false;
        private float lastAttackTime = 0f;
        private bool wasEquipPressed = false;

        public void Start()
        {
            playerRigidbody = GetComponent<Rigidbody>();
            inputManager = FindFirstObjectByType<InputManager>();
            animator = GetComponent<Animator>();

            currentHP = maxHP;

            if (hpBar != null)
            {
                hpBar.SetMaxHP(maxHP);
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (Camera.main != null)
            {
                cameraTransform = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning("Không tìm thấy Camera có tag MainCamera.");
            }
        }

        public void Update()
        {
            if (isDead) return;

            bool isEquipPressed = inputManager.Equip;
            if (isEquipPressed && !wasEquipPressed && !isAttacking)
            {
                ToggleWeapon();
            }
            wasEquipPressed = isEquipPressed;

            if (inputManager.Attack && isArmed && !isAttacking && Time.time >= lastAttackTime + attackCooldown)
            {
                PerformAttack();
            }
        }

        public void FixedUpdate()
        {
            if (isDead || isAttacking) return;

            Vector3 moveDirection = GetCameraRelativeMovementDirection();
            Move(moveDirection);
            HandleRotation(moveDirection);
        }

        private Vector3 GetCameraRelativeMovementDirection()
        {
            if (cameraTransform == null || inputManager.Move == Vector2.zero)
                return Vector3.zero;

            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;

            camForward.y = 0f;
            camRight.y = 0f;

            camForward.Normalize();
            camRight.Normalize();

            Vector3 direction = camForward * inputManager.Move.y + camRight * inputManager.Move.x;

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
                // Tạo một biến riêng chỉ dùng cho Animation
                float animationSpeedValue = 0f;

                if (inputManager.Move != Vector2.zero)
                {
                    // Nếu đang đi: Bấm Shift thì gửi 1 (Chạy), Không bấm thì gửi 0.5 (Đi bộ)
                    animationSpeedValue = inputManager.Run ? 1f : 0.5f;
                }

                // Có thể dùng thêm Mathf.Lerp ở đây nếu muốn nhân vật chuyển từ đi sang chạy mượt hơn, 
                // nhưng tạm thời cứ gán trực tiếp để sửa lỗi đè animation đã.
                animator.SetFloat("Speed", animationSpeedValue);
            }

            Vector3 targetVelocity = moveDirection * targetSpeed;

            float xVelDifference = targetVelocity.x - playerRigidbody.linearVelocity.x;
            float zVelDifference = targetVelocity.z - playerRigidbody.linearVelocity.z;

            playerRigidbody.AddForce(new Vector3(xVelDifference, 0f, zVelDifference), ForceMode.VelocityChange);
        }

        private void HandleRotation(Vector3 moveDirection)
        {
            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);

                playerRigidbody.MoveRotation(
                    Quaternion.Slerp(
                        playerRigidbody.rotation,
                        targetRotation,
                        rotationSpeed * Time.fixedDeltaTime
                    )
                );
            }
        }

        private void ToggleWeapon()
        {
            isArmed = !isArmed;

            if (weaponModel != null)
                weaponModel.SetActive(isArmed);

            if (animator != null)
                animator.SetBool("IsArmed", isArmed);

            Debug.Log(isArmed ? "Đã rút bút!" : "Đã cất bút!");
        }

        private void PerformAttack()
        {
            isAttacking = true;
            lastAttackTime = Time.time;

            playerRigidbody.linearVelocity = Vector3.zero;

            if (animator != null)
                animator.SetTrigger("Attack");

            if (attackPoint != null)
            {
                Collider[] hitEnemies = Physics.OverlapBox(
                    attackPoint.position,
                    attackArea / 2f,
                    attackPoint.rotation,
                    enemyLayer
                );

                foreach (Collider enemy in hitEnemies)
                {
                    EnemyBase enemyStats = enemy.GetComponent<EnemyBase>();

                    if (enemyStats != null)
                    {
                        enemyStats.TakeDamage(attackDamage);
                    }
                }
            }

            Invoke(nameof(ResetAttack), 0.5f);
        }

        private void ResetAttack()
        {
            isAttacking = false;
        }

        private void OnDrawGizmosSelected()
        {
            if (attackPoint == null) return;

            Gizmos.color = Color.red;

            Matrix4x4 rotationMatrix =
                Matrix4x4.TRS(attackPoint.position, attackPoint.rotation, Vector3.one);

            Gizmos.matrix = rotationMatrix;
            Gizmos.DrawWireCube(Vector3.zero, attackArea);
        }

        // ===== DAMAGE SYSTEM =====
        public void TakeDamage(float damage)
        {
            if (isDead) return;

            currentHP -= damage;
            currentHP = Mathf.Clamp(currentHP, 0, maxHP);

            if (hpBar != null)
            {
                hpBar.SetHP(currentHP);
            }

            Debug.Log("Player bị đánh! Máu còn: " + currentHP);

            if (currentHP <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            if (isDead) return;

            isDead = true;
            Debug.Log("Player Dead!");

            this.enabled = false;
            GetComponent<Collider>().enabled = false;

            if (playerRigidbody != null)
                playerRigidbody.isKinematic = true;

            if (animator != null)
                animator.SetTrigger("Die");
        }

        public void TriggerGameOverUI()
        {
            if (Level3Manager.Instance != null)
            {
                Level3Manager.Instance.PlayerDied();
            }
            else
            {
                BaseGameManager currentManager = FindFirstObjectByType<BaseGameManager>();

                if (currentManager != null)
                {
                    currentManager.GameOver();
                }
            }
        }
    }
}