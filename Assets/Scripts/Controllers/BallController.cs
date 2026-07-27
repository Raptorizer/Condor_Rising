using System;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [Header("Ball info")]
    Rigidbody rb;
    public Vector3 startingPos;
    public Vector3 currentPos;
    public Vector3 finalPos;
    public float ballSpeed;
    public bool isHit;
    public bool isMoving;
    public int hitCount;
    public int hitHeight = 1000;
    public int hitStrength = 2000;
    [Range(0, 0.5f)]
    public float spinPower;
    public enum SpinDirection{none, up, down, left, right}
    public SpinDirection spinDirection;
    public string groundValue;

    BoxCollider startCollider;
    WindManager windInfo;

    public static BallController instance { get; private set; } = null;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError($"Found Duplicate BallController on {gameObject.name}");
            Destroy(gameObject);
            return;
        }
    }
    void Start()
    {
        windInfo = GameObject.Find("WindManager").GetComponent<WindManager>();
        rb = GetComponent<Rigidbody>();
        //startCollider = GameObject.Find("StartCollider").GetComponent<BoxCollider>();
        startingPos = transform.position;
        groundValue = "Start";
    }

    void Update()
    {
        //show raycast
        //Debug.DrawRay(transform.position, Vector2.down * rcDistance, Color.green);

        currentPos = transform.position;
        ballSpeed = rb.linearVelocity.magnitude;
        if (ballSpeed < 0.5 && groundValue != "Start" && isMoving && isHit) BallStopRolling();
        if (Input.GetKeyDown(KeyCode.Space) && !isHit && !isMoving)
        {
            rb.WakeUp();
            BallHit();
        }
        if (Input.GetKeyDown(KeyCode.R)) ResetBall();
    }

    void ResetBall()
    {
        //reset game values
        hitCount = 0;
        isHit = false;
        isMoving = false;
        //reset position values
        finalPos = Vector3.zero;
        transform.position = startingPos;
        //reset rigidbody values
        rb.mass = 1;
        rb.linearDamping = 0.1f;
        rb.Sleep();
    }
    void BallHit()
    {
        //remove startCollider
        //startCollider.enabled = false;
        //add force to ball
        rb.AddForce(Vector3.up * hitHeight);
        rb.AddForce(Vector3.forward * hitStrength);
        if(spinDirection != 0) AddSpin();
        if (windInfo.isWindy && !windInfo.isRandWindy) rb.AddForce(windInfo.windDirection * windInfo.windPower);
        else if (windInfo.isWindy && windInfo.isRandWindy) rb.AddForce(windInfo.windRandomDirection * windInfo.windRandomPower);
        isHit = true;
        isMoving = true;
        hitCount++;
    }
    void BallStopRolling()
    {
        //Stops the ball from moving
        isMoving = false;
        isHit = false;
        rb.linearVelocity = new Vector3(0, 0, 0);
        rb.Sleep();
        finalPos = transform.position;
        //Debug.Log("Final Position: " + finalPos);
       //startCollider.enabled = true;
    }
    void AddSpin()
    {
        switch (spinDirection)
        {
            case SpinDirection.up:
                rb.AddForce(Vector3.up * (spinPower * 1000));
                break;
            case SpinDirection.down:
                rb.AddForce(Vector3.down * (-spinPower * 1000));
                break;
            case SpinDirection.left:
                rb.AddForce(Vector3.left * (-spinPower * 1000));
                break;
            case SpinDirection.right:
                rb.AddForce(Vector3.right * (spinPower * 1000));
                break;
            default:
                break;
        }
    }

     void OnCollisionEnter(Collision collision)
    {
        //Debug.Log($"Touched {collision.collider.tag}");
        switch (collision.collider.tag)
        {
            case "Start":
                groundValue = collision.collider.tag;
                break;
            case "Fairway":
                groundValue = collision.collider.tag;
                rb.mass = 40;
                rb.linearDamping = 0.6f;
                break;
            case "Sand":
                groundValue = collision.collider.tag;
                BallStopRolling();
                break;
            case "Green":
                groundValue = collision.collider.tag;
                break;
            case "Water":
                groundValue = collision.collider.tag;
                ResetBall();
                break;
            case "WindArea":
                break;
            default:
                groundValue = collision.collider.tag;
                break;
        }
    }
}
