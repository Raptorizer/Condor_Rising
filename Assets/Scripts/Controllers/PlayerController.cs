using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private InputSystem_Actions keybindings;
    float rotationValue;
    [SerializeField] float rotationSpeed = 100f; //Degrees per second
    [SerializeField] TrajectoryLine trajectoryLine;
    [SerializeField] BallController ballController;
    [SerializeField] Transform ballSpawnPoint;
    Vector3 TotalForce;


    private void OnEnable()
    {
        if (keybindings == null) keybindings = new();

        keybindings.Player.Hit.performed += Hit_performed;
        keybindings.Player.Reset.performed += Reset_performed;
        keybindings.Player.Pause.performed += Pause_performed;
        keybindings.Player.Rotate.performed += Rotate_performed;
        keybindings.Player.Rotate.canceled += Rotate_canceled;
        keybindings.Enable();
    }
    private void Update()
    {
        RotatePlayer();
        TotalForce = (Vector3.up * ballController.hitHeight) + (Vector3.forward * ballController.hitStrength);
        trajectoryLine.SimulateTrajectory(ballController, ballSpawnPoint.position, TotalForce);
    }
    private void Pause_performed(InputAction.CallbackContext obj)
    {
        throw new System.NotImplementedException();
    }

    private void Rotate_performed(InputAction.CallbackContext obj)
    {
        // Reads the -1 (Left) or 1 (Right) float value from the keys
        rotationValue = obj.ReadValue<float>();
    }
    private void Rotate_canceled(InputAction.CallbackContext obj)
    {
        rotationValue = 0f;
    }
    private void Hit_performed(InputAction.CallbackContext obj)
    {
        Debug.Log("Hit Performed");
        if (!ballController.isMoving && !ballController.isHit)
        {
            var Spawned = Instantiate(ballController, ballSpawnPoint.position, ballSpawnPoint.rotation);
            Spawned.Init(TotalForce,false);
        }
    }
    private void Reset_performed(InputAction.CallbackContext obj)
    {
        BallController.Instance.ResetBall();
    }
    void RotatePlayer()
    {
        // Calculate framing rotation independent of frame rates
        float rotationAmount = rotationValue * rotationSpeed * Time.deltaTime;

        // For a standard 3D game (rotating around the Y-Axis)
        transform.Rotate(0, rotationAmount, 0);

    }
    private void OnDisable()
    {
        keybindings.Player.Hit.performed -= Hit_performed;
        keybindings.Player.Reset.performed -= Reset_performed;
        keybindings.Player.Pause.performed -= Pause_performed;
        keybindings.Player.Rotate.performed -= Rotate_performed;
        keybindings.Disable();
    }
}