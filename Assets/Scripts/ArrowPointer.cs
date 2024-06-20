using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowPointer : MonoBehaviour
{
   [SerializeField] GameObject plane;

    void Update()
    {
        transform.LookAt(plane.transform);
    }
}
