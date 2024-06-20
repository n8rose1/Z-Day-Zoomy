using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Non-changeable base traits for all characters
    public static readonly int MAX_HEALTH = 100;
    public static int health = MAX_HEALTH;

    public static bool fell = false;
    private static bool usedSecondLife = false;

    public int ID;
    [SerializeField] public float speed;
    [SerializeField] private int healthStart;

    private Joystick joystick;
    private HealthBar healthBar;
    private Vector3 movement;
    private Animator animator;
    private Rigidbody rb;
    private CapsuleCollider cc;
    private GameController gameController;
    private ScoreManager scoreManager;
    private Parts currPart = Parts.None;
    private GameObject arrow;


    private void OnEnable() {
        if (GameController.newGame) {
            health = healthStart;
            usedSecondLife = false;
            fell = false;
        }
        GameController.newGame = false;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CapsuleCollider>();
        joystick = FindObjectOfType<Joystick>();
        healthBar = FindObjectOfType<HealthBar>();
        gameController = FindObjectOfType<GameController>();
        scoreManager = FindObjectOfType<ScoreManager>();
        arrow = GameObject.FindGameObjectWithTag("Arrow");
        arrow.transform.parent = transform;
        arrow.SetActive(false);
        if (joystick == null) {
            StartCoroutine(FindJoystick());
        } else {
            LoadScene.tasksCompleted += 1;
        }
        if (gameController == null) {
            StartCoroutine(FindGameController());
        } else {
            LoadScene.tasksCompleted += 1;
        }
        if (healthBar == null) {
            StartCoroutine(FindHealthBar());
        } else {
            healthBar.SetMax(healthStart);
            healthBar.SetHealth(health);
            LoadScene.tasksCompleted += 1;
        }
        if (scoreManager == null) {
            StartCoroutine(FindScoreManager());
        } else {
            LoadScene.tasksCompleted += 1;
        }
        StartCoroutine(WaitForStart());
    }

    // Update is called once per frame
    void Update() {
        movement = new Vector3(joystick.Horizontal, 0, joystick.Vertical);
    }

    private void FixedUpdate() {
        MovePlayer(movement);
        ApplyAnimationState();
    }

        private void OnCollisionExit(Collision collision) {
        rb.velocity = new Vector3(0, 0, 0);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Money")) {
            CollectMoney(other.gameObject);
        } else if (other.CompareTag("Bounds")) {
            StartCoroutine(Fall());
        } else if (other.CompareTag("Health")) {
            CollectHealth(other.gameObject);
        } else if (other.CompareTag("PlanePart")) {
            CollectPart(other.gameObject);
        }
    }

    private void MovePlayer(Vector3 movement) {
        transform.LookAt(transform.position + movement);
        transform.Translate(movement * speed * Time.deltaTime, Space.World);
    }

    private void ApplyAnimationState() {
        animator.SetBool("isRunning", joystick.Horizontal != 0.0 || joystick.Vertical != 0.0);
    }

    public bool IsMoving() {
        return joystick.Horizontal != 0.0 || joystick.Vertical != 0.0;
    }

    private void CollectMoney(GameObject money) {
        FindObjectOfType<AudioManager>().Play("collectMoney");
        MoneyTracker.AddMoneyToBank(1);
        ScoreManager.playerScore += 5;
        scoreManager.UpdateScoreText();
        Destroy(money);
    }

    private void CollectHealth(GameObject healthBox) {
        if (health == healthStart) { return; }
        if (health + 10 > healthStart) {
            health = healthStart;
        } else {
            health += 10;
        }
        FindObjectOfType<AudioManager>().Play("healthPickup");
        healthBar.SetHealth(health);
        Destroy(healthBox);
    }

    private void CollectPart(GameObject part) {
        if (GetPart() != Parts.None) { return; }
        PlaneBuilder plane = FindObjectOfType<PlaneBuilder>();
        Parts type = part.GetComponent<PlanePart>().GetPart();
        if (type == Parts.Chassis) {
            currPart = Parts.Chassis;
        } else {
            if (!PlaneBuilder.hasChassis) {
                //TODO: Do not pickup item and inform player to find chassis first
                return;
            } else {
                currPart = type;
            }
        }
        FindObjectOfType<AudioManager>().Play("partPickup");
        arrow.SetActive(true);
        plane.PlayerHasPart();
        Destroy(part);
    }

    public Parts GetPart() {
        return currPart;
    }

    public void RemovePart() {
        arrow.SetActive(false);
        currPart = Parts.None;
    }

    public void TakeDamage(int damage) {
        health -= damage;
        if (health <= 0) {
            GameOver();
        }
    }

    public void StopMoving() {
        movement = new Vector3(0, 0, 0);
    }

    public void GainHealth(int healthToGain) {
        if (health + healthToGain <= healthStart) {
            health += healthToGain;
        } else {
            health = healthStart;
        }
    }

    public void GetSecondLife() {
        fell = false;
        health = healthStart;
        healthBar.SetHealth(health);
        usedSecondLife = true;
        transform.position = new Vector3(5, 0, 5);
        cc.isTrigger = false;
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezePositionY |
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;
    }

    private void GameOver() {
        joystick.SimulateRelease();
        if (usedSecondLife) {
            gameController.GameOver(false);
        } else {
            StartCoroutine(gameController.ShowSecondLifeOption(this));
        }
    }

    private IEnumerator WaitForStart() {
        SkinnedMeshRenderer mr = GetComponentInChildren<SkinnedMeshRenderer>();
        mr.enabled = false;
        cc.enabled = false;
        while (!GameStarter.finishedAnimation) {
            yield return null;
        }
        mr.enabled = true;
        cc.enabled = true;
        healthBar.SetMax(healthStart);
        healthBar.SetHealth(health);

    }

    private IEnumerator Fall() {
        cc.isTrigger = true;
        rb.constraints = RigidbodyConstraints.None;
        rb.useGravity = true;
        yield return new WaitForSeconds(1);
        fell = true;
        GameOver();
    }

    private IEnumerator FindJoystick() {
        while (joystick == null) {
            joystick = FindObjectOfType<Joystick>();
            yield return null;
        }
        LoadScene.tasksCompleted += 1;
    }

    private IEnumerator FindGameController() {
        while (gameController == null) {
            gameController = FindObjectOfType<GameController>();
            yield return null;
        }
        LoadScene.tasksCompleted += 1;
    }

    private IEnumerator FindHealthBar() {
        while (healthBar == null) {
            healthBar = FindObjectOfType<HealthBar>();
            yield return null;
        }
        healthBar.SetMax(healthStart);
        healthBar.SetHealth(health);
        LoadScene.tasksCompleted += 1;
    }

    private IEnumerator FindScoreManager() {
        while (scoreManager == null) {
            scoreManager = FindObjectOfType<ScoreManager>();
            yield return null;
        }
        LoadScene.tasksCompleted += 1;
    }
}
