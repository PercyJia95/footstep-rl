using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;


public class FootstepAgent_mine : Agent
{

    public GameObject leftFoot, rightFoot, leftHand, rightHand, centerOfMass;
    public float d;  //d is the distance of foot away from the center of the parabola
    public float k;  //ki is the distance of hands away from main body
    public enum Side
    {
        Left = 0,
        Right = 1
    }

    public struct Action{
        public float phi_change;       //orientation of parabola in degree angle
        public float speed;     //desired initial speed of the center of mass | a magnitude, it is not a Vector3
        public float time;      //desired time duration of the step
        public Action(float _phi_change, float _speed, float _time){
            phi_change = _phi_change;
            speed = _speed;
            time = _time;
        }
    }
    public struct State{
        public Vector3 position;    //position of center of mass at the end of the parabola, end of the step
        public Vector3 velocity;    //velocity of center of mass at the end of the parabola, a normalized vector
        public Vector3 location;    //location of the foot
        public Vector3 orientation;  //orientation of current foot
        public Side side;
        
        public State(Vector3 _position, Vector3 _velocity, Vector3 _location, Vector3 _orientation, Side _side){
            position = _position;
            velocity = _velocity;
            location = _location;
            orientation = _orientation;
            side = _side;
        }
    }

    private bool firstTimeStarting = true;
    private Vector3 startingPosition_body;
    private Vector3 startingPosition_leftFoot;
    private Vector3 startingPosition_rightFoot;
    private Vector3 startingPosition_leftHand;
    private Vector3 startingPosition_rightHand;
    private State nextState;


    public override void OnEpisodeBegin() //— Called at the beginning of an Agent's episode, including at the beginning of the simulation.
    {

        if (firstTimeStarting == true){
            startingPosition_body = centerOfMass.transform.localPosition;
            startingPosition_leftFoot = leftFoot.transform.localPosition;
            startingPosition_rightFoot = rightFoot.transform.localPosition;
            startingPosition_leftHand = leftHand.transform.localPosition;
            startingPosition_rightHand = rightHand.transform.localPosition;
            firstTimeStarting = false;
        }
        nextState = new State(startingPosition_body + new Vector3(0,0,1), (new Vector3 (-1f,0,1f)).normalized, startingPosition_rightFoot, Vector3.zero, Side.Right);
        storedState_1 = nextState;
        storedState_2 = nextState;
        storedState_3 = nextState;
        centerOfMass.transform.localPosition = startingPosition_body;
        leftFoot.transform.localPosition = startingPosition_leftFoot;
        rightFoot.transform.localPosition = startingPosition_rightFoot;
        leftHand.transform.localPosition = startingPosition_leftHand;
        rightHand.transform.localPosition = startingPosition_rightHand;
    }


    [SerializeField] private Transform targetTransform;
    public float viewRadius = 5f;
    [Range(0,360)] public float viewAngle = 180f;
    public float matrixResolution = 6f;
    public override void CollectObservations(VectorSensor sensor) //— Called every step that the Agent requests a decision. This is one possible way for collecting the Agent's observations of the environment; see Generating Observations below for more options.
    {
        // sensor.AddObservation(transform.localPosition);
        // sensor.AddObservation(targetTransform.localPosition);
        sensor.AddObservation(storedState_1.velocity);
        sensor.AddObservation(storedState_2.velocity);
        sensor.AddObservation(storedState_3.velocity);
        sensor.AddObservation(storedAction_1.phi_change);
        sensor.AddObservation(storedAction_1.speed);
        sensor.AddObservation(storedAction_1.time);
        sensor.AddObservation(storedAction_2.phi_change);
        sensor.AddObservation(storedAction_2.speed);
        sensor.AddObservation(storedAction_2.time);

        // Sampling in the Feild of View area
        // Debug.Log(DrawFieldOfView()[0].point);

        List<ViewCastInfo> viewPoints = DrawFieldOfView();
        foreach (ViewCastInfo point in viewPoints){   //7*7*4 = 196
            if(point.hit == true){
                sensor.AddObservation(1f);
                sensor.AddObservation(point.point);
            }else{
                sensor.AddObservation(0f);
                sensor.AddObservation(point.point);
            }
        }
    } 

    public float max_speed_of_agent = 1;
    public float max_time_per_step = 1;
    State storedState_1;
    State storedState_2;
    State storedState_3;
    Action storedAction_1;
    Action storedAction_2;
    private bool actionUpdated = false;
    private bool second_step = false;
    public override void OnActionReceived(ActionBuffers actions) //— Called every time the Agent receives an action to take. Receives the action chosen by the Agent. It is also common to assign a reward in this method.
    {
        
        // float moveX = actions.ContinuousActions[0];
        // float moveZ = actions.ContinuousActions[1];
        // float moveSpeed = 70f;
        // transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;

        float phi_change_1 = 45 + 40 * Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);      // to walk in straight, phi_change = 45

        float speed_1 = 0.559015f + 0.01f +  max_speed_of_agent * Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);     // to walk in straight, speed = 2.23606

        float time_1 = 0.5f + 0.01f + max_time_per_step * Mathf.Clamp(actions.ContinuousActions[2], -1f, 1f);  // to walk in straight, time = 2

        Action a = new Action(phi_change_1, speed_1, time_1);
        storedAction_1 = a;

        storedState_1 = nextState;
        nextState = createFootstep(nextState, a);

    
         //plan a second step
        float phi_change_2 = 45 + 15 * Mathf.Clamp(actions.ContinuousActions[3], -1f, 1f);      // to walk in straight, phi_change = 45
        float speed_2 = 0.559015f + 0.01f + max_speed_of_agent * Mathf.Clamp(actions.ContinuousActions[4], -1f, 1f);     // to walk in straight, speed = 2.23606
        float time_2 = 0.5f + 0.01f + max_time_per_step * Mathf.Clamp(actions.ContinuousActions[5], -1f, 1f);  // to walk in straight, time = 2

        a = new Action(phi_change_2, speed_2, time_2);
        storedAction_2 = a;

        storedState_2 = nextState;
        nextState = createFootstep(nextState, a);

        storedState_3 = nextState;
        second_step = false;


        // Vector3 startingVelocity = new Vector3 (- nextState.velocity.x, 0, nextState.velocity.z);
        // //enforce G1 piecewise
        // float test_phi = storedAction_2.phi_change * Mathf.Deg2Rad;
        // Vector3 parabola_orientation = new Vector3(Mathf.Sin(test_phi), 0, Mathf.Cos(test_phi));
        // float testAngle1 = Mathf.Abs(Vector3.Angle(storedState_2.velocity.normalized, parabola_orientation));
        // float testAngle2 = Mathf.Abs(Vector3.Angle(startingVelocity.normalized, parabola_orientation)); 
        // float testAngle = Mathf.Abs(testAngle1 - testAngle2);
        // Debug.Log(testAngle);


        // if(testAngle > 8){
            // AddReward(-10f);
            // EndEpisode();
        // }

        // if(testAngle > 10){
        //     AddReward(-10f);
            
        // }else if(testAngle > 30){
        //     AddReward(-20f);
        //     EndEpisode();
        // }else if(testAngle > 40){
        //     AddReward(-40f);
        //     EndEpisode();
        // }

        float distance = Vector3.Distance(centerOfMass.transform.localPosition, targetTransform.localPosition);
        // AddReward(Mathf.Exp(-distance));


        actionUpdated = true;

        float c_x = centerOfMass.transform.localPosition.z;
        float c_z = centerOfMass.transform.localPosition.z;
        if(c_x > 15 || c_z > 15 || c_x<-15 || c_z<-15){
            AddReward(-1f);
            EndEpisode();
            return;
        }
    }

    int heuristic_count = 1;
    public override void Heuristic(in ActionBuffers actionsOut){
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        // if(heuristic_count%2 == 0){
        //     continuousActions[0] = 1f;
        //     heuristic_count++;
        // }else{
        //     continuousActions[0] = -1f;
        //     heuristic_count++;
        // }

        continuousActions[0] = 0f;
        continuousActions[1] = Input.GetAxisRaw("Vertical");
        continuousActions[2] = 0.2f;
    }


    private bool first_time_update = true;
    private Quaternion original_rotation;
    private float time_count = 0.0f;
    private float time_between_two_steps = 1f;
    void Update()
    {

        if(second_step == false) nextState = storedState_2;
        else if(second_step == true) nextState = storedState_3;

        // centerOfMass.transform.localPosition = nextState.position;

        Vector3 body_moving_direction = nextState.position - centerOfMass.transform.localPosition;
        if((centerOfMass.transform.localPosition - nextState.position).magnitude > 0.1){
            centerOfMass.transform.localPosition += body_moving_direction.normalized * Time.deltaTime * 10 ;
        }

        
        if(first_time_update) {original_rotation = centerOfMass.transform.rotation; first_time_update = false;}
        // if(actionUpdated) {time_count = 0; actionUpdated = false;}
        Quaternion rotation_between = Quaternion.identity;
        if (second_step == false)
        rotation_between = Quaternion.FromToRotation(storedState_2.velocity, nextState.velocity);
        if (second_step != false)
        rotation_between = Quaternion.FromToRotation(storedState_2.velocity, nextState.velocity);

        centerOfMass.transform.rotation *= rotation_between;
        // Quaternion target_rotation = original_rotation * rotation_between;

        // centerOfMass.transform.rotation = Quaternion.Slerp(original_rotation, target_rotation, time_count);
        // time_count += Time.deltaTime ;

        // if(time_count > 0.5f) time_count = 0.5f;


        Vector3 body_orientation = nextState.velocity;
        float x = nextState.location.x;
        float z = -(x) * body_orientation.x / body_orientation.z;
        Vector3 ortho_body_orientation = (new Vector3(x, 0, z)).normalized;
        if(nextState.side == Side.Left) {
            leftHand.transform.localPosition = centerOfMass.transform.localPosition + k * ortho_body_orientation;
            rightHand.transform.localPosition = centerOfMass.transform.localPosition - k * ortho_body_orientation;
        }else{
            leftHand.transform.localPosition = centerOfMass.transform.localPosition - k * ortho_body_orientation;
            rightHand.transform.localPosition = centerOfMass.transform.localPosition + k * ortho_body_orientation;
        }
        
        if(nextState.side == Side.Left){
            leftFoot.transform.localPosition =  nextState.location;
        }else{
            rightFoot.transform.localPosition = nextState.location;
        }

        if(second_step == false && time_count < time_between_two_steps) 
            time_count += Time.deltaTime;
            return;
        

        second_step = true;
        time_count = 0;
    }

    private IEnumerator Wait(float seconds)
    {
        yield return new WaitForSeconds( seconds );
    }



    private void OnTriggerEnter(Collider other){
        if (other.TryGetComponent<Goal>(out Goal goal)){
            Debug.Log("Reset: Reached Goal");
            // AddReward(+100f);
            EndEpisode();
        }

        if (other.TryGetComponent<Wall>(out Wall wall)){
            Debug.Log("Reset: Collision with Wall");
            AddReward(-10f);
            EndEpisode();
        }

        // if(other.CompareTag("Wall")){
        //     AddReward(-1f);
        //     EndEpisode();
        // };
    }



    public State createFootstep(State s, Action a){
        //To get alpha, you have to compute the orientation of next parabola
        //To get stride length, we have the desired time and velocity to compute it

        // Debug.Log(s.position.ToString() + s.velocity + s.location + s.orientation + s.side);

        Vector3 position_start = s.position;
        float phi_change = a.phi_change; 

        float angle = Vector3.Angle(s.velocity, Vector3.forward);
        if (s.velocity.x >= 0){
            angle = angle;
        }else{
            angle = -angle;
        }


        float phi;
        if(s.side == Side.Right){  // next foot is left foot
            phi = angle + phi_change; 

        }else{                     // next foot is right foot
            phi = angle - phi_change;
        }

        //*** ??? *** *** ??? *** *** ??? *** *** ??? ***//
        // phi = angle;
        //*** ??? *** *** ??? *** *** ??? *** *** ??? ***//

        // Debug.Log(s.side);
        // Debug.Log("Velociy: " + s.velocity);
        // Debug.Log("Angle: " + angle);
        // Debug.Log("storedAction.phi_change: "+ storedAction.phi_change);
        // Debug.Log("Phi: " + phi);

        phi = phi * Mathf.Deg2Rad;

        Vector3 Vdesired = s.velocity * a.speed;

        Vector3 normalized_Vx0 = new Vector3(Mathf.Sin(phi), 0, Mathf.Cos(phi));
        
        Vector3 Vx0 = Vector3.Project(Vdesired, normalized_Vx0);

        //*** Very Important ***
        Vector3 position_end = position_start + a.time * Vx0;

        float complement_angle = phi - 90 * Mathf.Deg2Rad;
        Vector3 normalized_Vdesired_perpendicular = new Vector3(Mathf.Sin(complement_angle), 0,  Mathf.Cos(complement_angle));

        Vector3 Vdesired_perpendicular = Vector3.Project(Vdesired, normalized_Vdesired_perpendicular);
        float magnitude_Vdesired_perpendicular = Vdesired_perpendicular.magnitude;
         
        //since y' = 2 * alpha * t
        float alpha = magnitude_Vdesired_perpendicular/(2 * a.time);

        Vector3 center_of_parabola = position_start + Vx0 * 1/2 * a.time + Vdesired_perpendicular * alpha * Mathf.Pow(1/2 * a.time, 2);

        //d is the distance of foot away from the center of the parabola
        Vector3 location_of_foot_new = center_of_parabola + Vdesired_perpendicular.normalized * d;

        Vector3 velocity_at_endParabola = (Vx0 + (-Vdesired_perpendicular)).normalized;

        Side new_side;
        if(s.side == Side.Left) new_side = Side.Right; else new_side = Side.Left;

        State new_state = new State(position_end, velocity_at_endParabola, location_of_foot_new, Vector3.zero, new_side);

        return new_state;
    }


    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal){
        if(!angleIsGlobal){
            angleInDegrees += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

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
