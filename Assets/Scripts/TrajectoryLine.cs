using UnityEngine;

public class TrajectoryLine : MonoBehaviour
{
    [Header("References")]
    [SerializeField] BallController bc;

    [Header("Trajectory Line Smoothness/Length")]
    [SerializeField] int _segmentCount = 50;
    [SerializeField] float _curveLength = 3.5f;
    Vector3[] _segments;
    LineRenderer lr;

    float hitPower;
    float _hitGravity;

    const float TIME_CURVE_ADDITION = 0.5f;
    private void Start()
    {
        //initialize segments
        _segments = new Vector3[_segmentCount];

        //Grab Line Renderer and set its points
        lr = GetComponent<LineRenderer>();
        lr.positionCount = _segmentCount;

        //grab the projectile speed from the player's bullet behavior
        bc = GetComponentInParent<BallController>();
        hitPower = bc.hitStrength;
        _hitGravity = bc.hitGravity;
    }

    private void Update()
    {
        //set the starting position of the line Renderer
        Vector3 startPos = bc.startingPos;
        _segments[0] = startPos;
        lr.SetPosition(0, startPos);

        //set the start velocity based on balls
        Vector3 startVelocity = -transform.right * hitPower;
        for(int i = 1; i < _segmentCount; i++)
        {
            //compute the time offset
            float timeOffset = (i * Time.fixedDeltaTime * _curveLength);

            //compute gravity
            Vector3 gravityOffset = TIME_CURVE_ADDITION * Physics.gravity * _hitGravity * Mathf.Pow(timeOffset, 2);

            //Set the position of the point in the line renderer
            _segments[i] = _segments[0] + startVelocity * timeOffset + gravityOffset;
            lr.SetPosition(i, _segments[i]);
        }
    }

}
