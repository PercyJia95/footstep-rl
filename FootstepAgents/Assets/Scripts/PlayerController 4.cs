using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public NavMeshAgent agent;
    public GameObject goal;

    // Update is called once per frame
    void Update()
    {
        goal = GameObject.Find("Goal");
        agent.SetDestination(goal.transform.position);
    }
}
