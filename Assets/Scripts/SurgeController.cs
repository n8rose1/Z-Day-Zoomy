using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SurgeController : MonoBehaviour
{

    public static bool inSurge = false;

    [SerializeField] private TMP_Text remainingText;
    [SerializeField] private int countdownTime = 150;
    [SerializeField] private int surgeTime = 60;


    private int remainingCountdown;
    private int remainingSurge;
    
    void Start()
    {
        remainingCountdown = countdownTime;
        remainingSurge = surgeTime;
        inSurge = false;
        StartCoroutine(Countdown());
    }

    private IEnumerator Countdown() {
        while (!GameStarter.finishedAnimation) {
            yield return null;
        }
        while (!GameController.gameOver && !inSurge) {
            remainingText.text = "Surge begins in\n" + (remainingCountdown / 60) + ":" + (remainingCountdown % 60).ToString("D2");
            yield return new WaitForSeconds(1);
            remainingCountdown -= 1;
            if (remainingCountdown < 0) {
                remainingSurge = surgeTime;
                inSurge = true;
                remainingText.text = "SURGE!\n2x Points";
                ScoreManager.pointsMultiplier *= 2;
                StartCoroutine(Surge());
            }
        }
    }

    private IEnumerator Surge() {
        while (!GameController.gameOver && inSurge) {
            yield return new WaitForSeconds(1);
            remainingSurge -= 1;
            if (remainingSurge < 0) {
                remainingCountdown = countdownTime;
                inSurge = false;
                ScoreManager.pointsMultiplier /= 2;
                StartCoroutine(Countdown());
            }
        }
    }
}
