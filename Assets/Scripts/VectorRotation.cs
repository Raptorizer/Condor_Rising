using UnityEngine;

public class VectorRotation : MonoBehaviour
{
    const float LengthMultiplier = 4f;

    [SerializeField] Vector3 sourceVector;
    [SerializeField] Vector3 rotatedVector;
    Vector3 LimitA;
    Vector3 LimitB;
    [SerializeField] float rotationRange;


    private void OnValidate()
    {
        LimitA = Quaternion.AngleAxis(-rotationRange / 2, Vector3.forward) * sourceVector;
        LimitB = Quaternion.AngleAxis(rotationRange / 2, Vector3.forward) * sourceVector;
    }

    private void OnDrawGizmos()
    {
        var p = transform.position;
        Gizmos.color = Color.white;
        Gizmos.DrawRay(p, sourceVector * LengthMultiplier);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(p, rotatedVector * LengthMultiplier);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(p, LimitA * LengthMultiplier);
        Gizmos.DrawRay(p, LimitB * LengthMultiplier);
    }
    [ContextMenu("Rotate")]
    void RotateVector()
    {
        var randomRotationAngle = Random.Range(-rotationRange / 2, rotationRange / 2);
        rotatedVector = Quaternion.AngleAxis(randomRotationAngle, Vector3.forward) * sourceVector;
    }
}
