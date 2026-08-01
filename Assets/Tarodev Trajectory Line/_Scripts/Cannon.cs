using UnityEngine;
using UnityEngine.InputSystem;

public class Cannon : MonoBehaviour {
    [SerializeField] private Projection _projection;
    private InputSystem_Actions keybindings;

    [SerializeField] private Ball _ballPrefab;
    [SerializeField] private float _force = 20;
    [SerializeField] private Transform _ballSpawn;
    [SerializeField] private Transform _barrelPivot;
    [SerializeField] private float _rotateSpeed = 30;
    [SerializeField] private AudioSource _source;
    [SerializeField] private AudioClip _clip;
    [SerializeField] private Transform _leftWheel, _rightWheel;
    [SerializeField] private ParticleSystem _launchParticles;

    private void FixedUpdate() 
    {
        _projection.SimulateTrajectory(_ballPrefab, _ballSpawn.position, _ballSpawn.forward * _force);
    }

    #region Handle Controls
   
    private void OnEnable()
    {
        keybindings ??= new();

        keybindings.Player.Hit.performed += Hit_performed;
        keybindings.Player.Pause.performed += Pause_performed;
        keybindings.Player.RotateLeft.performed += Rotateleft_performed;
        keybindings.Player.RotateRight.performed += Rotateright_performed;
        keybindings.Player.RotateUp.performed += Rotateup_performed;
        keybindings.Player.RotateDown.performed += Rotatedown_performed;
        keybindings.Enable();
    }
    private void Pause_performed(InputAction.CallbackContext obj)
    {
        throw new System.NotImplementedException();
    }

    private void Rotateleft_performed(InputAction.CallbackContext obj)
    {
        _barrelPivot.Rotate(_rotateSpeed * Time.deltaTime * Vector3.left);
    }
    private void Rotateright_performed(InputAction.CallbackContext obj)
    {
        _barrelPivot.Rotate(_rotateSpeed * Time.deltaTime * Vector3.right);
    }
    private void Rotateup_performed(InputAction.CallbackContext obj)
    {
        transform.Rotate(_rotateSpeed * Time.deltaTime * Vector3.up);
        _leftWheel.Rotate(_rotateSpeed * 1.5f * Time.deltaTime * Vector3.back);
        _rightWheel.Rotate(_rotateSpeed * 1.5f * Time.deltaTime * Vector3.forward);
    }
    private void Rotatedown_performed(InputAction.CallbackContext obj)
    {
        transform.Rotate(_rotateSpeed * Time.deltaTime * Vector3.down);
        _leftWheel.Rotate(_rotateSpeed * Time.deltaTime * Vector3.forward);
        _rightWheel.Rotate(_rotateSpeed * Time.deltaTime * Vector3.back);
    }
    private void Hit_performed(InputAction.CallbackContext obj)
    {
        var spawned = Instantiate(_ballPrefab, _ballSpawn.position, _ballSpawn.rotation);

        spawned.Init(_ballSpawn.forward * _force, false);
        _launchParticles.Play();
        _source.PlayOneShot(_clip);
    }
    //private void Reset_performed(InputAction.CallbackContext obj)
    //{
    //    ballController.ResetBall();
    //    ballSpawnPoint.gameObject.SetActive(true);
    //    windManager.SetWindValues();
    //}
    private void OnDisable()
    {
        keybindings.Player.Hit.performed -= Hit_performed;
        keybindings.Player.Pause.performed -= Pause_performed;
        keybindings.Player.RotateLeft.performed -= Rotateleft_performed;
        keybindings.Player.RotateRight.performed -= Rotateright_performed;
        keybindings.Player.RotateUp.performed -= Rotateup_performed;
        keybindings.Player.RotateDown.performed -= Rotatedown_performed;
        keybindings.Disable();
    }
    #endregion
}