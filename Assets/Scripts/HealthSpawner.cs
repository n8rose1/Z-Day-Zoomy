using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HealthSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] foods;
    [SerializeField] private float spawnRange;
    [SerializeField] private int amountToSpawn = 15;

    private Vector3 defaultSpawn = new Vector3(5, 0, -5);
    private Vector3 spawnPoint;
    private Vector3 height = new Vector3(0, 0.1f, 0);

    // Start is called before the first frame update
    void Start()
    {
        SpawnHealth();
    }

    private void SpawnHealth() {
        for (int i = 0; i < amountToSpawn; i++) {
            int foodIndex = Random.Range(0, foods.Length);
            Vector2 randomPoint = Random.insideUnitCircle * spawnRange;
            Vector3 randomSpawn = new Vector3(randomPoint.x, 0, randomPoint.y);
            GetRandomPoint(randomSpawn, out spawnPoint);
            _ = Instantiate(foods[foodIndex], spawnPoint + height, Quaternion.Euler(0, Random.Range(0, 360f), 0));
        }
    }

    private void GetRandomPoint(Vector3 randomPoint, out Vector3 result) {
        for (int i = 0; i < 30; i++) {
            Vector3 point = randomPoint;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(point, out hit, 1.0f, NavMesh.AllAreas)) {
                result = hit.position;
                return;
            }
        }
        result = defaultSpawn;
    }
}
