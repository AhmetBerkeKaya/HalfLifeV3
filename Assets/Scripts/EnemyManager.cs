using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    public int EnemyHealth = 200;
    // Navmesh
    public NavMeshAgent enemyAgent;
    public Transform player;
    public LayerMask groundLayer;
    public LayerMask playerLayer;

    // Patrolling
    public Vector3 walkPoint;
    public float walkPointRange;
    public bool walkPointSet;

    //
    public float sightRange, attackRange;
    public bool EnemySightRange, EnemyAttackRange;
    //Attacking
    public float attackDelay;
    public bool isAttacking;

    public Transform attackPoint;
    public GameObject projectile;
    public float projectileForce = 18f;
    public Animator enemyAnimator;
    private GameManager gameManager;

    void Start()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player").transform;
        enemyAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        EnemySightRange = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        EnemyAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if(!EnemySightRange && !EnemyAttackRange)
        {
            Patrolling();
            enemyAnimator.SetBool("Patrolling", true);
            enemyAnimator.SetBool("PlayerDetecting", false);
            enemyAnimator.SetBool("PlayerAttacking", false);
        }
        else if(EnemySightRange && !EnemyAttackRange)
        {
            DetectPlayer();
            enemyAnimator.SetBool("Patrolling", false);
            enemyAnimator.SetBool("PlayerDetecting", true);
            enemyAnimator.SetBool("PlayerAttacking", false);

        }
        else if(EnemySightRange && EnemyAttackRange)
        {
            AttackPlayer();
            enemyAnimator.SetBool("Patrolling", false);
            enemyAnimator.SetBool("PlayerDetecting", false);
            enemyAnimator.SetBool("PlayerAttacking", true);
        }
    }

    void Patrolling()
    {
        if(walkPointSet == false)
        {
            float randomZPos = Random.Range(-walkPointRange, walkPointRange);
            float randomXPos = Random.Range(-walkPointRange, walkPointRange);
        
            walkPoint = new Vector3(transform.position.x + randomXPos,transform.position.y, transform.position.z +randomZPos);

            if(Physics.Raycast(walkPoint, -transform.up, 2f, groundLayer))
            {
                walkPointSet = true;
            }

        }

        if (walkPointSet == true)
        {
            enemyAgent.SetDestination(walkPoint);
        }
        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }

    void DetectPlayer()
    {
        enemyAgent.SetDestination(player.position);

        transform.LookAt(player);
    }

    void AttackPlayer()
    {
        enemyAgent.SetDestination(transform.position);
        transform.LookAt(player);

        if(isAttacking == false)
        {
            Rigidbody rb = Instantiate(projectile, attackPoint.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * projectileForce, ForceMode.Impulse);

            isAttacking = true;
            Invoke("ResetAttack",attackDelay);
        }
    }

    void ResetAttack()
    {
        isAttacking = false;
    }
    public void EnemyTakeDamage(int DamageAmount)
    {
        EnemyHealth -= DamageAmount;

        if (EnemyHealth < 0)
        {
            EnemyDeath();

        }
    }
    void EnemyDeath()
    {
        Destroy(gameObject);
        gameManager = FindObjectOfType<GameManager>();
        gameManager.AddKill();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,attackRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere (transform.position,sightRange);
    }


}
