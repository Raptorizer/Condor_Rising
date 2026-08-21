using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent (typeof(Rigidbody))]
public class BallControllerTest : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("Turn speed in degrees per second")]
    [SerializeField] float turnSpeed = 90f;

    [Header("Movement Settings")]
    [SerializeField] float moveForce = 15f;
    [SerializeField] float maxVelocity = 20f;
    [SerializeField] Transform referenceTransform; //camera

    Rigidbody rb;
    float currentYaw;
    float rotationInput;
    float thrustInput;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = 50f; //smooth rolling at high speeds
        currentYaw = transform.eulerAngles.y;
    }
    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;
        rotationInput = 0f;
        if (keyboard.aKey.isPressed) rotationInput -= 1f;
        if (keyboard.dKey.isPressed) rotationInput += 1f;

        thrustInput = 0f;
        if(keyboard.wKey.isPressed) thrustInput += 1f;
        if(keyboard.sKey.isPressed)thrustInput -= 1f;

        currentYaw += rotationInput * turnSpeed * Time.deltaTime;
        currentYaw = Mathf.Repeat(currentYaw, 360f);
    }
    private void FixedUpdate()
    {
        Vector3 yForward = GetHeadingForward();
        if(Mathf.Abs(thrustInput) > 0.01f && rb.linearVelocity.magnitude <maxVelocity) rb.AddForce(yForward * (thrustInput * moveForce), ForceMode.Acceleration);
    }
    public Vector3 GetHeadingForward()
    {
        return Quaternion.Euler(0f,currentYaw, 0f) * Vector3.forward;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Vector3 forward = GetHeadingForward();
        Gizmos.DrawRay(transform.position, forward * 2f);
    }
}
