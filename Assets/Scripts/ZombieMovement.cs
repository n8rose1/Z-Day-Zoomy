using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class ZombieMovement : MonoBehaviour
{
    private PlayerController player;
    private NavMeshAgent agent;
    private Rigidbody rb;
    private CapsuleCollider cc;
    private Animator animator;
    private HealthBar playerHealthBar;
    private ScoreManager scoreManager;
    private bool inRange;
    private bool canAttack= true;

    [SerializeField] private ZombieScriptableObject[] zombieScriptableObjects;

    private ZombieScriptableObject zombieScriptableObject;
    private int levelBeyondStart;
    Vector3 prevPos;

    /*
    [SerializeField] private int damage;
    [SerializeField] private int secondsBetweenAttacks;
    [SerializeField] private int[] speedRange = { 1, 2 };
    [SerializeField] private int pointsForDeath; */

    private void Start() {
        zombieScriptableObject = zombieScriptableObjects[0];
        player = FindObjectOfType<PlayerController>();
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CapsuleCollider>();
        animator = GetComponent<Animator>();
        playerHealthBar = FindObjectOfType<HealthBar>();
        agent.enabled = false;
        StartCoroutine(Birth());
        levelBeyondStart = SceneManager.GetActiveScene().buildIndex - SceneController.START_MAP;
        agent.speed = Random.Range(zombieScriptableObject.speedRange[0],
            zombieScriptableObject.speedRange[1] + levelBeyondStart * zombieScriptableObject.speedIncrementer);
        scoreManager = FindObjectOfType<ScoreManager>();

    }

    private void Update() {
        Vector3 curMove = transform.position - prevPos;
        animator.SetFloat("speed", curMove.magnitude / Time.deltaTime);
        prevPos = transform.position;
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Player")) {
            if (canAttack) {
                inRange = true;
                StartCoroutine(DamagePlayer());
            } else {
                StartCoroutine(WaitToAttack());
            }
        } else if (collision.gameObject.CompareTag("Meteor")) {
            Die();
        }
    }

    private void OnCollisionExit(Collision collision) {
        if (collision.gameObject.CompareTag("Player")) {
            StartCoroutine(AttackBuffer());
            inRange = false;
            animator.SetBool("isAttacking", false);
        }
    }

    
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Bounds")) {
            Fall();
        }
    }

    private IEnumerator AttackBuffer() {
        yield return new WaitForSeconds(1);
        canAttack = true;
    }

    private IEnumerator WaitToAttack() {
        while (!canAttack && inRange) {
            yield return new WaitForFixedUpdate();
        }
        if (inRange) {
            DamagePlayer();
        }
    }

    private IEnumerator DamagePlayer() {
        animator.SetBool("isAttacking", true);
        while (inRange && !GameController.gameOver) {
            player.TakeDamage(zombieScriptableObject.damage);
            playerHealthBar.SetHealth(PlayerController.health);
            canAttack = false;
            yield return new WaitForSeconds(zombieScriptableObject.secondsBetweenAttacks);
            canAttack = true;
        }
    }

    private IEnumerator Birth() {
        agent.enabled = false;
        cc.enabled = false;
        while (animator.GetCurrentAnimatorStateInfo(0).IsName(zombieScriptableObject.birthAnimationName)) {
            yield return new WaitForSeconds(0.05f);
        }
        agent.enabled = true;
        cc.enabled = true;
        StartCoroutine(UpdateTarget());
    }

    private IEnumerator UpdateTarget() {
        while (!GameController.gameOver && agent.enabled == true) {
            if (SurgeController.inSurge && zombieScriptableObject != zombieScriptableObjects[1]) {
                zombieScriptableObject = zombieScriptableObjects[1];
                agent.speed = Random.Range(zombieScriptableObject.speedRange[0],
                    zombieScriptableObject.speedRange[1] + levelBeyondStart * zombieScriptableObject.speedIncrementer);
            } else if (!SurgeController.inSurge && zombieScriptableObject != zombieScriptableObjects[0]) {
                zombieScriptableObject = zombieScriptableObjects[0];
                agent.speed = Random.Range(zombieScriptableObject.speedRange[0],
                    zombieScriptableObject.speedRange[1] + levelBeyondStart * zombieScriptableObject.speedIncrementer);
            }
            rb.velocity = Vector3.zero;
            agent.SetDestination(player.transform.position);
            yield return new WaitForSeconds(zombieScriptableObject.secondsBetweenPlayerSearch);
        }
    }

    public void Die() {
        if (!GameController.gameOver && !PlayerController.fell) {
            ScoreManager.playerScore += zombieScriptableObject.pointsForDeath * ScoreManager.pointsMultiplier;
        }
        if (scoreManager != null) {
            scoreManager.UpdateScoreText();
        } else {
            scoreManager = FindObjectOfType<ScoreManager>();
            scoreManager.UpdateScoreText();
        }
        Destroy(gameObject);
    }

    private void Fall() {
        Vector3 speed = agent.velocity;
        agent.enabled = false;
        cc.isTrigger = true;
        rb.isKinematic = false; 
        rb.velocity = speed;
        rb.useGravity = true;
        StartCoroutine(DieFromFall());
    }

    private IEnumerator DieFromFall() {
        yield return new WaitForSeconds(1f);
        Die();
    }
}
