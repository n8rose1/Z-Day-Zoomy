using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif

public class HomeScreenController : MonoBehaviour
{
    [SerializeField] private GameObject meteor;
    [SerializeField] private GameObject plane;
    [SerializeField] private GameObject planeTrail;
    [SerializeField] private GameObject[] clouds;
    [SerializeField] private SceneController sceneController;
    [SerializeField] private float secondsBeforeHit = 0.35f;
    [SerializeField] private float secondsAfterHit = 2f;

    private void Start() {
#if UNITY_IOS
        if (ATTrackingStatusBinding.GetAuthorizationTrackingStatus() ==
        ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED) {

            ATTrackingStatusBinding.RequestAuthorizationTracking();

        }

#endif
    }

    public void StartGame() {
        SceneController.CheckForTutorial();
        StartCoroutine(CrashPlane());
    }

    private IEnumerator CrashPlane() {
        meteor.SetActive(true);
        yield return new WaitForSeconds(secondsBeforeHit);
        plane.GetComponent<Animator>().SetTrigger("Crash");
        planeTrail.SetActive(true);
        yield return new WaitForSeconds(secondsAfterHit);
        foreach (GameObject cloud in clouds) {
            cloud.SetActive(true);
        }
        yield return new WaitForSeconds(1);
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        audioManager.Stop("Theme");
        audioManager.Play("Game");
        sceneController.PlayGame();
    }
}
