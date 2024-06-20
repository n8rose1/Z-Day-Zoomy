using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlaneBuilder : MonoBehaviour
{
    public static readonly int CHASSIS = 0;
    public static readonly int PROPELLER = 1;
    public static readonly int TAIL = 2;
    public static readonly int TAIL_WING = 3;
    public static readonly int WING = 4;
    public static readonly int TOTAL_PARTS = 5;

    public static bool hasChassis = false;

    [SerializeField] private int pointsForBuildingPart = 100;
    [SerializeField] private int pointsForTakeoff = 500;
    [SerializeField] private int pointsForWinning = 3000;
    [SerializeField] private int healthGainedForTakeoff = 50;
    [SerializeField] private GameObject[] parts;
    [SerializeField] private TMP_Text partsCollected;
    [SerializeField] private float constructionTime;
    [SerializeField] private float enterTime;
    [SerializeField] private float flightTime = 5f;
    [SerializeField] private GameObject rangeIndicator;
    [SerializeField] private GameObject gameUI;
    [SerializeField] private Image progressIndicator;
    [SerializeField] private Image progressBackground;
    [SerializeField] private Image getInIndicator;
    [SerializeField] private Image getInBackground;
    [SerializeField] private CameraMovement mainCamera;
    [SerializeField] private GameObject[] clouds;
    [SerializeField] private GameController gameController;

    private bool isBuilding = false;
    private bool isEntering = false;
    private float elapsed = 0f;
    private int partsBuilt;
    private Animator animator;
    private ScoreManager scoreManager;

    private void Start() {
        getInBackground.gameObject.SetActive(false);
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player.GetPart() != Parts.None) {
                isBuilding = true;
                StartCoroutine(Build(player));
            } else if (partsBuilt == TOTAL_PARTS) {
                //TODO: Start coroutine for getting in plane
                isEntering = true;
                StartCoroutine(GetIn(player));
            }
        }
    }

    public int GetPartsBuilt() {
        return partsBuilt;
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            isBuilding = false;
            isEntering = false;
        }
    }


    public void PlayerHasPart() {
        progressBackground.gameObject.SetActive(true);
    }

    private IEnumerator Build(PlayerController player) {
        progressIndicator.fillAmount = 0;
        progressIndicator.gameObject.SetActive(true);
        while (isBuilding && elapsed < constructionTime) {
            yield return new WaitForEndOfFrame();
            if (!player.IsMoving()) {
                elapsed += Time.deltaTime;
            }
            progressIndicator.fillAmount = elapsed / constructionTime;
        }
        if (elapsed >= constructionTime) {
            progressIndicator.gameObject.SetActive(false);
            progressBackground.gameObject.SetActive(false);
            switch (player.GetPart()) {
                case Parts.Chassis:
                    parts[CHASSIS].SetActive(true);
                    hasChassis = true;
                    break;
                case Parts.Propeller:
                    parts[PROPELLER].SetActive(true);
                    break;
                case Parts.Tail:
                    parts[TAIL].SetActive(true);
                    break;
                case Parts.TailWing:
                    parts[TAIL_WING].SetActive(true);
                    break;
                case Parts.Wing:
                    parts[WING].SetActive(true);
                    break;
            }
            ScoreManager.playerScore += pointsForBuildingPart * ScoreManager.pointsMultiplier;
            FindObjectOfType<ScoreManager>().UpdateScoreText();
            player.RemovePart();
            elapsed = 0;
            isBuilding = false;
            partsBuilt += 1;
            partsCollected.text = "Parts\n" + partsBuilt + "/5";
            if (partsBuilt == TOTAL_PARTS) {
                getInBackground.gameObject.SetActive(true);
                isEntering = true;
                StartCoroutine(GetIn(player));
            }
        }
    }

    private IEnumerator GetIn(PlayerController player) {
        getInIndicator.fillAmount = 0;
        float enterElapse = 0;
        while (isEntering && enterElapse < enterTime) {
            yield return new WaitForEndOfFrame();
            if (!player.IsMoving()) {
                enterElapse += Time.deltaTime;
            }
            getInIndicator.fillAmount = enterElapse / enterTime;
        }
        getInIndicator.fillAmount = 0;
        if (enterElapse >= enterTime) {
            ScoreManager.playerScore += pointsForTakeoff * ScoreManager.pointsMultiplier;
            FindObjectOfType<ScoreManager>().UpdateScoreText();
            gameUI.SetActive(false);
            rangeIndicator.SetActive(false);
            player.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
            player.GetComponent<CapsuleCollider>().enabled = false;
            mainCamera.GetComponent<CameraMovement>().enabled = false;
            player.GainHealth(healthGainedForTakeoff);
            animator.SetTrigger("Takeoff");
            parts[PROPELLER].GetComponent<Animator>().SetTrigger("Takeoff");
            mainCamera.SetAnimator(CameraMovement.TAKEOFF);
            mainCamera.EnableAnimator();
            yield return new WaitForSeconds(flightTime);

            if (SceneController.IsTutorial()) {
                foreach (GameObject cloud in clouds) {
                    Animator cloudAnimator = cloud.GetComponent<Animator>();
                    cloudAnimator.SetTrigger("Close");
                }
                yield return new WaitForEndOfFrame();
                yield return new WaitForSeconds(1.5f);
                SceneController.EndTutorial();
            } else if (!SceneController.IsFinalLevel()) {
                foreach (GameObject cloud in clouds) {
                    Animator cloudAnimator = cloud.GetComponent<Animator>();
                    cloudAnimator.SetTrigger("Close");
                }
                yield return new WaitForEndOfFrame();
                yield return new WaitForSeconds(1.5f);
                SceneController.PlayNextMap();
            } else {
                ScoreManager.playerScore += pointsForWinning;
                FindObjectOfType<ScoreManager>().UpdateScoreText();
                yield return new WaitForSeconds(1);
                mainCamera.DisableAnimator();
                gameController.GameOver(true);
            }
        }
    }
}
