using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] float maxHealth = 100f;
    [SerializeField] float currentHealth;
    
    [SerializeField] float damageCooldown = 0.5f;
    private bool _canTakeDamage = true;

    [Header("Effects")]
    [SerializeField] GameObject deathEffect; // Optional VFX
    //[SerializeField] AudioClip hitSound;      // Optional SFX

    void Start()
    {
        currentHealth = maxHealth;
    }

    // Called when a particle/collider hits the enemy
    void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Skill"))
        {
            SkillDamage skill = other.GetComponent<SkillDamage>();
            if (skill != null)
            {
                TakeDamage(skill.damage);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (!_canTakeDamage) return;

        currentHealth -= damage;
        _canTakeDamage = false;
        Invoke(nameof(ResetDamageCooldown), damageCooldown);

        // Play hit sound
        /*if (hitSound != null)
            AudioSource.PlayClipAtPoint(hitSound, transform.position);*/

        // Check for death
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Play death effect
        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    void ResetDamageCooldown()
    {
        _canTakeDamage = true;
    }
}