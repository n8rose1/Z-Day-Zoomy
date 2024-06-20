using UnityEngine;


[CreateAssetMenu(fileName = "ZombieScriptableObject", menuName = "ScriptableObjects/Zombie")]
public class ZombieScriptableObject : ScriptableObject
{
    public int damage;
    public string birthAnimationName;
    public float secondsBetweenAttacks;
    public float secondsBetweenPlayerSearch;
    public int pointsForDeath;
    public float[] speedRange = { 1, 2 };
    public float speedIncrementer;
}
