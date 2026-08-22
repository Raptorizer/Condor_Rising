using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [Header("Core References")]
    [SerializeField]  TrajectoryLine trajectoryLine;
    [SerializeField]  BallController ballController;
    [SerializeField]  WindManager windManager;
    [SerializeField]  LineRenderer aimLineRenderer; // Cached to avoid GetComponent calls

    [Header("Aiming Settings")]
    [SerializeField]  Transform targetHole; // Assign the hole/pin in the Inspector
    [SerializeField]  Transform ballSpawnPoint;
    public float rotationSpeed = 60f;
    [SerializeField]  float maxRotationLimit = 45f;

    [Header("Hit Power")]
    public float hitHeight = 50f;
    public float hitStrength = 100f;

     InputSystem_Actions keybindings;
     float rotationValue;
     float baseYaw;
     float currentYaw;

    // Limits recalculations to when aim actually shifts
     bool needsTrajectoryUpdate = true;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Fallback in case it wasn't assigned in the inspector
        if (aimLineRenderer == null) aimLineRenderer = GetComponent<LineRenderer>();

        if (ballSpawnPoint != null)
        {
            baseYaw = ballSpawnPoint.eulerAngles.y;
            currentYaw = baseYaw;
        }
        AimAtTarget(targetHole);
    }

    private void OnEnable()
    {
        if (keybindings == null) keybindings = new InputSystem_Actions();

        keybindings.Player.Hit.performed += Hit_performed;
        keybindings.Player.Reset.performed += Reset_performed;
        keybindings.Player.Rotate.performed += Rotate_performed;
        keybindings.Player.Rotate.canceled += Rotate_canceled;
        keybindings.Enable();
    }

    private void Update()
    {
        if (rotationValue != 0f)
        {
            RotatePlayer();
            needsTrajectoryUpdate = true;
        }

        if (needsTrajectoryUpdate && !ballController.isMoving)
        {
            Vector3 baseForce = CalculateBaseHitForce();
            trajectoryLine.SimulateTrajectory(ballSpawnPoint.position, baseForce);
            needsTrajectoryUpdate = false;
        }
    }

    private Vector3 CalculateBaseHitForce()
    {
        Vector3 forwardForce = ballSpawnPoint.forward * hitStrength;
        Vector3 upwardForce = Vector3.up * hitHeight;
        return forwardForce + upwardForce;
    }

    #region Controls
    private void Hit_performed(InputAction.CallbackContext obj)
    {
        if (!ballController.isMoving && !ballController.isHit)
        {
            aimLineRenderer.enabled = false;

            Vector3 finalForce = CalculateBaseHitForce();
            ballController.ExecuteHit(finalForce);

            needsTrajectoryUpdate = false;
        }
    }

    private void Rotate_performed(InputAction.CallbackContext obj)
    {
        rotationValue = obj.ReadValue<float>();
    }

    private void Rotate_canceled(InputAction.CallbackContext obj)
    {
        rotationValue = 0f;
    }

    private void Reset_performed(InputAction.CallbackContext obj)
    {
        ballController.ResetBall();
        aimLineRenderer.enabled = true;

        windManager?.SetWindValues();

        // Snap the aim back to the target on reset
        if (targetHole != null)
        {
            AimAtTarget(targetHole);
        }
        else
        {
            needsTrajectoryUpdate = true;
        }
    }
    #endregion

    private void RotatePlayer()
    {
        float rotationAmount = rotationValue * rotationSpeed * Time.deltaTime;
        currentYaw += rotationAmount;

        // Clamp the rotation relative to the base yaw
        currentYaw = Mathf.Clamp(currentYaw, baseYaw - maxRotationLimit, baseYaw + maxRotationLimit);

        ballSpawnPoint.rotation = Quaternion.Euler(0, currentYaw, 0);

        // Sync the ball's visual orientation
        if (ballController != null)
        {
            ballController.transform.rotation = ballSpawnPoint.rotation;
        }
    }

    public void AimAtTarget(Transform targetHole)
    {
        if (targetHole != null)
        {
            ballSpawnPoint.LookAt(targetHole);

            Vector3 eulerAngles = ballSpawnPoint.eulerAngles;
            ballSpawnPoint.rotation = Quaternion.Euler(0, eulerAngles.y, 0);

            baseYaw = eulerAngles.y;
            currentYaw = baseYaw;

            if (ballController != null)
            {
                ballController.transform.rotation = ballSpawnPoint.rotation;
            }

            needsTrajectoryUpdate = true;
        }
    }
    // Call this from BallController when the ball comes to a complete stop
    public void EnableAimLine()
    {
        if (aimLineRenderer != null)
        {
            aimLineRenderer.enabled = true;

            // Re-center the aim toward the hole from the new position
            if (targetHole != null)
            {
                AimAtTarget(targetHole);
            }
            else
            {
                needsTrajectoryUpdate = true;
            }
        }
    }
    private void OnDisable()
    {
        keybindings?.Disable();
    }
}