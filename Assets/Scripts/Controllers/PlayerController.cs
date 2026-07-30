using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private InputSystem_Actions keybindings;

    private void OnEnable()
    {
        if (keybindings == null) keybindings = new();

        keybindings.Player.Hit.performed += Hit_performed;
        keybindings.Player.Reset.performed += Reset_performed;
        keybindings.Player.Pause.performed += Pause_performed;
        keybindings.Player.Rotate.performed += Rotate_performed;
        keybindings.Enable();
    }

    private void Pause_performed(InputAction.CallbackContext obj)
    {
        throw new System.NotImplementedException();
    }

    private void Rotate_performed(InputAction.CallbackContext obj)
    {
        throw new System.NotImplementedException();
    }
    private void Hit_performed(InputAction.CallbackContext obj)
    {
        Debug.Log("Hit Performed");
        if (!BallController.Instance.isMoving && !BallController.Instance.isHit)
            BallController.Instance.PrepareHit();
    }
    private void Reset_performed(InputAction.CallbackContext obj)
    {
        BallController.Instance.ResetBall();
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