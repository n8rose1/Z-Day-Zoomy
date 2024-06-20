using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialArrow : MonoBehaviour
{
    [SerializeField] private GameObject pairedPart;

    private void Update() {
        if (pairedPart == null) {
            gameObject.SetActive(false);
            this.enabled = false;
        }
    }
}
