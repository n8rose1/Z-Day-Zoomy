using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Parts { Tail, Wing, Chassis, TailWing, Propeller, None }

public class PlanePart : MonoBehaviour
{
    [SerializeField] private Parts part;

    public Parts GetPart() {
        return part;
    }
}
