using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterRarities : int { Common = 0, Rare = 1, Epic = 2, Legendary = 3 }

public class NewCharacters : MonoBehaviour
{
    public static readonly int COLLECTED = 1;
    public static readonly int UNCOLLECTED = 0;

    public static readonly int MAP_CHARACTERS_START_INDEX = 100;

    [SerializeField] private CharacterRarities rarity;
    [SerializeField] private int playerID;
    [SerializeField] private string characterName;

    public int GetRarity() {
        return (int) rarity;
    }

    public int GetID() {
        return playerID;
    }

    public string GetName() {
        return characterName;
    }


    /* This method takes in a NewCharacter object and checks whether or not the 
     * character has already been collected. If it has, this methood returns 
     * false and refunds the player based on the rarity, if it hasn't, 
     * the method saves the new character and returns true */
    public static bool CollectCharacter(NewCharacters character) {
        if (PlayerPrefs.GetInt(PlayerPreferences.hasCharacter + character.GetID(), UNCOLLECTED) == COLLECTED) {
            // refund player
            MoneyTracker.Refund(character.GetRarity());
            return false;
            
        } else {
            // New Character Collected
            PlayerPrefs.SetInt(PlayerPreferences.hasCharacter + character.GetID(), COLLECTED);
            return true;
        }
    }

    public static bool CollectCharacter(int ID) {
        if (PlayerPrefs.GetInt(PlayerPreferences.hasCharacter + ID, UNCOLLECTED) == COLLECTED) {
            return false;
        } else {
            PlayerPrefs.SetInt(PlayerPreferences.hasCharacter + ID, COLLECTED);
            return true;
        }
    }
}
