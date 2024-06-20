using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStarter : MonoBehaviour
{
    public static bool finishedAnimation = false;

    [SerializeField] private CameraMovement mainCamera;
    [SerializeField] private GameObject plane;
    [SerializeField] private GameObject explosion;

    private Vector3 epsilonV = new Vector3(Vector3.kEpsilon, Vector3.kEpsilon, Vector3.kEpsilon);

    void Start()
    {
        StartCoroutine(InitializeStart());
    }

    private IEnumerator InitializeStart() {
        mainCamera.SetAnimator(CameraMovement.CRASH);
        mainCamera.EnableAnimator();
        while (Vector3.Distance(plane.transform.position, epsilonV) > 0.5f) {
            yield return null;
        }
        mainCamera.DisableAnimator();
        FindObjectOfType<AudioManager>().Play("planeExplosion");
        plane.SetActive(false);
        explosion.SetActive(true);
        finishedAnimation = true;
    }
}
