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
    public float three_k_break;
    public float final;
    public float high_speed;
    public float field_elevation;

    //max roll when a/c turns
    public int max_roll;

    //-124.45 is Directly North
    public Rigidbody north;
    public Rigidbody rb;
    public float speed;
    public float taxi_speed;
    public bool luaw; //line up and wait
    public float luaw_speed;

    public bool turning = false;
    public bool left_turn = false;
    public bool right_turn = false;
    public bool departure = false;

    public Aircraft ac;
    public Database db;
    public List<object> ac_characteristics;
    // Use this for initialization
    void Start () {
        final = 90; //used for downwind base and break and luaw
        left_downwind = -90;
        three_k_break = -90;
        high_speed = 125;
        field_elevation = 4f;
        max_roll = 35;
        taxi_speed = 2;
        luaw_speed = 2;
        luaw = false;

        rb = gameObject.GetComponent<Rigidbody>();
        north = GameObject.Find("North").GetComponent<Rigidbody>();
        //speed = 10f;
        ac = new Aircraft();
        db = new Database();

        Initialize_Aircraft(rb, ac);

    }

    private void OnDestroy()
    {
        Destroy(gameObject);
    }

    public void Initialize_Aircraft(Rigidbody rb, Aircraft ac)
    {
        //current list of ac characteristics in order
        //type, climb_rate, descent_rate, turn_rate
        //roll_rate, ground_speed, category, weight_class
        //departure point
        ac.call_sign = gameObject.name;
        print(ac.call_sign);
        ac_characteristics = db.get_ac_characteristics(db.get_type(ac.call_sign));
        ac.type = (string)ac_characteristics[0];
        ac.climb_rate = (int)ac_characteristics[1];
        ac.descent_rate = (int)ac_characteristics[2];
        ac.turn_rate = (float)ac_characteristics[3];
        ac.roll_rate = (float)ac_characteristics[4];
        ac.ground_speed = (int)ac_characteristics[5];
        ac.category = (int)ac_characteristics[6];
        ac.weight_class = (string)ac_characteristics[7];
        ac.departure_point = (string)ac_characteristics[8];

        //changes important aircraft mechanics in the following lines
        departure = db.is_departure(ac.call_sign);

        if(departure == true)
        {
            ac.ry = 0f;
            lineup_and_wait(ac);
        }else if(departure == false)
        {
            ac.rx = rb.transform.rotation.x; //0 is level flight
                                             //y rotation is important for aircraft heading on spawn
            if (ac.type == "C172")
                ac.ry = 200;// for direct 
            else if (ac.type == "F16")
                ac.ry = 90f;//initial to overhead
            ac.rz = rb.transform.rotation.z;//0 is wings level
            rb.transform.localEulerAngles = new Vector3(ac.rx, ac.ry, ac.rz);
            ac.py = rb.transform.position.y;//altitude
        }
        
    }

    public void lineup_and_wait(Aircraft ac)
    {
        right_turn = true;
        float degree_turn = get_degree_turn(final);
        rb.constraints = RigidbodyConstraints.FreezePositionY;//ensure a/c doesn't change altitude
        luaw = true;
        //uniform taxi speed could be made a/c specific
        ac.turn_rate = .5f;
        Turn_Controller(ac, degree_turn);

    }


    // Update is called once per frame
    void Update () {
        if(departure == true)
        {
            if(luaw == true)//aircaft_luaw handles turn onto runway
            {
                //uniform taxi speed could be made a/c specific
                rb.AddRelativeForce(Vector3.forward * luaw_speed);
                rb.velocity = Vector3.ClampMagnitude(rb.velocity, luaw_speed);
                rb.transform.localEulerAngles = new Vector3(ac.rx, ac.ry, ac.rz);
            }else if(luaw == false && ac.ry > 0)
            {
                rb.constraints = RigidbodyConstraints.FreezePosition;
            }
        }
        else
        {
            ac.py = rb.transform.position.y;
            Move();
        }
    }

    public void Turn_Controller(Aircraft ac, float degree_turn)
    {
        if (luaw == true)
        {
            System.Threading.Thread mThread = new System.Threading.Thread(() => aircraft_luaw(ac, degree_turn));
            mThread.Start();
        }
        else
        {
            System.Threading.Thread mThread = new System.Threading.Thread(() => Turn(ac, degree_turn));
            mThread.Start();
        }
        
    }
    
    public void aircraft_luaw(Aircraft ac, float degree_turn)
    {
        float target = 0;
        while (target < degree_turn)
        {
            System.Threading.Thread.Sleep(80);
            ac.ry += ac.turn_rate;
            target += ac.turn_rate;
        }
        luaw = false;
        right_turn = false;
        turning = false;
        ac.turn_rate = 1f;//turn_rate returned to initial value
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
            turning = false;
            ac.turn_rate = 1f; //turn_rate returned to initial value
            
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
            turning = false;
            ac.turn_rate = 1f;//turn_rate returned to initial value
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
            if(ac.py < altitude + 8)
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
        bool past_mid_turn = false;

        if(left_turn == true)
        {
            float init_heading = ac.ry;
            int mid_turn = (int)init_heading - ((int)degree_turn / 2);
            
            while (turning == true)
            {
                //aircraft rolling
                if (ac.rz < max_roll && past_mid_turn == false)
                {                    
                    System.Threading.Thread.Sleep(80);
                    ac.rz += ac.roll_rate;
                    
                    if (ac.ry <= mid_turn)
                    {                        
                        past_mid_turn = true;
                    }
                        
                }
                else if (ac.rz > 0 && past_mid_turn == true) //aircraft is unrolling
                {                    
                    System.Threading.Thread.Sleep(80);
                    ac.rz -= ac.roll_rate;
                    if (ac.rz <= 0)
                    {
                        ac.rz = 0;
                        break;
                    }
                }else if(ac.rz >= max_roll && ac.ry <= mid_turn)
                {
                    past_mid_turn = true;
                }
            }
            ac.rz = 0;
            ac.roll_rate = .35f;//reset roll rate
            
        }
        else if(right_turn == true)
        {
            float init_heading = ac.ry;
            int mid_turn = ((int)degree_turn / 2) + (int)init_heading;
            
            //use abs value of ac.rz, negative values for right turns on z axis
            while (turning == true)
            {
                //aircraft is rolling
                if ((ac.rz*-1) < max_roll && past_mid_turn == false)
                {
                    
                    System.Threading.Thread.Sleep(80);
                    ac.rz -= ac.roll_rate;
                    
                    if (ac.ry >= mid_turn) {
                        past_mid_turn = true;
                    }

                }else if((ac.rz*-1) > 0 && past_mid_turn == true) //aircraft is unrolling
                {

                    ac.rz += ac.roll_rate;
                    System.Threading.Thread.Sleep(80);

                    if((ac.rz*-1) <= 0)
                    {
                        ac.rz = 0;
                        break;
                    }
                }else if ((ac.rz*-1) >= max_roll && ac.ry >= mid_turn)
                {
                    past_mid_turn = true;
                }
            }
            ac.rz = 0;
            ac.roll_rate = .35f;//reset roll rate
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
            print("target: " + target);
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
        if(departure == false)
        {
            rb.AddRelativeForce(Vector3.forward * ac.ground_speed);
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, ac.ground_speed);
            //rx == pitch, ry == yaw, rz == roll
            rb.transform.localEulerAngles = new Vector3(ac.rx, ac.ry, ac.rz);
        }
    }

    public void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.name == "Downwind")
        {
            right_turn = true;
            turning = true;
            float degree_turn = get_degree_turn(left_downwind);
            Turn_Controller(ac, degree_turn);
            Roll_Controller(ac, degree_turn);
        }
        else if (other.gameObject.name == "Perch")
        {
            left_turn = true;
            turning = true;
            float degree_turn = get_degree_turn(final);
            ac.turn_rate = .625f;//for base turn only
            Turn_Controller(ac, degree_turn);
            Descent_Controller(ac, field_elevation);
            Roll_Controller(ac, degree_turn);
        } else if (other.gameObject.name == "Landing" && departure == false)
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
        }else if(other.gameObject.name == "Three_K_Break")
        {
            right_turn = true;
            turning = true;
            ac.ground_speed = 10;
            ac.turn_rate = 1f;
            ac.roll_rate = 1f;
            float degree_turn = get_degree_turn(three_k_break);
            Turn_Controller(ac, degree_turn);
            Roll_Controller(ac, degree_turn);

        }else if(other.gameObject.name == "Break_Perch")
        {
            right_turn = true;
            turning = true;
            ac.ground_speed = 9;
            ac.turn_rate = .9f;
            ac.roll_rate = 1f;
            ac.descent_rate = 50;
            float degree_turn = get_degree_turn(final);
            Turn_Controller(ac, degree_turn);
            Roll_Controller(ac, degree_turn);
            Descent_Controller(ac, field_elevation);
        }
        else if (other.gameObject.name == "Aircraft_Clear")
        {
            
            Destroy(gameObject);
        }
    }
}
