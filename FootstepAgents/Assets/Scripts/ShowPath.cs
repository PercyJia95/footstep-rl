using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShowPath : MonoBehaviour
{
    public Transform target;
    public Transform source;

    private NavMeshPath path;

    void Start()
    {
        path = new NavMeshPath();
    }

    // Update is called once per frame
    void Update()
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(source.position, out hit, 1000, UnityEngine.AI.NavMesh.AllAreas)) {
            if (!NavMesh.CalculatePath(hit.position, target.position, UnityEngine.AI.NavMesh.AllAreas, path)) {
                Debug.Log(path.status);
            }
 
            for (int i = 0; i < path.corners.Length - 1; i++){
                Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.white);
            }
        }else {
            Debug.LogError("Entity is too far from the nav mesh");
        }
    }
}
