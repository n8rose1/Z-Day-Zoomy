using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectableCharacter : MonoBehaviour
{
    public static readonly float MAX_SPEED = 5f;
    public static readonly int MAX_HEALTH = 115;

    [SerializeField] private Material[] materials;
    [SerializeField] private Renderer rend;
    [SerializeField] private int ID;
    [SerializeField] private string characterName;
    [SerializeField] private RectTransform parent;
    [SerializeField] private float expandSpeed = 10f;
    [SerializeField] private int rarity;
    [SerializeField] private Button purchaseButton;
    public float speed;
    public int health;

    private bool hasCharacter;
    private bool isSelected;
    private float origScale;
    private float expandScale;

    void Start()
    {
        origScale = gameObject.transform.localScale.x;
        expandScale = origScale * 2;
    }

    private void Update() {
        hasCharacter = PlayerPrefs.GetInt(PlayerPreferences.hasCharacter + ID, 0) == NewCharacters.COLLECTED;
        if (hasCharacter) {
            rend.material = materials[0];
            if (purchaseButton != null) {
                purchaseButton.gameObject.SetActive(false);
            }
        } else {
            rend.material = materials[1];
            if (purchaseButton != null) {
                purchaseButton.gameObject.SetActive(true);
            }
        }
        if (isSelected) {
            Expand();
        } else {
            Shrink();
        }
    }

    private void Expand() {
        float scaleVal = Mathf.Lerp(gameObject.transform.localScale.x, expandScale, Time.deltaTime * expandSpeed);
        gameObject.transform.localScale = new Vector3(scaleVal, scaleVal, scaleVal);
    }

    private void Shrink() {
        float scaleVal = Mathf.Lerp(gameObject.transform.localScale.x, origScale, Time.deltaTime * expandSpeed);
        gameObject.transform.localScale = new Vector3(scaleVal, scaleVal, scaleVal);
    }

    public void SetSelect(bool select) {
        isSelected = select;
    }

    public string GetName() {
        if (PlayerPrefs.GetInt(PlayerPreferences.hasCharacter + ID, 0) == NewCharacters.COLLECTED) {
            return characterName;
        } else {
            return "???";
        }
    }

    public int GetID() {
        return ID;
    }

    public int GetRarity() {
        return rarity;
    }

    public bool HasCharacter() {
        return PlayerPrefs.GetInt(PlayerPreferences.hasCharacter + ID, 0) == NewCharacters.COLLECTED;
    }

    public RectTransform GetParent() {
        return parent;
    }

    public override string ToString() {
        return gameObject.name;
    }
}
