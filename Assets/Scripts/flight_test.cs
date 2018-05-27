using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flight_test : MonoBehaviour {

    //north in game is -124 due to how map was created
    //in future game rotation could be changed to
    //fix but until small calculations are made
    //for determining actual headings and turns
    public float game_circle_start = -124;
    public float game_circle_stop = 235;
    public float left_downwind = -90;
    public float final = 90;
    public float field_elevation = 4f;

    //-124.45 is Directly North
    public Rigidbody north;
    public Rigidbody rb;
    public float speed;
    public bool turning = false;
    public bool left_turn = false;
    public bool right_turn = false;

    public Aircraft ac;

    // Use this for initialization
    void Start () {

        rb = gameObject.GetComponent<Rigidbody>();
        north = GameObject.Find("North").GetComponent<Rigidbody>();
        speed = 10f;
        ac = new Aircraft();

        Initialize_Aircraft(rb, ac);

    }

    public void Initialize_Aircraft(Rigidbody rb, Aircraft ac)
    {
        
        ac.rx = rb.transform.rotation.x; //0 is level flight
        ac.ry = 200f;//200 for direct downwind
        ac.rz = rb.transform.rotation.z;//0 is wings level
        rb.transform.localEulerAngles = new Vector3(ac.rx, ac.ry, ac.rz);
        ac.turn_rate = 1f;
        ac.ground_speed = 10;
        ac.py = rb.transform.position.y;//altitude
        ac.descent_rate = 5;
    }

    // Update is called once per frame
    void Update () {
        Move();
    }

    public void Turn_Controller(Aircraft ac, float degree_turn)
    {
        System.Threading.Thread mThread = new System.Threading.Thread(() => Turn(ac, degree_turn));
        mThread.Start();
    }

    public void Turn(Aircraft ac, float degree_turn)
    {
        float target = 0;
        
        if(left_turn == true)
        {
            while (target < degree_turn)
            {
                System.Threading.Thread.Sleep(80);
                ac.ry -= ac.turn_rate;
                target += ac.turn_rate;
            }
            ac.rz = 0;
            left_turn = false;
            ac.turn_rate = 1f;
            
        }
        else if(right_turn == true)
        {
            while (target < degree_turn)
            {
                System.Threading.Thread.Sleep(80);
                ac.ry += ac.turn_rate;
                target += ac.turn_rate;
            }
            ac.rz = 0;
            right_turn = false;
            ac.turn_rate = 1f;
        }
    }

    public void Descent_Controller(Aircraft ac, float altitude)
    {
        System.Threading.Thread mThread = new System.Threading.Thread(() => Descend(ac, altitude));
        mThread.Start();
    }

    public void Descend(Aircraft ac, float altitude)
    {
        print("enter descend");
        
        while(ac.rx > ac.descent_rate)
        {
            System.Threading.Thread.Sleep(100);
            ac.rx -= .5f;
        }
        
    }

    public Vector3 get_heading()
    {
        var heading = north.position - rb.position;

        return heading;
    }


    //get how many degrees an aircraft will 
    //have to turn for processing during turn
    //operations
    public float get_degree_turn(float target)
    {
        float degree_turn = 0;
        float true_heading = ac.ry + 124;
        float north = game_circle_start += 124;
        north = 360;
        
        if (left_turn == true)
        {
            target += 124;
            if(target > true_heading)
            {
                degree_turn = (north - target) + true_heading;
            }else if(target < true_heading)
            {
                degree_turn = (true_heading - target);
            }
        }
        else if(right_turn == true)
        {
            target += 124;
            if(target < true_heading)
            {
                degree_turn = (north - true_heading) + target;

            }else if(target > true_heading)
            {
                degree_turn = north - true_heading;
            }
        }

        return degree_turn;
        
    }

    public void Move()
    {
        rb.AddRelativeForce(Vector3.forward * ac.ground_speed);
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, ac.ground_speed);
        rb.transform.localEulerAngles = new Vector3(ac.rx, ac.ry, ac.rz);
    }

    public void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.name == "Downwind")
        {
            right_turn = true;
            float degree_turn = get_degree_turn(left_downwind);
            Turn_Controller(ac, degree_turn);
        }
        else if (other.gameObject.name == "Perch")
        {
            left_turn = true;
            float degree_turn = get_degree_turn(final);
            print(degree_turn);
            ac.turn_rate = .6f;
            Turn_Controller(ac, degree_turn);
            Descent_Controller(ac, field_elevation);
        }
    }
}
