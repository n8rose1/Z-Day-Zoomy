using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombSpawner : MonoBehaviour
{
    public static int numBombs = 0;

    [SerializeField] private GameObject bomb;
    [SerializeField] private int minSpawnTime;
    [SerializeField] private int maxSpawnTime;
    [SerializeField] private float deltaSpawn;
    [SerializeField] private float maxBombs;

    private void Start() {
        numBombs = 1;
        float randomX = Random.Range(-deltaSpawn, deltaSpawn);
        float randomZ = Random.Range(-deltaSpawn, deltaSpawn);
        _ = Instantiate(bomb, new Vector3(randomX, 0, randomZ), bomb.transform.rotation);
    }

    void Update()
    {
        if (numBombs < maxBombs) {
            StartCoroutine(SpawnBomb());
        }
    }

    private IEnumerator SpawnBomb() {
        numBombs++;
        int waitTime = Random.Range(minSpawnTime, maxSpawnTime);
        yield return new WaitForSeconds(waitTime);
        float randomX = Random.Range(-deltaSpawn, deltaSpawn);
        float randomZ = Random.Range(-deltaSpawn, deltaSpawn);
        _ = Instantiate(bomb, new Vector3(randomX, 0, randomZ), bomb.transform.rotation);
    }
}
