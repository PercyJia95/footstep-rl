using UnityEngine;
using UnityEngine.AI;

public class ShowGoldenPath : MonoBehaviour
{
    
    public Transform target;
    public Transform source;


    private NavMeshPath path;
    private float elapsed = 0.0f;
    void Start()
    {
        path = new NavMeshPath();
        elapsed = 0.0f;
    }

    void Update()
    {
        // Update the way to the goal every second.
        elapsed += Time.deltaTime;
        if (elapsed > 1.0f)
        {
            elapsed -= 1.0f;
            NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path);
            // Debug.Log(NavMesh.AllAreas);
            Debug.Log(path.status);
            
        }
        for (int i = 0; i < path.corners.Length - 1; i++){
            Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
            // Debug.Log("I can print something");
            Debug.Log(path.corners[i].ToString());
            Debug.Log(path.corners[i+1].ToString());
        }


        // NavMeshHit hit;
        // if (NavMesh.SamplePosition(source.position, out hit, 1000, NavMesh.AllAreas)) {
        //     if (!NavMesh.CalculatePath(hit.position, target.position, NavMesh.AllAreas, path)) {
        //         Debug.Log(path.status);
        //     }
 
        //     for (int i = 0; i < path.corners.Length - 1; i++){
        //         Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.green);
        //         // Debug.Log("I can print something");
        //     }
        // }else {
        //     Debug.LogError("Entity is too far from the nav mesh");
        // }
    }
}
