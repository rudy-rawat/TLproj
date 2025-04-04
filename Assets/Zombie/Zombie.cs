
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PostProcessing;

public class Zombie : MonoBehaviour
{
    [Header("Zombie Things")]
    public NavMeshAgent agent;
    public GameObject lookPoint;
    public LayerMask playerLayer;

    [Header("Zombie Guarding Variables")]
    public GameObject[] walkPoints;
    private int currentZombiePosition = 0;
    public float walkingSpeed = 1.5f;
    public float chasingSpeed = 2.5f;
    private float walkingPointRadius = 2f;
    public float MinimumPlayerZombieDistance = 3f;

    [Header("Zombie Mods")]
    public float visionRadius;
    public float attackingRadius;
    public bool playerInVisionRadius;
    public bool playerInAttackingRadius;

    [Header("Zombie Attack")]
    public Transform attackPoint;
    public int damage = 10;
    public float attackCooldown = 1.5f;
    private bool alreadyAttacked = false;
    public PlayerHealth playerHealth;

    [Header("Zombie Health and Things")]
    private float zombieHealth = 100f;
    public float presentHealth;

    [Header("Animations")]
    public Animator animator;
    private bool isDead = false;
    private bool isAttacking = false;
    private bool isScreaming = false;
    private bool hasScreamed = false;

    public bool isonfloor = false;

    public float rotationSpeed = 5f; // Speed of smooth rotation

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        presentHealth = zombieHealth;

        // Configure components for root motion
        animator.applyRootMotion = false;
        //agent.updatePosition = false;
        //agent.updateRotation = false; // Let animator handle rotation too
    }

    private void Update()
    {
        if (isDead) return;

        playerInVisionRadius = Physics.CheckSphere(transform.position, visionRadius, playerLayer);
        playerInAttackingRadius = Physics.CheckSphere(transform.position, attackingRadius, playerLayer);

        if (!playerInAttackingRadius && !playerInVisionRadius)
        {
            agent.isStopped = false;
            Guard();
        }
        else if (playerInVisionRadius && !playerInAttackingRadius)
        {
            if (!isScreaming && !hasScreamed)
            {
                agent.isStopped = true;
                StartScreaming();
            }
            else if(hasScreamed)
            {
                agent.isStopped = false;
                ChasePlayer();
            }
        }
        else if (playerInAttackingRadius && playerInVisionRadius)
        {
            AttackPlayer();
            agent.isStopped = true;
        }

        // Set Idle when speed is 0
        //float speed = agent.velocity.magnitude;
        animator.SetFloat("Speed", agent.speed, 0.1f, Time.deltaTime); // Added smoothing
    }

    private void Guard()
    {
        if (walkPoints.Length == 0) return;


        agent.speed = walkingSpeed;

        if (Vector3.Distance(walkPoints[currentZombiePosition].transform.position, transform.position) < walkingPointRadius)
        {
            currentZombiePosition = Random.Range(0, walkPoints.Length);
        }

        agent.SetDestination(walkPoints[currentZombiePosition].transform.position);
        RotateTowardsTarget(walkPoints[currentZombiePosition].transform); // Smooth rotation
    }

    private void StartScreaming()
    {
        isScreaming = true;
        hasScreamed = true;
        agent.isStopped = true;
        //agent.SetDestination(transform.position);
        animator.SetBool("IsScreaming", true);

        Invoke(nameof(ChasePlayer), 2f);
    }

    private void ChasePlayer()
    {


        isScreaming = false;
        animator.SetBool("IsScreaming", false);
        agent.isStopped = false;
        agent.speed = chasingSpeed;

        if (lookPoint != null)
        {
            //agent.isStopped = false;

            agent.SetDestination(lookPoint.transform.position);
            RotateTowardsTarget(lookPoint.transform);
        }
    }

    private void AttackPlayer()
    {
        if (!alreadyAttacked)
        {
            // Stop movement but maintain destination
            agent.isStopped = true;

            // Let animator handle rotation during attack
            if (lookPoint != null)
            {
                RotateTowardsTarget(lookPoint.transform);
            }

            if (Physics.CheckSphere(transform.position, MinimumPlayerZombieDistance, playerLayer))
            {
                agent.ResetPath();
            }


            //Randomly choose between attack and neck bite
            int attackType = Random.Range(0, 2); // 0 = Normal Attack, 1 = Neck Bite
            animator.SetFloat("AttackType", attackType);
            animator.SetBool("IsAttacking", true);
            isAttacking = true;

            RaycastHit hit;
            if (attackPoint != null && Physics.Raycast(attackPoint.position, attackPoint.forward, out hit, attackingRadius))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    if (playerHealth != null)
                    {
                        playerHealth.TakeDamage(damage);
                        Debug.Log("Zombie attacked player!");
                    }
                }
            }

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), attackCooldown);
        }
    }

    private void ResetAttack()
    {
        isAttacking = false;
        animator.SetBool("IsAttacking", false);
        alreadyAttacked = false;
        // Resume movement
        agent.isStopped = false;
    }

    private void RotateTowardsTarget(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0; // Prevents tilting up/down

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    public void ZombieTakeDamage(float dealtDamage)
    {
        presentHealth -= dealtDamage;
        if (presentHealth <= 0)
        {
            ZombieDied();
        }
    }

    private void ZombieDied()
    {
        isDead = true;
        agent.isStopped = true;
        animator.SetBool("IsDead", true);

        int deathType = Random.Range(0, 2); // 0 = Dying1, 1 = Dying2
        animator.SetFloat("DeathType", deathType);

        agent.enabled = false;

        attackingRadius = 0;
        visionRadius = 0;
        playerInAttackingRadius = false;
        playerInVisionRadius = false;

        Object.Destroy(gameObject, 5.0f);
    }
    public void OnTriggerEnter(Collider other)
    {
        if ((other.tag == "floor") && (isonfloor == false))
        {
            agent.enabled = false;
            agent.enabled = true;
            isonfloor = true;
        }
    }

    //void OnAnimatorMove()
    //{
    //    if (isDead) return;

    //    // Get root position from animator
    //    Vector3 rootPosition = animator.rootPosition;

    //    // Preserve NavMeshAgent's Y position for slope handling
    //    rootPosition.y = agent.nextPosition.y;

    //    // Update transform and NavMeshAgent
    //    // Partial implementation missing velocity updates
    //    transform.position = rootPosition;
    //    agent.nextPosition = transform.position;

    //    // Update agent velocity for proper speed calculation
    //    if (Time.deltaTime > 0)
    //    {
    //        agent.velocity = animator.deltaPosition / Time.deltaTime;
    //    }
    //}


}



