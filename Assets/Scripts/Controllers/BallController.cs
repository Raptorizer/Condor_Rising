using System;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [Header("Ball info")]
    Rigidbody rb;
    Vector3 startingPos;
    Vector3 finalPos;
    bool isHit;
    public int shootHeight;
    public int shootPower;
    [Range(0, 0.5f)]
    public float spinValue;
    public enum SpinDirection{up, down, left, right}
    [SerializeField] SpinDirection spinDirection;


    [Header("Raycast Info")]
    [SerializeField] LayerMask lm;
    int groundValue;
    float rcDistance = 0.01f;
    RaycastHit rch;

    GameObject windInfo;

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

        rb = GetComponent<Rigidbody>();
        startingPos = this.transform.position;
    }

    void Update()
    {
        //show raycast
        //Debug.DrawRay(transform.position, Vector2.down * rcDistance, Color.green);

        //Check where ball stopped at
        finalPos = this.transform.position;
        //Ground check
        if (Physics.Raycast(transform.position, Vector3.down, out rch, rcDistance, lm)) GroundCheck();
        if (Input.GetKeyDown(KeyCode.Space) && !isHit)
        {
            rb.WakeUp();
            isHit = true;
            BallHit();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetBall();
            isHit = false;
        }
    }

    void ResetBall()
    {
        //call finalpos and reset back to 0
        Debug.Log("Final Position: " + finalPos);
        finalPos = Vector3.zero;
        //reset rigidbody values
        rb.mass = 1;
        rb.linearDamping = 0.1f;
        this.transform.position = startingPos;
        rb.Sleep();
    }
    void BallHit()
    {
        //add force to ball
        rb.AddForce(Vector3.up * shootHeight);
        rb.AddForce(Vector3.forward * shootPower);
        if(spinDirection != 0) AddSpin();
        if (windInfo.GetComponent<WindManager>().isWindy) rb.AddForce(windInfo.GetComponent<WindManager>().windDirection * windInfo.GetComponent<WindManager>().windPower);
        else if (windInfo.GetComponent<WindManager>().isRandWindy) rb.AddForce(windInfo.GetComponent<WindManager>().windRandomDirection * windInfo.GetComponent<WindManager>().windRandomPower);
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
                rb.AddForce(Vector3.left * (-spinValue * 100));
                break;
            case SpinDirection.right:
                rb.AddForce(Vector3.right * (spinValue * 100));
                break;
        }
    }
    void GroundCheck()
    {
        Debug.Log(rch.collider.tag + " " + groundValue);
        switch (rch.collider.tag)
        {
            case "Rock":
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
        windInfo = other.gameObject;
        if (other.tag != "WindArea") transform.position = startingPos;
    }
}
