using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private static readonly int HOME_SCREEN = 0;
    private static readonly int TUTORIAL = 1;
    private static readonly int PLAYER_SELECT_SCENE = 2;
    private static readonly int OPEN_GIFT_SCENE = 3;
    public static readonly int START_MAP = 4;

    public void GoHome() {
        FindObjectOfType<AudioManager>().Play("buttonBackward");
        SceneManager.LoadScene(HOME_SCREEN);
    }

    public static void CheckForTutorial() {
        int needTutorial = PlayerPrefs.GetInt(PlayerPreferences.firstTimePlaying, PlayerPreferences.TRUE);

        if (needTutorial == PlayerPreferences.TRUE) {
            StartTutorial();
        }
    }

    public static void StartTutorial() {
        SceneManager.LoadScene(TUTORIAL);
    }

    public static void EndTutorial() {
        PlayerPrefs.SetInt(PlayerPreferences.firstTimePlaying, PlayerPreferences.FALSE);
        PlayGameFromScript();
    }

    public void PlayGame() {
        GameStarter.finishedAnimation = false;
        PlaneBuilder.hasChassis = false;
        GameController.newGame = true;
        ScoreManager.playerScore = 0;
        ScoreManager.pointsMultiplier = 1;
        SceneManager.LoadScene(START_MAP);
    }

    public void OpenGift() {
        SceneManager.LoadScene(OPEN_GIFT_SCENE);
    }

    public void SelectPlayer() {
        FindObjectOfType<AudioManager>().Play("buttonForward");
        SceneManager.LoadScene(PLAYER_SELECT_SCENE);
    }

    public static void PlayNextMap() {
        GameStarter.finishedAnimation = false;
        PlaneBuilder.hasChassis = false;
        // Collect Map Character for getting off island
        CollectMapCharacter();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public static void CollectMapCharacter() {
        NewCharacters.CollectCharacter(NewCharacters.MAP_CHARACTERS_START_INDEX + (SceneManager.GetActiveScene().buildIndex - START_MAP));
    }

    public static void PlayGameFromScript() {
        GameStarter.finishedAnimation = false;
        PlaneBuilder.hasChassis = false;
        GameController.newGame = true;
        ScoreManager.playerScore = 0;
        ScoreManager.pointsMultiplier = 1;
        SceneManager.LoadScene(START_MAP);
    }

    public static bool IsFinalLevel() {
        return SceneManager.GetActiveScene().buildIndex == (SceneManager.sceneCountInBuildSettings - 1);
    }

    public static bool IsTutorial() {
        return SceneManager.GetActiveScene().buildIndex == TUTORIAL;
    }
}
