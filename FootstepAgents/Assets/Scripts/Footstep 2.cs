using UnityEngine;
using UnityEditor;

public class Footstep : MonoBehaviour
{

    public enum Side
    {
        Left = 0,
        Right = 1
    }

    public Vector3 ComPosition { get; set; }
    // public Vector3 ComVelocity { get; set; } = new Vector3(0, 0, 1);

    public Vector3 ComVelocity = new Vector3(-1, 0, 1);

    public Vector3 FootstepPosition { get; set; }
    public Vector3 FootstepOrientation { get; set; }
    public Vector3[] FootstepOrientationInterval { get; set; }
    public Side FootstepSide;

    public Transform pivot;

    // Update is called once per frame
    void Update()
    {


        transform.position = FootstepPosition;

        transform.rotation = Quaternion.Euler(FootstepOrientation);
    }
}
