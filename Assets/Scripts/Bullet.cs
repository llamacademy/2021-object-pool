using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem ImpactSystem;
    private Rigidbody Rigidbody;
    public delegate void OnDisableCallback(Bullet Instance);
    public OnDisableCallback Disable;

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }

    public void Shoot(Vector3 Position, Vector3 Direction, float Speed)
    {
        Rigidbody.velocity = Vector3.zero;
        transform.position = Position;
        transform.forward = Direction;

        Rigidbody.AddForce(Direction * Speed, ForceMode.VelocityChange);
    }

    private void OnTriggerEnter(Collider other)
    {
        ImpactSystem.transform.forward = -1 * transform.forward;
        ImpactSystem.Play();
        Rigidbody.velocity = Vector3.zero;
    }

    private void OnParticleSystemStopped()
    {
        Disable?.Invoke(this);
    }
}
