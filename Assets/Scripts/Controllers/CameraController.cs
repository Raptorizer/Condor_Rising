using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    [SerializeField] Vector3 offsetPos;
    [SerializeField] Quaternion offsetRot;
    public static CameraController Instance { get; private set; } = null;

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"Found Duplicate CameraController on {gameObject.name}");
            Destroy(gameObject);
            return;
        }
    }
    private void Update()
    {
        transform.SetPositionAndRotation(target.position + offsetPos, offsetRot);
        transform.LookAt(target.position);
    }
}
