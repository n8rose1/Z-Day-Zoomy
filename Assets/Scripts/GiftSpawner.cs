using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GiftSpawner : MonoBehaviour
{
    public static readonly int COMMON = (int) CharacterRarities.Common;
    public static readonly int RARE = (int) CharacterRarities.Rare;
    public static readonly int EPIC = (int) CharacterRarities.Epic;
    public static readonly int LEGENDARY = (int) CharacterRarities.Legendary;
    public static readonly int GIFT_COST = 100;


    [SerializeField] private GameObject[] gifts;
    [SerializeField] private GameObject[] explosions;
    [SerializeField] private AnimationCurve[] animationCurves;
    [SerializeField] private GameObject[] rarityEyes;
    [SerializeField] private GameObject[] regularEyes;

    [SerializeField] private NewCharacters[] allCharacters;

    [SerializeField] private int commonPerc;
    [SerializeField] private int rarePerc;
    [SerializeField] private int epicPerc;
    [SerializeField] private int legendaryPerc;

    [SerializeField] private int secondsBeforeExploding;
    [SerializeField] private TMP_Text cashText;
    [SerializeField] private TMP_Text characterName;
    [SerializeField] private Button purchaseButton;
    [SerializeField] private Button playButton;
    [SerializeField] private Button homeButton;
    [SerializeField] private float zoomSize = 1.75f;
    [SerializeField] private float zoomY = 10f;
    [SerializeField] private float zoomSpeed = 5f;

    [SerializeField] private List<NewCharacters>[] sortedCharacters =
        { new List<NewCharacters>(), new List<NewCharacters>(), new List<NewCharacters>(), new List<NewCharacters>(), new List<NewCharacters>() };
    private Vector3 spawnLocation;
    private Vector3 targetPos;
    private Vector3 camOrigPos;
    private GameObject currentGift;
    private GameObject explosion;
    private NewCharacters character;
    private bool characterIsNew = false;
    private float origSize;
    private float targetSize;

    private void Start() {
        cashText.text = "$" + string.Format("{0:n0}", PlayerPrefs.GetInt(PlayerPreferences.money, 0));
        spawnLocation = new Vector3(0, gameObject.transform.position.y, 0);
        origSize = Camera.main.orthographicSize;
        camOrigPos = Camera.main.transform.position;
        targetSize = origSize;
        targetPos = camOrigPos;

        SortCharactersByRarity();
    }

    private void SortCharactersByRarity() {
        foreach (NewCharacters character in allCharacters) {
            sortedCharacters[character.GetRarity()].Add(character);
        }
    }

    private void LateUpdate() {
        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, targetSize, zoomSpeed * Time.deltaTime);
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, targetPos, zoomSpeed * Time.deltaTime);
    }

    public void OpenGift() {
        homeButton.gameObject.SetActive(false);
        if (character != null) {
            Destroy(character.gameObject);
        }
        if (explosion != null) {
            Destroy(explosion);
        }
        targetPos = camOrigPos;
        targetSize = origSize;
        playButton.gameObject.SetActive(false);
        characterName.gameObject.SetActive(false);
        int rarity = Random.Range(0, 100);
        PlayerPrefs.SetInt(PlayerPreferences.money, PlayerPrefs.GetInt(PlayerPreferences.money) - GIFT_COST);
        cashText.text = "$" + string.Format("{0:n0}", PlayerPrefs.GetInt(PlayerPreferences.money, 0));

        Debug.Log(rarity);

        if (rarity < legendaryPerc) {
            Debug.Log("Dropping Legendary");
            //currentGift = Instantiate(gifts[LEGENDARY], spawnLocation, gifts[LEGENDARY].transform.rotation);
            int selection = Random.Range(0, sortedCharacters[LEGENDARY].Count);
            character = sortedCharacters[LEGENDARY][selection];
            rarity = LEGENDARY;
        } else if (rarity < legendaryPerc + epicPerc) {
            Debug.Log("Dropping Epic");
            //currentGift = Instantiate(gifts[EPIC], spawnLocation, gifts[EPIC].transform.rotation);
            int selection = Random.Range(0, sortedCharacters[EPIC].Count);
            character = sortedCharacters[EPIC][selection];
            rarity = EPIC;
        } else if (rarity < legendaryPerc + epicPerc + rarePerc) {
            Debug.Log("Dropping Rare");
            //currentGift = Instantiate(gifts[RARE], spawnLocation, gifts[RARE].transform.rotation);
            int selection = Random.Range(0, sortedCharacters[RARE].Count);
            character = sortedCharacters[RARE][selection];
            rarity = RARE;
        } else {
            Debug.Log("Dropping Common");
            //currentGift = Instantiate(gifts[COMMON], spawnLocation, gifts[COMMON].transform.rotation);
            int selection = Random.Range(0, sortedCharacters[COMMON].Count);
            character = sortedCharacters[COMMON][selection];
            rarity = COMMON;
        }

        characterIsNew = NewCharacters.CollectCharacter(character);

        //StartCoroutine(SpinEyes(rarity));

        currentGift = Instantiate(gifts[rarity], spawnLocation, gifts[rarity].transform.rotation);
        StartCoroutine(ExplodeGift(currentGift, rarity));
    }

    public void PlayWithCharacter() {
        PlayerPrefs.SetInt(PlayerPreferences.currCharacterID, character.GetID());
        SceneController.PlayGameFromScript();
    }

    public IEnumerator SpinEyes(int rarity) {
        float randomTime = Random.Range(3, 6);
        int randomTurns = Random.Range(1, 4);
        float maxAngle =  360 + (rarity * 90);
        maxAngle -= 90;
        float timer = 0f;
        Debug.Log(maxAngle);

        int animationCurveNumber = Random.Range(0, animationCurves.Length);

        while (timer < randomTime) {
            float angle = maxAngle * animationCurves[animationCurveNumber].Evaluate(timer / randomTime);
            Debug.Log(angle);
            foreach (GameObject eye in rarityEyes) {
               // eye.transform.eulerAngles = new Vector3(angle - 90, eye.transform.eulerAngles.y, eye.transform.eulerAngles.z);
                eye.transform.eulerAngles = Vector3.Lerp(eye.transform.eulerAngles, new Vector3(angle, eye.transform.eulerAngles.y, eye.transform.eulerAngles.z), 0.5f);
            }
            timer += Time.deltaTime;
            yield return 0;
        }

        foreach (GameObject eye in rarityEyes) {
            eye.transform.eulerAngles = new Vector3(maxAngle, eye.transform.eulerAngles.y, eye.transform.eulerAngles.z);
        }
        Debug.Log(maxAngle % 360);
        yield return new WaitForSeconds(0.5f);

        currentGift = Instantiate(gifts[rarity], spawnLocation, gifts[rarity].transform.rotation);
        StartCoroutine(ExplodeGift(currentGift, rarity));
    }

    private IEnumerator ExplodeGift(GameObject gift, int rarity) {
        purchaseButton.gameObject.SetActive(false);
        yield return new WaitForSeconds(secondsBeforeExploding);
        Destroy(gift);
        explosion = Instantiate(explosions[rarity], spawnLocation, explosions[rarity].gameObject.transform.rotation);
        character = Instantiate(character, spawnLocation, character.gameObject.transform.rotation);
        targetSize = zoomSize;
        targetPos = new Vector3(camOrigPos.x, zoomY, camOrigPos.z);
        playButton.gameObject.SetActive(true);
        if (characterIsNew) {
            characterName.text = character.GetName();
        } else {
            characterName.text = character.GetName() + "\nTRY AGAIN!";
        }
        cashText.text = "$" + string.Format("{0:n0}", PlayerPrefs.GetInt(PlayerPreferences.money, 0));
        characterName.gameObject.SetActive(true);
        if (PlayerPrefs.GetInt(PlayerPreferences.money, 0) >= GIFT_COST) {
            purchaseButton.gameObject.SetActive(true);
        }
        homeButton.gameObject.SetActive(true);
    }
}
