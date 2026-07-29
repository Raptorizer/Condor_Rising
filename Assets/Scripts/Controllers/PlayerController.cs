using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private TrajectoryLine trajectoryLine;

    private void Update()
    {
        HandleControls();
        trajectoryLine.SimulateTrajectory(golfBall, _ballSpawn.position);
    }

    #region Handle Controls

    [SerializeField] private BallController golfBall;
    [SerializeField] private Transform _ballSpawn;
    [SerializeField] private Transform playerTurn;
    [SerializeField] private float _rotateSpeed = 30;
    [SerializeField] private ParticleSystem _launchParticles;

    /// <summary>
    /// This is absolute spaghetti and should not be look upon for inspiration. I quickly smashed this together
    /// for the tutorial and didn't look back
    /// </summary>
    private void HandleControls()
    {
        if (Input.GetKey(KeyCode.S)) playerTurn.Rotate(_rotateSpeed * Time.deltaTime * Vector3.right);
        else if (Input.GetKey(KeyCode.W)) playerTurn.Rotate(_rotateSpeed * Time.deltaTime * Vector3.left);

        if (Input.GetKey(KeyCode.A)) 
            transform.Rotate(_rotateSpeed * Time.deltaTime * Vector3.down);
        else if (Input.GetKey(KeyCode.D))
            transform.Rotate(_rotateSpeed * Time.deltaTime * Vector3.up);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(golfBall, _ballSpawn.position, _ballSpawn.rotation);
            golfBall.PrepareHit();
        }
    }
    #endregion
}
