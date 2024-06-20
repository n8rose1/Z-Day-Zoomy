using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieSpawn : MonoBehaviour
{
    [SerializeField] private ZombieMovement[] zombies;
    [SerializeField] private MeteorMovement meteor;
    [SerializeField] private float zombieSpawnRange;
    [SerializeField] private Vector3 zombieDefaultSpawn;

    [SerializeField] private int giantsPointsBenchmark = 30;
    [SerializeField] private int hellhoundsPointsBenchmark = 50;
    [SerializeField] private int meteorPointsBenchmark = 80;

    [SerializeField] private float timeSpawningGrunt;
    [SerializeField] private float timeSpawningGiant;
    [SerializeField] private float timeSpawningHellhound;
    [SerializeField] private float timeSpawningMeteor;
    [SerializeField] private float surgeMultiplier = 3;
    [SerializeField] private float meteorDampener = 2;

    private float surgeEnhancer = 1;

    private Vector3 spawnPoint;
    private Vector3 meteorHeight = new Vector3(0, 40, 0);

    void Start()
    {
        StartCoroutine(SpawnGrunts());
        StartCoroutine(SpawnGiants());
        StartCoroutine(SpawnHellhounds());
        StartCoroutine(SpawnMeteors());
    }

    private void Update() {
        Vector2 randomPoint = Random.insideUnitCircle * zombieSpawnRange;
        Vector3 randomSpawn = new Vector3(randomPoint.x, 0, randomPoint.y);
        GetRandomPoint(randomSpawn, out spawnPoint);
        if (SurgeController.inSurge) {
            surgeEnhancer = surgeMultiplier;
        } else {
            surgeEnhancer = 1;
        }
    }

    private void GetRandomPoint(Vector3 randomPoint, out Vector3 result) {
        for (int i = 0; i < 30; i++) {
            Vector3 point = randomPoint;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(point, out hit, 1, NavMesh.AllAreas)) {
                result = hit.position;
                return;
            }
        }
        result = zombieDefaultSpawn;
    }

    private IEnumerator SpawnGrunts() {
        while (!LoadScene.allTasksCompleted) {
            yield return null;
        }
        while (!GameController.gameOver) {
            yield return new WaitForSeconds(timeSpawningGrunt / surgeEnhancer);
            _ = Instantiate(zombies[0], spawnPoint, zombies[0].transform.rotation * Quaternion.Euler(0, Random.Range(0, 360f), 0));
        }
    }

    private IEnumerator SpawnGiants() {
        while (!LoadScene.allTasksCompleted || ScoreManager.playerScore < giantsPointsBenchmark) {
            yield return null;
        }
        while (!GameController.gameOver) {
            yield return new WaitForSeconds(timeSpawningGiant / surgeEnhancer);
            _ = Instantiate(zombies[1], spawnPoint, zombies[1].transform.rotation * Quaternion.Euler(0, Random.Range(0, 360f), 0));
        }
    }

    private IEnumerator SpawnHellhounds() {
        while (!LoadScene.allTasksCompleted || ScoreManager.playerScore < hellhoundsPointsBenchmark) {
            yield return null;
        }
        while (!GameController.gameOver) {
            yield return new WaitForSeconds(timeSpawningHellhound / surgeEnhancer);
            _ = Instantiate(zombies[2], spawnPoint, zombies[2].transform.rotation * Quaternion.Euler(0, Random.Range(0, 360f), 0));
        }
    }

    private IEnumerator SpawnMeteors() {
        while (!LoadScene.allTasksCompleted || ScoreManager.playerScore < meteorPointsBenchmark) {
            yield return null;
        }
        while (!GameController.gameOver) {
            yield return new WaitForSeconds(timeSpawningMeteor / surgeEnhancer * meteorDampener);
            _ = Instantiate(meteor, spawnPoint + meteorHeight, meteor.transform.rotation);
        }
    }
}
