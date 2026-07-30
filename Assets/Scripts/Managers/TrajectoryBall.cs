using UnityEngine;

public class TrajectoryBall : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] GameObject impactEffect;


    public void BallHit()
    {
        BallController.Instance.PrepareHit();
    }
    public void OnCollisionEnter(Collision collision)
    {
        Instantiate(impactEffect, collision.contacts[0].point, Quaternion.Euler(collision.contacts[0].normal));
    }
}
