using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (FieldOfView))]
public class FieldOfViewEditor : Editor
{
  void OnSceneGUI() {
      FieldOfView fow = (FieldOfView) target;
      Handles.color = Color.white;
      Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.viewRadius);
    
      Vector3 viewAngleA = fow.DirFromAngle (-fow.viewAngle/2 , false);
      Vector3 viewAngleB = fow.DirFromAngle (fow.viewAngle /2, false);

      Handles.DrawLine (fow.transform.position, fow.transform.position + viewAngleA * fow.viewRadius);
      Handles.DrawLine (fow.transform.position, fow.transform.position + viewAngleB * fow.viewRadius);

      
    
      List<FieldOfView.ViewCastInfo> viewPoints = fow.DrawFieldOfView();
      foreach (FieldOfView.ViewCastInfo point in viewPoints){
          if(point.hit == true){
              Handles.color = Color.red;
              Handles.DrawSolidDisc(point.point,(new Vector3(0,1,0)),point.radius);
          }else{
              Handles.color = Color.white;
              Handles.DrawSolidDisc(point.point,(new Vector3(0,1,0)),point.radius);
          }
          
      }

      ;
  }
}
