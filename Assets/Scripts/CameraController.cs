using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    [SerializeField] Vector3 offsetPos;
    [SerializeField] Quaternion offsetRot;
    public int sensitivity;
    public float smoothTime;
    public Vector2Int rotationXMinMax;

    private void Update()
    {
        transform.position = target.position + offsetPos;
        transform.rotation = offsetRot;
        transform.LookAt(target.position);
    }
}
