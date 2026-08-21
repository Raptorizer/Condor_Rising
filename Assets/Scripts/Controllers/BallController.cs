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
    [SerializeField]  float stopVelocityThreshold = 0.3f;
    [SerializeField]  float spinMultiplier = 10f;

    [Header("Hit Settings")]
    //150 height & 135 strength = 174m/190y Beginner
    //150 height & 155 strength = 200m/220y Average
    //150 height & 174 strength = 228m/250y Good
    //150 height & 203 strength = 270m/296y PGA tour
    public bool isHit;
    bool applyHitForce; // Flag to sync Update input with FixedUpdate physics
    public int hitCount;
    public int hitHeight = 50; 
    public int hitStrength = 100;
    [Range(0, 0.5f)] public float spinPower;
    public SpinDirection spinDirection;
    public Vector3 totalForce;
    [Header("External References")]
    [SerializeField] WindManager windInfo;
    [SerializeField] BoxCollider windArea;
    [SerializeField] GameObject fauxball;

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

    public Rigidbody rb;

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
        if (fauxball == null) fauxball = GameObject.Find("Fauxball");
        if (rb == null) rb = GetComponent<Rigidbody>();
        startingPos = transform.position;
    }

    void Update()
    {
        currentPos = transform.position;
    }
     void FixedUpdate()
    {
        ballSpeedVector = rb.linearVelocity;
        ballSpeedMagnitude = ballSpeedVector.magnitude;
        totalForce = (Vector3.up * hitHeight) + (Vector3.forward * hitStrength) + (PlayerController.Instance.rotationSpeed * transform.localRotation.y * Vector3.right);
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
    public void ResetBall()
    {
        hitCount = 0;
        isHit = false;
        isMoving = false;
        applyHitForce = false;
        rb.Sleep();
        PlayerController.Instance.GetComponent<LineRenderer>().enabled = true;
        transform.position = startingPos;
        fauxball.transform.position = startingPos;
    }
    public void PrepareHit()
    {
        Debug.Log("Prepare hit");
        rb.WakeUp();
        if (windArea != null) windArea.enabled = false;
        isHit = true;
        isMoving = true;
        hitCount++;

        // Signal FixedUpdate to apply forces on the next physics step
        applyHitForce = true;
    }

    public void ExecuteHit()
    {
        Debug.Log("Execute Hit");
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
        applyHitForce = false;
        rb.linearVelocity = Vector3.zero;
        rb.Sleep();
    
        finalPos = transform.position;
        fauxball.transform.position = finalPos;
        PlayerController.Instance.GetComponent<LineRenderer>().enabled = true;
        //if (windArea != null) windArea.enabled = true;
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
                break;
            case GroundTypes.Fairway:
                break;
            case GroundTypes.Rough:
                break;
            case GroundTypes.Green:
                break;
            case GroundTypes.Water:
                ResetBall();
                break;
            case GroundTypes.Sand:
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
