using System;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [Header("Ball info")]
    Rigidbody rb;
    public Vector3 startingPos;
    public Vector3 currentPos;
    public Vector3 finalPos;
    public bool isHit;
    public bool isMoving;
    public int hitCount;
    public int hitHeight = 1000;
    public int hitStrength = 2000;
    [Range(0, 0.5f)]
    public float spinPower;
    public enum SpinDirection{up, down, left, right}
    public SpinDirection spinDirection;


    [Header("Raycast Info")]
    [SerializeField] LayerMask lm;
    public int groundValue;
    float rcDistance = 0.01f;
    RaycastHit rch;

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
        startingPos = transform.position;
    }

    void Update()
    {
        //show raycast
        //Debug.DrawRay(transform.position, Vector2.down * rcDistance, Color.green);

        //Check where ball stopped at
        currentPos = transform.position;
        //Ground check
        if (Physics.Raycast(transform.position, Vector3.down, out rch, rcDistance, lm)) GroundCheck();
        if (Input.GetKeyDown(KeyCode.Space) && !isHit && !isMoving)
        {
            rb.WakeUp();
            BallHit();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetBall();
        }
        //checks if the ball stops moving
        if(rb.linearVelocity.magnitude < 0.5f)
        {
            isMoving = false;
            isHit = false;
            //call finalpos
            rb.linearVelocity = new Vector3(0, 0, 0);
            if (groundValue != 0)
            {
                finalPos = transform.position;
                Debug.Log("Final Position: " + finalPos);
            }
        }
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

        //add force to ball
        rb.AddForce(Vector3.up * hitHeight);
        rb.AddForce(Vector3.forward * hitStrength);
        if(spinDirection != 0) AddSpin();
        if (windInfo.isWindy && !windInfo.isRandWindy) rb.AddForce(windInfo.windDirection * windInfo.windPower);
        else if (windInfo.isRandWindy) rb.AddForce(windInfo.windRandomDirection * windInfo.windRandomPower);
        isHit = true;
        isMoving = true;
        hitCount++;
    }
    void AddSpin()
    {
        switch (spinDirection)
        {
            case SpinDirection.up:
                break;
            case SpinDirection.down:
                break;
            case SpinDirection.left:
                rb.AddForce(Vector3.left * (-spinPower * 100));
                break;
            case SpinDirection.right:
                rb.AddForce(Vector3.right * (spinPower * 100));
                break;
        }
    }
    void GroundCheck()
    {
        Debug.Log(rch.collider.tag + " " + groundValue);
        switch (rch.collider.tag)
        {
            case "Start":
                groundValue = 0;
                break;
            case "Fairway":
                groundValue = 1;
                break;
            case "Sand":
                groundValue = 2;
                break;
            case "Green":
                groundValue = 3;
                break;
            case "Water":
                groundValue = 4;
                ResetBall();
                break;
            default:
                groundValue = 0;
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Touched the ground");
        if(collision.collider.tag != "Start" && collision.collider.tag != "WindArea")
        {
            rb.mass = 40;
            rb.linearDamping = 0.6f;
        }
    }
    void OnTriggerEnter(Collider other)
    {
        //windInfo = other.gameObject;
        if (other.tag != "WindArea") transform.position = startingPos;
    }
}
