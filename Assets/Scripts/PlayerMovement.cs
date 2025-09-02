using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private PlayerInGameStats_Default stats;

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
        currentSpeed = stats.speed;
    }

    private void Update()
    {
        moveAmount = moveAction.ReadValue<Vector2>();
        if (playerCollider.IsTouchingLayers(LayerMask.GetMask("Enemy")))
        {
            currentSpeed = stats.speed * 0.5f;
        }
        else
        {
            currentSpeed = stats.speed;
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
