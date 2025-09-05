using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private PlayerStats stats;

    private InputAction moveAction;

    private Vector2 moveAmount;
    private float currentSpeed;

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
        currentSpeed = stats.Speed;
    }

    private void Update()
    {
        moveAmount = moveAction.ReadValue<Vector2>();
        if (playerCollider.IsTouchingLayers(LayerMask.GetMask("Enemy")))
        {
            currentSpeed = stats.Speed * 0.5f;
        }
        else
        {
            currentSpeed = stats.Speed;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        transform.position += new Vector3(moveAmount.x, moveAmount.y, 0) * currentSpeed * Time.fixedDeltaTime;
    }

}
