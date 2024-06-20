using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoneySpawner : MonoBehaviour
{
    [SerializeField] private Vector3[] spawnPoints;
    [SerializeField] private GameObject money;
    [SerializeField] private GameObject placeholder;
    [SerializeField] private float timeBetweenChecks = 10f;
    [SerializeField] private int maxSpawnTime = 40;
    [SerializeField] private int minSpawnTime = 20;

    private GameObject[] spawnedMoney;

    void Start()
    {
        spawnedMoney = new GameObject[spawnPoints.Length];
        RandomStartSpawn();
        StartCoroutine(CheckSpawnPoints());
    }

    private void RandomStartSpawn() {
        for (int i = 0; i < spawnPoints.Length; i++) {
            int spawn = Random.Range(0, 2);
            if (spawn == 0) {
                spawnedMoney[i] = Instantiate(money, spawnPoints[i], money.transform.rotation);

            } else {
                spawnedMoney[i] = null;
            }
        }
    }

    private IEnumerator CheckSpawnPoints() {
        while (!GameController.gameOver) {
            for (int i = 0; i < spawnedMoney.Length; i++) {
                if (spawnedMoney[i] == null) {
                    StartCoroutine(SpawnAtSpawnPoint(i));
                    spawnedMoney[i] = placeholder;
                }
            }
            yield return new WaitForSeconds(timeBetweenChecks);
        }
    }

    private IEnumerator SpawnAtSpawnPoint(int spawnPoint) {
        int spawnTime = Random.Range(minSpawnTime, maxSpawnTime);
        yield return new WaitForSeconds(spawnTime);
        if (!GameController.gameOver) {
            spawnedMoney[spawnPoint] = Instantiate(money, spawnPoints[spawnPoint],
                money.transform.rotation);
        }
    }
}
