using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    public static int playerToSpawnID = 0;

    [SerializeField] private PlayerController[] characters;

    void Start()
    {
        playerToSpawnID = PlayerPrefs.GetInt(PlayerPreferences.currCharacterID, 0);

        foreach (PlayerController character in characters) {
            if (character.ID == playerToSpawnID) {
                _ = Instantiate(character, new Vector3(0, 0, 0), character.transform.rotation);
                break;
            }
        }
    }
}
