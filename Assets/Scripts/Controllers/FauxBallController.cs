using UnityEngine;

public class FauxBallController : MonoBehaviour
{
    [SerializeField]  Rigidbody _rb;
    //[SerializeField]  GameObject _poofPrefab;
     bool _isGhost;

    public void Init(Vector3 velocity, bool isGhost)
    {
        _isGhost = isGhost;
        _rb.AddForce(velocity);
    }

    public void OnCollisionEnter(Collision col)
    {
        if (_isGhost) return;
        //Instantiate(_poofPrefab, col.contacts[0].point, Quaternion.Euler(col.contacts[0].normal));
    }
}
