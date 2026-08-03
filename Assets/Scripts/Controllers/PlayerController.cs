using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    InputSystem_Actions keybindings;
    public static PlayerController Instance { get; private set; } = null;
    [SerializeField] TrajectoryLine trajectoryLine;
    [SerializeField] BallController ballController;
    [SerializeField] WindManager windManager;
    [SerializeField] FauxBallController fauxBall;

    [SerializeField] Transform ballSpawnPoint;
    [SerializeField] Vector3 TotalForce;
    [SerializeField] float rotationValue;
    [SerializeField] public float rotationSpeed = 60f; //Degrees per second

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError($"Found Duplicate Player Controller on {gameObject.name}");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
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
        TotalForce = ballController.totalForce;
        trajectoryLine.SimulateTrajectory(fauxBall, ballSpawnPoint.position, TotalForce);
    }
#region Controls
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
            this.GetComponent<LineRenderer>().enabled = false;
            ballController.PrepareHit();
    }
    private void Reset_performed(InputAction.CallbackContext obj)
    {
        ballController.ResetBall();
        this.GetComponent<LineRenderer>().enabled = true;
        windManager.SetWindValues();
    }
#endregion
    void RotatePlayer()
    {
        // Calculate framing rotation independent of frame rates
        float rotationAmount = rotationValue * rotationSpeed * Time.deltaTime;
        //ballSpawnPoint.Rotate(rotationAmount * Time.deltaTime * Vector3.right);

        // For a standard 3D game (rotating around the Y-Axis)
        ballSpawnPoint.transform.Rotate(0, rotationAmount, 0);

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