using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;

    private InputAction moveAction;

    private Vector2 moveAmount;
    [SerializeField] private float movementSpeed = 3f;
    private float baseSpeed;

    private Collider2D playerCollider;

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
        playerCollider = GetComponent<Collider2D>();
        baseSpeed = movementSpeed;
    }

    private void Update()
    {
        moveAmount = moveAction.ReadValue<Vector2>();
        if (playerCollider.IsTouchingLayers(LayerMask.GetMask("Enemy")))
        {
            movementSpeed = baseSpeed * 0.5f;
        }
        else
        {
            movementSpeed = baseSpeed;
        }
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
