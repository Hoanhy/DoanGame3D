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
        private Vector2 currentVelocity;
        private const float walkSpeed = 2f;
        private const float runSpeed = 5f;

        public void Start()
        {
            playerRigidbody = GetComponent<Rigidbody>();
            inputManager = FindFirstObjectByType<InputManager>();
        }

        public void FixedUpdate()
        {
            Move();
        }
        private void Move()
        {
            float targetSpeed = inputManager.Run ? walkSpeed : runSpeed;
            if (inputManager.Move == Vector2.zero) targetSpeed = 0.1f;

            currentVelocity.x = targetSpeed * inputManager.Move.x;
            currentVelocity.y = targetSpeed * inputManager.Move.y;

            var xVelDifference = currentVelocity.x - playerRigidbody.linearVelocity.x;
            var zVelDifference = currentVelocity.y - playerRigidbody.linearVelocity.z;

            playerRigidbody.AddForce(transform.TransformVector(new Vector3(xVelDifference, 0f, zVelDifference)), ForceMode.VelocityChange);
        }
    }
}