using UnityEngine;

public class pinballRotate : MonoBehaviour
{

    private Rigidbody2D rb;
    private void FixedUpdate()
    {
        float speed = rb.linearVelocity.magnitude;
        float radius = GetComponent<CircleCollider2D>().radius * transform.localScale.x;
        bool invertSpin = false;
        // Pure rolling: ω(rad/s) = v / r ; convert to deg/s
        float wRad = speed / radius;
        float wDeg = wRad * Mathf.Rad2Deg;

        // Sign: In 2D top-down or side view, choose sign based on travel direction.
        // We'll use cross of forward (Z+) with velocity to determine left/right.
        float signed = Mathf.Sign(Vector3.Cross(Vector3.forward, new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, 0f)).z);
        if (invertSpin) signed = -signed;
        rb.angularVelocity = wDeg * signed;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
}
