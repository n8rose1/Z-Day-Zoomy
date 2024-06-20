using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PartSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] parts;
    [SerializeField] private float range;

    private Vector3 partDefaultSpawn = new Vector3(5, 0, -5);
    private Vector3 spawnPoint;

    void Start()
    {
        StartCoroutine(WaitForStart());
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
        result = partDefaultSpawn;
    }

    private IEnumerator WaitForStart() {
        while (!GameStarter.finishedAnimation) {
            yield return null;
        }

        SpawnParts();
    }

    private void SpawnParts() {
        foreach (GameObject part in parts) {
            Vector2 randomPoint = Random.insideUnitCircle * range;
            Vector3 randomSpawn = new Vector3(randomPoint.x, 0, randomPoint.y);
            GetRandomPoint(randomSpawn, out spawnPoint);
            _ = Instantiate(part, new Vector3(spawnPoint.x, part.transform.position.y, spawnPoint.z), part.transform.rotation);
        }
    }

}
