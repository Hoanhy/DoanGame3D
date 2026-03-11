using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

namespace UnityTutorial.Manager
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private PlayerInput playerInput;

        public Vector2 Move { get; private set; }
        public Vector2 Look { get; private set; }
        public bool Run { get; private set; }
        public bool Interact { get; private set; }
        public bool Attack { get; private set; }
        public bool Equip { get; private set; }

        private InputActionMap currentMap;
        private InputAction moveAction;
        private InputAction lookAction;
        private InputAction runAction;
        private InputAction interactAction;
        private InputAction attackAction;
        private InputAction equipAction;

        private void Awake()
        {
            currentMap = playerInput.currentActionMap;
            moveAction = currentMap.FindAction("Move");
            lookAction = currentMap.FindAction("Look");
            runAction = currentMap.FindAction("Run");
            interactAction = currentMap.FindAction("Interact");
            attackAction = currentMap.FindAction("Attack");
            equipAction = currentMap.FindAction("Equip");

            moveAction.performed += onMove;
            lookAction.performed += onLook;
            runAction.performed += onRun;
            interactAction.performed += onInteract;
            attackAction.performed += onAttack;
            equipAction.performed += onEquip;

            moveAction.canceled += onMove;
            lookAction.canceled += onLook;
            runAction.canceled += onRun;
            interactAction.canceled += onInteract;
            attackAction.canceled += onAttack;
            equipAction.canceled += onEquip;
        }

        private void onMove(InputAction.CallbackContext context)
        {
            Move = context.ReadValue<Vector2>();
        }
        private void onLook(InputAction.CallbackContext context)
        {
            Look = context.ReadValue<Vector2>();
        }
        private void onRun(InputAction.CallbackContext context)
        {
            Run = context.ReadValueAsButton();
        }
        private void onInteract(InputAction.CallbackContext context)
        {
            Interact = context.ReadValueAsButton();
        }
        private void onAttack(InputAction.CallbackContext context)
        {
            Attack = context.ReadValueAsButton();
        }
        private void onEquip(InputAction.CallbackContext context)
        {
            Equip = context.ReadValueAsButton();
        }
        private void OnEnable()
        {
            currentMap.Enable();
        }
        private void OnDisable()
        {
            currentMap.Disable();
        }
    }
}