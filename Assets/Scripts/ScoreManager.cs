using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static int playerScore = 0;
    public static int pointsMultiplier = 1;

    [SerializeField] private int timeBetweenPoints;

    [SerializeField] private TMP_Text currScoreText;
    [SerializeField] private TMP_Text gameMoney;
    [SerializeField] private TMP_Text finalScoreText;
    [SerializeField] private TMP_Text highScoreText;

    void Start() {
        pointsMultiplier = SceneManager.GetActiveScene().buildIndex - SceneController.START_MAP + 1;
        StartCoroutine(UpdateScore());
    }

    private void Update() {
        gameMoney.text = "$" + string.Format("{0:n0}", PlayerPrefs.GetInt(PlayerPreferences.money));
    }

    private IEnumerator UpdateScore() {
        while (!LoadScene.allTasksCompleted) {
            yield return null;
        }
        while (!GameController.gameOver && !PlayerController.fell) {
            currScoreText.text = string.Format("{0:n0}", playerScore);
            if (currScoreText.text.Length > 11) {
                //currScoreText.fontSize = 125 - (25 * (((Mathf.Floor(Mathf.Log10(playerScore) + 1) - (9 + 1)) / 3) + 1));
                currScoreText.fontSize = 100;
            }
            yield return new WaitForSeconds(timeBetweenPoints);
            playerScore += 1;
        }
    }

    public void UpdateScoreText() {
        currScoreText.text = string.Format("{0:n0}", playerScore);
    }

    public void SaveScore() {
        if (playerScore > PlayerPrefs.GetInt(PlayerPreferences.highScore, 0)) {
            PlayerPrefs.SetInt(PlayerPreferences.highScore, playerScore);
        }
        finalScoreText.text = string.Format("{0:n0}", playerScore);
        highScoreText.text = "BEST: " + string.Format("{0:n0}", PlayerPrefs.GetInt(PlayerPreferences.highScore, 0));
    }
}
