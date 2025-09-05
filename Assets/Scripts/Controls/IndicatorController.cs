using UnityEngine;
using UnityEngine.InputSystem; // Needed for the new Input System

public class IndicatorController : MonoBehaviour
{
    [SerializeField] private float radius = 1.5f; // Drag the player here in Inspector
    [SerializeField] private InputActionReference moveAction; // Reference to your Vector2 Input Action
    private Transform selfTransform; // Drag the gold indicator here in Inspector

    private void Awake()
    {
        selfTransform = GetComponent<Transform>();
    }

    private void Update()
    {
        // Read the joystick input (Vector2)
        Vector2 input = moveAction.action.ReadValue<Vector2>().normalized;

        // If input is not zero, rotate indicator
        if (input != Vector2.zero)

        {

            // Get the angle from joystick vector
            float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;

            Transform center = selfTransform.parent;

            Vector3 targetPos = (Vector3)(input * radius) + new Vector3(center.position.x, center.position.y, selfTransform.position.z);

            // Apply rotation (z-axis because it's 2D)
            selfTransform.rotation = Quaternion.Euler(0, 0, angle - 90f);
            selfTransform.position = targetPos;
            // subtract 90 if your sprite points "up" by default
        }
    }
}
