using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 targetDirection;
    private float speed;
    private float lifetime;
    private Rigidbody rb;


    public void Initialize(Vector3 spawnPosition, Vector3 movingPosition, float projectileSpeed, float duration)
    {
        rb = GetComponent<Rigidbody>();
        targetDirection = (movingPosition - spawnPosition).normalized;

        speed = projectileSpeed;
        lifetime = duration;
        Debug.Log(targetDirection);
        rb.linearVelocity = targetDirection * speed;
        Destroy(gameObject, lifetime); // Destroy after a set time
    }


}
