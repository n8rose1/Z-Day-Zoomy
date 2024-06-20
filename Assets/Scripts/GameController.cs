using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private static int elapsedTime = 0;

    public static bool gameOver = false;
    public static bool newGame;

    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private AdsManager adsManager;
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject gameScreen;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject secondLifePanel;
    [SerializeField] private Image secondLifeTimer;
    [SerializeField] private GameObject giftButton;
    [SerializeField] private GameObject moneyRemaining;
    [SerializeField] private TMP_Text endStatusText;
    [SerializeField] private TMP_Text moneyRemainingText;
    [SerializeField] private TMP_Text currMoneyText;
    [SerializeField] private GameObject[] clouds;

    private float timerLength = 5f;
    private bool finishedSecondLife;
    private bool closeSecondLife = false;
    private PlayerController player;

    public void Start() {
        StartCoroutine(UpdateElapsedTime());
        secondLifePanel.SetActive(false);
        pauseScreen.SetActive(false);
        gameScreen.SetActive(true);
        gameOverScreen.SetActive(false);
        gameOver = false;
    }

    private void Update() {
        if (!gameOver) { return; }
        if (PlayerPrefs.GetInt(PlayerPreferences.money, 0) >= 100) {
            giftButton.SetActive(true);
            moneyRemaining.SetActive(false);
        } else {
            moneyRemainingText.text = "Need " + "$" + string.Format("{0:n0}", GiftSpawner.GIFT_COST - PlayerPrefs.GetInt(PlayerPreferences.money, 0));
            giftButton.SetActive(false);
            moneyRemaining.SetActive(true);
        }
        currMoneyText.text = "$" + string.Format("{0:n0}", MoneyTracker.GetBalance());
    }

    public void PauseGame() {
        Time.timeScale = 0;
        gameScreen.SetActive(false);
        pauseScreen.SetActive(true);
    }

    public void ResumeGame() {
        pauseScreen.SetActive(false);
        gameScreen.SetActive(true);
        Time.timeScale = 1;
    }

    public void GameOver(bool wonGame) {
        if (wonGame) {
            SceneController.CollectMapCharacter();
            endStatusText.text = "You Survived!";
        } else {
            endStatusText.text = "You Died!";
        }
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        audioManager.Stop("Game");
        adsManager.LoadInterstitialAd();
        adsManager.LoadRewardAd();
        scoreManager.SaveScore();
        if (elapsedTime > 300) {
            elapsedTime = 0;
            adsManager.ShowInterstitialAd();
        } else if (Random.Range(0, 3) == 1) {
            adsManager.ShowInterstitialAd();
        }
        secondLifePanel.SetActive(false);
        pauseScreen.SetActive(false);
        gameScreen.SetActive(false);
        gameOverScreen.SetActive(true);
        CloudOnceServices.instance.SubmitScoreToLeaderboard(PlayerPrefs.GetInt(PlayerPreferences.highScore));
        gameOver = true;
    }

    public void FinishedSecondLifeAd() {
        finishedSecondLife = true;
        secondLifePanel.SetActive(false);
        player.GetSecondLife();
    }

    public void CloseSecondLife() {
        closeSecondLife = true;
    }

    public IEnumerator ShowSecondLifeOption(PlayerController player) {
        adsManager.LoadRewardAd();
        yield return new WaitForSecondsRealtime(0.1f);
        if (!adsManager.AdsAreReady()) {
            GameOver(false);
            yield break;
        }
        secondLifePanel.SetActive(true);
        this.player = player;
        while (timerLength > 0 && !finishedSecondLife && !closeSecondLife) {
            yield return new WaitForEndOfFrame();
            timerLength -= Time.deltaTime;
            secondLifeTimer.fillAmount = timerLength / 5;
        }
        if (!finishedSecondLife) {
            GameOver(false);
        }
    }

    public void Restart() {
        StartCoroutine(RestartGame());
    }

    public IEnumerator RestartGame() {
        foreach (GameObject cloud in clouds) {
            Animator cloudAnimator = cloud.GetComponent<Animator>();
            cloudAnimator.SetTrigger("Close");
        }
        yield return new WaitForSeconds(1.5f);
        secondLifePanel.SetActive(false);
        pauseScreen.SetActive(false);
        gameScreen.SetActive(true);
        gameOverScreen.SetActive(false);
        gameOver = false;
        SceneController.PlayGameFromScript();
    }

    private IEnumerator UpdateElapsedTime() {
        while (this) {
            yield return new WaitForSecondsRealtime(1);
            elapsedTime++;
        }
    }
}
