using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float viewRadius;

    [Range(0,360)]
    public float viewAngle;

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal){
        if(!angleIsGlobal){
            angleInDegrees += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public float matrixResolution;

    public List<ViewCastInfo> DrawFieldOfView(){
        int stepCount = Mathf.RoundToInt(/*viewAngle * */ matrixResolution);
        float stepAngleSize = viewAngle / stepCount;
        float stepDistantSize = viewRadius / stepCount;
        List<ViewCastInfo> viewPoints = new List<ViewCastInfo> ();
        for (int i =0; i <= stepCount; i++){
            float angle = transform.eulerAngles.y - viewAngle/2 + stepAngleSize * i;
            for(int j =0; j <= stepCount; j++){
                Vector3 position = transform.position + DirFromAngle(angle,true).normalized * stepDistantSize * j;
                // Debug.DrawLine(transform.position, transform.position + DirFromAngle(angle,true) * viewRadius, Color.red);
                ViewCastInfo newViewCast;
                
                if (Physics.CheckSphere(position, stepDistantSize/3)){
                   newViewCast = new ViewCastInfo(true, position, stepDistantSize * j, stepDistantSize/3);
                }else{
                   newViewCast = new ViewCastInfo(false, position, stepDistantSize * j, stepDistantSize/3);
                }
            
                viewPoints.Add(newViewCast);
            }
        }
        return viewPoints;
    }

    void Update(){
        DrawFieldOfView();
    }

    public struct ViewCastInfo{
        public bool hit;
        public Vector3 point;
        public float dst;
        public float radius;
        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _radius){
            hit = _hit;
            point = _point;
            dst = _dst;
            radius = _radius;
        }
    }

}
