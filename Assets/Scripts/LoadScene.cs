using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadScene : MonoBehaviour
{
    /* TASKS NEEDED TO COMPLETE BEFORE GAME START:
     * Connect player to Joystick
     * Connect player to GameController
     * Connect player to HealthBar
     * Connect player to ScoreManager
     * Connect camera to player
     * Connect power up button to player    (* currently inactive *)
     * Connect attack button to player      (* currently inactive *)
     * connect range indicator to player    (* currently inactive *)
     */

    private static readonly int TASKS_NEEDED = 5;
    public static int tasksCompleted;
    public static bool allTasksCompleted = false;

    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider bar;
    [SerializeField] private GameObject clouds;

    private void Awake() {
        tasksCompleted = 0;
        allTasksCompleted = false;
        bar.maxValue = TASKS_NEEDED;
        bar.value = 0;
    }

    void Start()
    {
        loadingScreen.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        bar.value = tasksCompleted;
        if (tasksCompleted == TASKS_NEEDED) {
            Terminate();
        }
    }

    void Terminate() {
        allTasksCompleted = true;
        loadingScreen.SetActive(false);
        clouds.SetActive(true);
        this.enabled = false;
    }
}
