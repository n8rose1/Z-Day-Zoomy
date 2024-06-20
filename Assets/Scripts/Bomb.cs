using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Bomb : MonoBehaviour
{
    [SerializeField] private float constructionTime;
    [SerializeField] private int countdownTime;
    [SerializeField] private int blastRadius;
    [SerializeField] private int damage;
    [SerializeField] private GameObject explosion;
    [SerializeField] private Animator animator;
    [SerializeField] private Image progressIndicator;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TMP_Text countdownText;

    private SphereCollider col;
    private HealthBar playerHealthBar;

    private bool isBuilding = false;
    private float elapsed = 0f;

    void Start()
    {
        isBuilding = false;
        col = GetComponent<SphereCollider>();
        playerHealthBar = FindObjectOfType<HealthBar>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            try {
                PlayerController player = other.GetComponent<PlayerController>();
                //TODO: set animation
                isBuilding = true;
                StartCoroutine(Build(player));
            } catch (Exception) {
                Debug.Log("Error in bomb finding player script");
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            isBuilding = false;
        }
    }

    private IEnumerator Build(PlayerController player) {
        while (isBuilding && elapsed < constructionTime) {
            yield return new WaitForEndOfFrame();
            if (!player.IsMoving()) {
                elapsed += Time.deltaTime;
                progressIndicator.fillAmount = elapsed / constructionTime;
            }
        }
        if (elapsed >= constructionTime) {
            col.enabled = false;
            animator.SetBool("Explode", true);
            StartCoroutine(Countdown());
        }
    }

    private IEnumerator Countdown() {
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        audioManager.Play("countdown");
        isBuilding = false;
        backgroundImage.enabled = false;
        progressIndicator.enabled = false;
        countdownText.enabled = true;
        countdownText.text = countdownTime.ToString();
        while (countdownTime > 0) {
            yield return new WaitForSeconds(1);
            audioManager.Play("countdown");
            countdownTime--;
            countdownText.text = countdownTime.ToString();
        }
        Explode();
    }

    private void Explode() {
        FindObjectOfType<AudioManager>().Play("bombExplosion");
        Collider[] hitObjects = Physics.OverlapSphere(transform.position, blastRadius);
        foreach (var currObject in hitObjects) {
            try {
                ZombieMovement currZombie = currObject.GetComponent<ZombieMovement>();
                currZombie.Die();
            } catch (Exception) {
                try {
                    PlayerController player = currObject.GetComponent<PlayerController>();
                    player.TakeDamage(damage);
                    playerHealthBar.SetHealth(PlayerController.health);
                } catch (Exception) {
                    // Do nothing;
                    // If this is reached, means that object was neither zombie nor player
                }
            }
        }
        BombSpawner.numBombs -= 1;
        Destroy(gameObject);
        _ = Instantiate(explosion, transform.position, explosion.transform.rotation);
        Destroy(this);
    }
}
