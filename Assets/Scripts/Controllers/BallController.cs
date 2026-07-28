using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum SpinDirection { none, up, down, left, right }
public enum GroundTypes {none, Start, Fairway, Green, Rough, Sand, Water }
[RequireComponent(typeof(Rigidbody))]
public class BallController : MonoBehaviour
{
    public static BallController Instance { get; private set; } = null;

    [Header("Physics")]
    [SerializeField] private float stopVelocityThreshold = 0.3f;
    [SerializeField] private float defaultMass = 0.04593f;
    [SerializeField] private float defaultDamping = 0.1f;
    [SerializeField] private float groundMassMultiplier = 40f;
    [SerializeField] private float groundDampingMultiplier = 6f;
    [SerializeField] private float spinMultiplier = 10f;

    [Header("Hit Settings")]
    public bool isHit;
    private bool applyHitForce; // Flag to sync Update input with FixedUpdate physics
    public int hitCount;
    public int hitHeight = 50;
    public int hitStrength = 100;
    [Range(0, 0.5f)] public float spinPower;
    public SpinDirection spinDirection;

    [Header("External References")]
    [SerializeField] WindManager windInfo;
    [SerializeField] BoxCollider windArea;

    [Header("Debug Values")]
    public Vector3 startingPos;
    public Vector3 currentPos;
    public Vector3 finalPos;
    public float ballSpeedMagnitude;
    public Vector3 ballSpeedVector;
    public bool isMoving;
    public bool isGrounded;
    public string groundValue;
    public GroundTypes groundType;

    Rigidbody rb;


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError($"Found Duplicate Wind Manager on {gameObject.name}");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    void Start()
    {
        if (windInfo == null) windInfo = WindManager.instance;
        if (windArea == null) windArea = GameObject.Find("WindArea").GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        startingPos = transform.position;
    }

    void Update()
    {
        currentPos = transform.position;
        if (Input.GetKeyDown(KeyCode.Space) && !isHit && !isMoving) PrepareHit();
        if (Input.GetKeyDown(KeyCode.R)) ResetBall();
    }
     void FixedUpdate()
    {
        ballSpeedVector = rb.linearVelocity;
        ballSpeedMagnitude = ballSpeedVector.magnitude;
        // Apply the hit force if spacebar was pressed during Update
        if (applyHitForce)
        {
            ExecuteHit();
            applyHitForce = false;
        }
        if (ballSpeedMagnitude > stopVelocityThreshold) return;
        if (groundValue == "Start") return;
        if (!isGrounded) return;
        if (!isMoving) return;
        BallStopRolling();
    }
    void ResetBall()
    {
        hitCount = 0;
        isHit = false;
        isMoving = false;
        applyHitForce = false;

        rb.mass = defaultMass;
        rb.linearDamping = defaultDamping;
        rb.Sleep();

        //finalPos = Vector3.zero;
        transform.position = startingPos;
    }
    void PrepareHit()
    {
        rb.WakeUp();
        if (windArea != null) windArea.enabled = false;
        isHit = true;
        isMoving = true;
        hitCount++;

        // Signal FixedUpdate to apply forces on the next physics step
        applyHitForce = true;
    }

    void ExecuteHit()
    {
        // Combine all force calculations into a single Vector3
        Vector3 totalForce = (Vector3.up * hitHeight) + (Vector3.forward * hitStrength);

        if (spinDirection != SpinDirection.none) totalForce += AddSpin();

        if (windInfo != null && windInfo.isWindy)
        {
            totalForce += windInfo.isRandWindy
                ? (windInfo.windRandomDirection * windInfo.windRandomPower)
                : (windInfo.windDirection * windInfo.windPower);
        }
        // Single line AddForce
        rb.AddForce(totalForce);
    }
    void BallStopRolling()
    {
        //Stops the ball from moving
        isMoving = false;
        isHit = false;
        rb.linearVelocity = Vector3.zero;
        rb.mass = defaultMass;
        rb.linearDamping = defaultDamping;
        rb.Sleep();

        finalPos = transform.position;
        if (windArea != null) windArea.enabled = true;
    }
    Vector3 AddSpin()
    {
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

    void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
        groundValue = collision.collider.tag;
        //Debug.Log($"On the Ground{ballSpeedVector}");
        //Debug.Log($"Touched {collision.collider.tag}");

        if (!Enum.IsDefined(typeof(GroundTypes), groundValue))
        {
            // error should occur
            return;
        }

        switch (Enum.Parse<GroundTypes>(groundValue))
        {
            case GroundTypes.Start:
                rb.mass = defaultMass;
                rb.linearDamping = defaultDamping;
                break;
            case GroundTypes.Fairway:
                rb.mass = defaultMass * groundMassMultiplier;
                rb.linearDamping = defaultDamping * groundDampingMultiplier;
                break;
            case GroundTypes.Rough:
                groundMassMultiplier = groundMassMultiplier * 2;
                groundDampingMultiplier = groundDampingMultiplier * 2;
                rb.mass = defaultMass * groundMassMultiplier;
                rb.linearDamping = defaultDamping * groundDampingMultiplier;
                break;
            case GroundTypes.Green:
                rb.mass = defaultMass * groundMassMultiplier;
                rb.linearDamping = defaultDamping * groundDampingMultiplier;
                break;
            case GroundTypes.Water:
                ResetBall();
                break;
            case GroundTypes.Sand:
                BallStopRolling();
                break;
            default:
                Debug.Log("Ball has not hit a valid ground type");
                break;
        }
    }
    void OnCollisionExit(Collision collision) 
    {
        isGrounded = false;
    }

}
