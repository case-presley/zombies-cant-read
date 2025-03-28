using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// --------------------------------------------------------
/// ZombieAI
///
/// Basic AI for zombies:
/// - Uses NavMeshAgent to chase the player
/// - Attacks when within range and cooldown is complete
/// - Plays animation states based on action
/// - Triggers death animation and self-destruction
///
/// Attach this to any enemy prefab with Animator and NavMeshAgent.
/// --------------------------------------------------------
public class ZombieAI : MonoBehaviour
{
    public Transform player;                      // Reference to the player target
    public float attackDelay = 0.9f;              // Delay between attack animation and hit
    public int attackDamage = 50;                 // Damage dealt to player on hit
    public float attackRange = 2f;                // Max distance for a valid hit

    private NavMeshAgent agent;                   // Unity's pathfinding component
    private Animator animator;                    // Animator for zombie animations

    private bool isAttacking;                     // Prevent overlapping attacks
    private bool playerInRange;                   // Tracks if player is in attack zone

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        ChasePlayerAnimation();
    }

    private void Update()
    {
        // --- Chase player if not attacking ---
        if (!isAttacking && player != null)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        else
        {
            agent.isStopped = true;
        }

        // --- Begin attack if player is in range ---
        if (playerInRange && !isAttacking)
        {
            StartCoroutine(AttackPlayer());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    /// Executes attack after a short delay, deals damage if still in range
    private IEnumerator AttackPlayer()
    {
        isAttacking = true;
        AttackPlayerAnimation();

        yield return new WaitForSeconds(attackDelay);

        // Confirm target is still valid and within range
        if (playerInRange && Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            Debug.Log("Zombie hit the player!");
            player.GetComponent<PlayerStats>().TakeDamage(attackDamage);
        }

        isAttacking = false;
        ChasePlayerAnimation();
    }

    /// Triggers walk/run animation
    public void ChasePlayerAnimation()
    {
        animator.SetBool("isWalking", true);
        animator.SetBool("isAttacking", false);
    }

    /// Triggers attack animation (random variant)
    public void AttackPlayerAnimation()
    {
        int attackingAnimation = Random.Range(0, 3);
        animator.SetBool("isWalking", false);
        animator.SetBool("isAttacking", true);
        animator.SetInteger("attackingAnimation", attackingAnimation);
    }

    /// Handles death logic and disables zombie systems
    public void Die()
    {
        animator.SetBool("isDead", true);
        agent.isStopped = true;
        agent.enabled = false;
        GetComponent<Collider>().enabled = false;

        StartCoroutine(DestroyAfterDeath());
    }

    /// Waits for death animation to finish before destroying the object
    private IEnumerator DestroyAfterDeath()
    {
        yield return new WaitForSeconds(0.1f); // Buffer to trigger death animation
        float deathAnimationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(deathAnimationLength);

        Destroy(gameObject);
    }
}
