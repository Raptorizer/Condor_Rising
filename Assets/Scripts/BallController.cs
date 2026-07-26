using System;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [Header("Ball Hit Values")]
    [SerializeField] int shootHeight;
    [SerializeField] int shootPower;
    [Range(0, 1)]
    [SerializeField] float spinValue;
    public enum SpinDirection{up, down, left, right}
    [SerializeField] SpinDirection spinDirection;

    bool isHit;
    [SerializeField] int randomDistance;

    //raycast info
    [SerializeField] int groundValue;
    [SerializeField] LayerMask lm;
    [SerializeField] float rcDistance = 0.01f;
    RaycastHit rch;

    Rigidbody rb;
    Vector3 startingPos;
    Vector3 finalPos;
    Transform mc;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mc = Camera.main.transform;
        rb = GetComponent<Rigidbody>();
        startingPos = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //show raycast
        //Debug.DrawRay(transform.position, Vector2.down * rcDistance, Color.green);

        //Check where ball stopped at
        finalPos = this.transform.position;
        //Ground check
        if (Physics.Raycast(transform.position, Vector3.down, out rch, rcDistance, lm))
        {
            GroundCheck();
        }
        if (Input.GetKeyDown(KeyCode.Space) && !isHit)
        {
            rb.WakeUp();
            isHit = true;
            randomDistance = UnityEngine.Random.Range(-100, 100);
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
        rb.AddForce(Vector3.forward * (shootPower + randomDistance));
    }
    void AddSpin()
    {

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
        if(collision.collider.tag != "Start")
        {
            rb.mass = 40;
            rb.linearDamping = 0.6f;
        }
        switch (spinDirection)
        {
            case SpinDirection.up:
                break;
            case SpinDirection.down:
                break;
            case SpinDirection.left:
                rb.AddForce(Vector3.left * (-spinValue *100));
                break;
            case SpinDirection.right:
                rb.AddForce(Vector3.right * (spinValue*100));
                break;
        }
    }
    void OnTriggerEnter(Collider other)
    {
        this.transform.position = startingPos;
    }
}
