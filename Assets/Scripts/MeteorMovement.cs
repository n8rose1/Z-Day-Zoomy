using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorMovement : MonoBehaviour
{
    [SerializeField] private Vector3 startPositionOffset = new Vector3(30, 30, 0);
    [SerializeField] private float speed;
    [SerializeField] private int damage;
    [SerializeField] private GameObject targetImage;

    private PlayerController player;
    private GameObject placedTarget;
    private Vector3 target;
    private Vector3 direction;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        target = new Vector3(player.transform.position.x, 0, player.transform.position.z);
        transform.position = target + startPositionOffset;
        direction = Vector3.Normalize(target - transform.position);
        placedTarget = Instantiate(targetImage, target + new Vector3(0, 0.05f, 0), targetImage.transform.rotation);
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Player")) {
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
        }
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
        if (transform.position.y - target.y < Vector3.kEpsilon) {
            Destroy(this.gameObject);
            Destroy(placedTarget);
        }
    }
}
