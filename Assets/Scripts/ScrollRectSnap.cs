using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScrollRectSnap : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform contentPanel;
    [SerializeField] private Camera uiCamera;
    [SerializeField] private float snapVeloctiy;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text healthStatNum;
    [SerializeField] private TMP_Text speedStatNum;
    [SerializeField] private Button playButton;
    [SerializeField] private GameObject stats;
    [SerializeField] private Image healthStat;
    [SerializeField] private Image speedStat;

    private SelectableCharacter characterToPlay;
    private SelectableCharacter[] characters;

    void Start()
    {
        characters = GetComponentsInChildren<SelectableCharacter>();
        scrollRect.horizontalNormalizedPosition = 0;
        characterToPlay = characters[1];
    }

    void Update()
    {
        FindClosest();
    }

    public void UpdateScreen() {
        float smallestDistance = float.MaxValue;
        float prevDistance = float.MaxValue;
        SelectableCharacter closestCharacter = characters[0];

        foreach (SelectableCharacter character in characters) {
            float distanceToCenter = Mathf.Abs(0.5f -
                (uiCamera.WorldToScreenPoint(character.transform.position).x / uiCamera.pixelWidth));
            if (distanceToCenter > prevDistance) {
                break;
            }
            if (distanceToCenter < smallestDistance) {
                smallestDistance = distanceToCenter;
                closestCharacter = character;
            }
            prevDistance = distanceToCenter;
        }

        Debug.Log(closestCharacter.GetID());
        Debug.Log("Has Character? " + closestCharacter.HasCharacter());
        characterToPlay.SetSelect(false);
        closestCharacter.SetSelect(true);
        nameText.text = closestCharacter.GetName();
        characterToPlay = closestCharacter;
        AdjustStats(closestCharacter);
        playButton.gameObject.SetActive(closestCharacter.HasCharacter());
    }

    private void FindClosest() {
        float smallestDistance = float.MaxValue;
        float prevDistance = float.MaxValue;
        SelectableCharacter closestCharacter = characters[0];

        foreach (SelectableCharacter character in characters) {
            float distanceToCenter = Mathf.Abs(0.5f -
                (uiCamera.WorldToScreenPoint(character.transform.position).x / uiCamera.pixelWidth));
            if (distanceToCenter > prevDistance) {
                break;
            }
            if (distanceToCenter < smallestDistance) {
                smallestDistance = distanceToCenter;
                closestCharacter = character;
            }
            prevDistance = distanceToCenter;
        }
        if (characterToPlay != closestCharacter) {
            FindObjectOfType<AudioManager>().Play("scrollClick");
            characterToPlay.SetSelect(false);
            closestCharacter.SetSelect(true);
            nameText.text = closestCharacter.GetName();
            characterToPlay = closestCharacter;
            AdjustStats(closestCharacter);
            playButton.gameObject.SetActive(closestCharacter.HasCharacter());
        }
        
    }

    private void AdjustStats(SelectableCharacter character) {
        if (character.HasCharacter()) {
            stats.SetActive(true);
            healthStat.fillAmount = (float)character.health / SelectableCharacter.MAX_HEALTH;
            healthStatNum.text = character.health.ToString();
            speedStat.fillAmount = character.speed / SelectableCharacter.MAX_SPEED;
            speedStatNum.text = (character.speed * 10).ToString();
        } else {
            stats.SetActive(false);
        }
    }

    public void PlayGame() {
        PlayerSpawn.playerToSpawnID = characterToPlay.GetID();
        PlayerPrefs.SetInt(PlayerPreferences.currCharacterID, characterToPlay.GetID());
        SceneController.PlayGameFromScript();
    }
}
