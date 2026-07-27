using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    [SerializeField] Vector3 offsetPos;
    [SerializeField] Quaternion offsetRot;
    public int sensitivity;
    public float smoothTime;
    public Vector2Int rotationXMinMax;
    public static CameraController instance { get; private set; } = null;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError($"Found Duplicate CameraController on {gameObject.name}");
            Destroy(gameObject);
            return;
        }
    }
    private void Update()
    {
        transform.position = target.position + offsetPos;
        transform.rotation = offsetRot;
        transform.LookAt(target.position);
    }
}
