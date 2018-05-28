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
    public float left_downwind;
    public float final;
    public float high_speed;
    public float field_elevation;

    //max roll when a/c turns
    public int max_roll;

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
        final = 90;
        left_downwind = -90;
        high_speed = 125;
        field_elevation = 4f;
        max_roll = 10;

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
        ac.descent_rate = 6;
        ac.type = "civilian";
    }


    // Update is called once per frame
    void Update () {
        ac.py = rb.transform.position.y;
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
        float current_descent_rate = ac.rx;

        while(current_descent_rate < ac.descent_rate)
        {
            System.Threading.Thread.Sleep(100);
            current_descent_rate += .05f;
            ac.rx += .05f;
        }

        while(current_descent_rate > 0)
        {
            if(ac.py < 12)
            {
                System.Threading.Thread.Sleep(100);
                current_descent_rate -= .05f;
                ac.rx -= .05f;
            }
        }
        ac.rx = 0;
    }

    public void Roll_Controller(Aircraft ac, float degree_turn)
    {
        System.Threading.Thread mThread = new System.Threading.Thread(() => Roll(ac, degree_turn));
        mThread.Start();
    }

    public void Roll(Aircraft ac, float degree_turn)
    {
        //get midway point of turn
        float init_heading = ac.ry;
        int mid_turn = ((int)degree_turn / 2) + (int)init_heading;
        bool past_mid_turn = false;

        if(left_turn == true)
        {
            while (true)
            {
                if (ac.rz < max_roll && past_mid_turn == false)
                {
                    
                    System.Threading.Thread.Sleep(80);
                    ac.rz += .5f;
                    if (ac.ry >= mid_turn)
                        past_mid_turn = true;
                }
                else if (ac.rz > 0 && past_mid_turn == true)
                {
                    
                    System.Threading.Thread.Sleep(80);
                    ac.rz -= .5f;
                    if (ac.rz <= 0)
                    {
                        ac.rz = 0;
                        break;
                    }
                }
            }
        }
        else if(right_turn == true)
        {
            
            //use abs value of ac.rz, negative values for right turns on z axis
            while (true)
            {
                if ((ac.rz*-1) < max_roll && past_mid_turn == false)
                {
                    
                    System.Threading.Thread.Sleep(80);
                    ac.rz -= .5f;
                    if (ac.ry >= mid_turn) {
                        print("trued");
                        past_mid_turn = true;
                    }

                }else if((ac.rz*-1) > 0 && past_mid_turn == true)
                {
                    print("bring back");
                    System.Threading.Thread.Sleep(80);
                    ac.rz += .5f;
                    if((ac.rz*-1) <= 0)
                    {
                        ac.rz = 0;
                        break;
                    }
                }
            }
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
                degree_turn = target - true_heading;
            }
        }

        return degree_turn;
        
    }

    public void Move()
    {
        rb.AddRelativeForce(Vector3.forward * ac.ground_speed);
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, ac.ground_speed);
        //rx == pitch, ry == yaw, rz == roll
        rb.transform.localEulerAngles = new Vector3(ac.rx, ac.ry, ac.rz);
    }

    public void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.name == "Downwind")
        {
            right_turn = true;
            float degree_turn = get_degree_turn(left_downwind);
            Turn_Controller(ac, degree_turn);
            Roll_Controller(ac, degree_turn);
        }
        else if (other.gameObject.name == "Perch")
        {
            left_turn = true;
            float degree_turn = get_degree_turn(final);
            ac.turn_rate = .625f;//for base turn only
            Turn_Controller(ac, degree_turn);
            Descent_Controller(ac, field_elevation);
            Roll_Controller(ac, degree_turn);
        } else if (other.gameObject.name == "Short_Final")
        {
            // a/c rolling out on runway speed
            ac.ground_speed = 8;
            ac.py = 4f;
            rb.transform.localEulerAngles = new Vector3(ac.rx, ac.ry, ac.rz);
            rb.constraints = RigidbodyConstraints.FreezePositionY;
        } else if (other.gameObject.name == "Enter_High_Speed")
        {
            right_turn = true;
            ac.turn_rate = 1.5f;
            float degree_turn = get_degree_turn(high_speed);
            Turn_Controller(ac, degree_turn);
        } else if (other.gameObject.name == "Aircraft_Clear")
        {
            
            Destroy(gameObject);
        }
    }
}
