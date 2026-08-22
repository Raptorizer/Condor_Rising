using System;
using UnityEngine;

public enum SpinDirection { none, up, down, left, right }
public enum GroundTypes { none, Start, Fairway, Green, Rough, Sand, Water }

[RequireComponent(typeof(Rigidbody))]
public class BallController : MonoBehaviour
{
    public static BallController Instance { get; private set; }

    [Header("Physics")]
    [SerializeField] private float stopVelocityThreshold = 0.3f;
    [SerializeField] private float spinMultiplier = 10f;

    [Header("Hit Settings")]
    public bool isHit;
    public int hitCount;
    [Range(0, 0.5f)] public float spinPower;
    public SpinDirection spinDirection;

    [Header("External References")]
    [SerializeField] WindManager windInfo;
    [SerializeField] BoxCollider windArea;

    [Header("State")]
    public bool isMoving;
    public bool isGrounded;
    public Rigidbody rb;

    private Vector3 startPosition;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        if (windInfo == null) windInfo = WindManager.instance;
        if (windArea == null) windArea = GameObject.Find("WindArea")?.GetComponent<BoxCollider>();
        if (rb == null) rb = GetComponent<Rigidbody>();

        startPosition = transform.position;
    }

    void FixedUpdate()
    {
        // One-line check to stop the ball
        if (!isMoving || !isGrounded || rb.linearVelocity.magnitude > stopVelocityThreshold) return;

        BallStopRolling();
    }

    public void ResetBall()
    {
        hitCount = 0;
        isHit = false;
        isMoving = false;

        // Zero out angular velocity as well to prevent residual spinning
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.Sleep();

        transform.position = startPosition;
    }

    public void ExecuteHit(Vector3 baseAimingForce)
    {
        isHit = true;
        isMoving = true;
        hitCount++;

        rb.WakeUp();
        if (windArea != null) windArea.enabled = false;

        Vector3 finalAppliedForce = baseAimingForce + GetSpinForce() + GetWindForce();
        rb.AddForce(finalAppliedForce);
    }

    private Vector3 GetSpinForce()
    {
        if (spinDirection == SpinDirection.none) return Vector3.zero;

        float appliedSpin = spinPower * spinMultiplier;

        return spinDirection switch
        {
            SpinDirection.up => Vector3.up * appliedSpin,
            SpinDirection.down => Vector3.down * appliedSpin,
            SpinDirection.left => Vector3.left * appliedSpin,
            SpinDirection.right => Vector3.right * appliedSpin,
            _ => Vector3.zero,
        };
    }

    private Vector3 GetWindForce()
    {
        if (windInfo == null || !windInfo.isWindy) return Vector3.zero;

        return windInfo.isRandWindy
            ? (windInfo.windRandomDirection * windInfo.windRandomPower)
            : (windInfo.windDirection * windInfo.windPower);
    }

    void BallStopRolling()
    {
        isMoving = false;
        isHit = false;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.Sleep();

        if (windArea != null) windArea.enabled = true;

        // TELL THE PLAYER CONTROLLER TO TURN THE LINE BACK ON
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.EnableAimLine();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;

        // TryParse is much more efficient than using Enum.IsDefined and Enum.Parse separately
        if (!Enum.TryParse(collision.collider.tag, out GroundTypes groundType)) return;

        switch (groundType)
        {
            case GroundTypes.Start:
                break;
            case GroundTypes.Fairway:
            case GroundTypes.Green: // Stacked cases since Fairway and Green use the exact same math
                break;
            case GroundTypes.Rough:
                break;
            case GroundTypes.Water:
                ResetBall();
                break;
            case GroundTypes.Sand:
                break;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }
}