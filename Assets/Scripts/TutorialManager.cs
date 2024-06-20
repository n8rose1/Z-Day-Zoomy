using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject[] instructions;
    [SerializeField] private PlayerController player;
    [SerializeField] private PlaneBuilder planeBuilder;
    [SerializeField] private ZombieMovement zombie;
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private Joystick joystick;
    [SerializeField] private Bomb bomb;
    [SerializeField] private GameObject pointArrowUI;
    [SerializeField] private GameObject cockpitArrow;
    [SerializeField] private GameObject[] remainingPartsArrows;

    private int firstTimePlaying;
    private Vector3 cameraOffset;

    void Start()
    {
        firstTimePlaying = PlayerPrefs.GetInt(PlayerPreferences.firstTimePlaying, PlayerPreferences.TRUE);

        if (firstTimePlaying == PlayerPreferences.TRUE) {
            // Set Starting Character to COLLECTED
            PlayerPrefs.SetInt(PlayerPreferences.hasCharacter + 0, NewCharacters.COLLECTED);
            // Set Characters bank account to 99
            PlayerPrefs.SetInt(PlayerPreferences.money, 100);
        }

        foreach (GameObject instruction in instructions) {
            instruction.SetActive(false);
        }

        foreach (GameObject arrow in remainingPartsArrows) {
            arrow.SetActive(false);
        }

        StartCoroutine(WaitForStart());
    }

    private IEnumerator WaitForStart() {
        SpawnPlayer();
        while (!GameStarter.finishedAnimation) {
            yield return null;
        }
        ActivatePlayer();
        StartCoroutine(TeachMovement());
    }

    private void SpawnPlayer() {
        player.gameObject.SetActive(true);
        player.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
    }

    private void ActivatePlayer() {
        player.GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
    }

    private IEnumerator TeachMovement() {
        instructions[0].SetActive(true);
        while (!player.IsMoving()) {
            yield return null;
        }
        // TODO: trigger closing animation for text
        StartCoroutine(CloseScript(0));
        StartCoroutine(TeachCollectParts());
    }

    private IEnumerator TeachCollectParts() {
        // Instruct player to pick up plane parts, starting with the large cockpit
        cockpitArrow.SetActive(true);
        instructions[1].SetActive(true);
        while (player.GetPart() == Parts.None) {
            yield return null;
        }
        cockpitArrow.SetActive(false);
        StartCoroutine(CloseScript(1));

        // Show text to bring part back to center
        instructions[2].gameObject.SetActive(true);
        while (player.GetPart() != Parts.None) {
            yield return null;
        }
        StartCoroutine(CloseScript(2));

        // Instruct player to collect all other parts
        instructions[3].SetActive(true);

        foreach (GameObject arrow in remainingPartsArrows) {
            arrow.SetActive(true);
        }

        // Wait until 3 parts of plane have been built
        while (planeBuilder.GetPartsBuilt() < 3) {
            yield return null;
        }
        StartCoroutine(CloseScript(3));

        StartCoroutine(TeachZombie());
    }

    private IEnumerator TeachZombie() {
        foreach (GameObject arrow in remainingPartsArrows) {
            if (arrow != null) {
                arrow.SetActive(false);
            }
        }
        // Spawn Zombie
        #region
        zombie.gameObject.SetActive(true);
        cameraOffset = mainCamera.GetComponent<CameraMovement>().GetOffset();
        DeactivateCameraFollowingPlayer();
        while (Vector3.Distance(mainCamera.transform.position, zombie.transform.position + cameraOffset) > 0.1f) {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, zombie.transform.position + cameraOffset, 0.125f);
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(0.5f);
        #endregion

        // Instruct player about zombies
        #region
        instructions[4].SetActive(true);
        yield return new WaitForSeconds(1.75f);

        StartCoroutine(CloseScript(4));

        #endregion
        StartCoroutine(TeachBomb());
    }

    private IEnumerator TeachBomb() {
        // Spawn Bomb
        bomb.gameObject.SetActive(true);
        while (Vector3.Distance(mainCamera.transform.position, bomb.transform.position + cameraOffset) > 0.1f) {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, bomb.transform.position + cameraOffset, 0.125f);
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(0.5f);

        // Instruct player about bomb
        instructions[6].SetActive(true);
        yield return new WaitForSeconds(1);
        ReactivateCameraFollowingPlayer();

        while (bomb != null) {
            yield return null;
        }
        StartCoroutine(CloseScript(6));
        StartCoroutine(FinishCollectingParts());
    }

    private IEnumerator FinishCollectingParts() {
        foreach (GameObject arrow in remainingPartsArrows) {
            if (arrow != null) {
                arrow.SetActive(true);
            }
        }
        // Instruct player to pick up remaining parts
        instructions[7].SetActive(true);
        while (planeBuilder.GetPartsBuilt() != PlaneBuilder.TOTAL_PARTS) {
            yield return null;
        }
        StartCoroutine(CloseScript(7));
        player.transform.position = new Vector3(3.4f, 0, 0.5f);

        // Explain Surge
        instructions[5].SetActive(true);
        pointArrowUI.SetActive(true);
        DisableGameUI();
        yield return new WaitForSecondsRealtime(1);
        while (Input.touchCount == 0) {
            yield return null;
        }
        pointArrowUI.SetActive(false);
        StartCoroutine(CloseScript(5));
        EnableGameUI();

        // Instruct player to get in plane
        CapsuleCollider checkForPlayerRendering = player.GetComponent<CapsuleCollider>();
        instructions[8].SetActive(true);

        while (checkForPlayerRendering.enabled == true) {
            yield return null;
        }
        StartCoroutine(CloseScript(8));
    }

    private void DisableGameUI() {
        joystick.SimulateRelease();
        joystick.gameObject.SetActive(false);
    }

    private void EnableGameUI() {
        joystick.gameObject.SetActive(true);
    }

    private void DeactivateCameraFollowingPlayer() {
        DisableGameUI();
        mainCamera.gameObject.GetComponent<CameraMovement>().enabled = false;
    }

    private void ReactivateCameraFollowingPlayer() {
        EnableGameUI();
        mainCamera.gameObject.GetComponent<CameraMovement>().enabled = true;
    }

    private IEnumerator CloseScript(int index) {
        instructions[index].GetComponent<Animator>().SetTrigger("Close");
        yield return new WaitForSeconds(0.5f);
        instructions[index].SetActive(false);
    }

}
