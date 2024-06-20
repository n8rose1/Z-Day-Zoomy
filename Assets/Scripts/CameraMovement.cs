using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public static readonly int CRASH = 0;
    public static readonly int TAKEOFF = 1;

    [SerializeField] private RuntimeAnimatorController[] animators;

    [SerializeField] private Vector3 offset = new Vector3(0, 39.445366f, -38.50083f);
    private PlayerController player;

    private Animator animator;

    private void Start() {
        animator = GetComponent<Animator>();
        player = FindObjectOfType<PlayerController>();
        if (player == null) {
            StartCoroutine(SearchForPlayer());
        } else {
            LoadScene.tasksCompleted += 1;
        }
    }

    private void FixedUpdate() {
        if (player != null && !GameController.gameOver && GameStarter.finishedAnimation && !PlayerController.fell) {
            transform.position = Vector3.Lerp(transform.position, player.transform.position + offset, 0.125f);
        }
    }

    public Vector3 GetOffset() {
        return offset;
    }

    public void SetAnimator(int index) {
        animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = animators[index];
    }

    public void DisableAnimator() {
        animator.enabled = false;
    }

    public void EnableAnimator() {
        animator = GetComponent<Animator>();
        animator.enabled = true;
    }

    private IEnumerator SearchForPlayer() {
        while (player == null) {
            player = FindObjectOfType<PlayerController>();
            yield return null;
        }
        LoadScene.tasksCompleted += 1;
    }
}
