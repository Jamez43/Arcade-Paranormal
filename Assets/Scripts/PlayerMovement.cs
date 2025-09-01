using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;

    private InputAction moveAction;

    private Vector2 moveAmount;
    [SerializeField] private float movementSpeed = 2f;

    private void OnEnable()
    {
        inputActions.FindActionMap("Gameplay").Enable();
    }

    private void OnDisable()
    {
        inputActions.FindActionMap("Gameplay").Disable();
    }

    private void Awake()
    {
        moveAction = InputSystem.actions.FindAction("Move");
    }

    private void Update()
    {
        moveAmount = moveAction.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        transform.position += new Vector3(moveAmount.x, moveAmount.y, 0) * movementSpeed * Time.fixedDeltaTime;
    }

}
