using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flight_test : MonoBehaviour {

    //-124.45 is Directly North
    public Rigidbody north;
    public Rigidbody rb;
    public float speed;
    public bool ijk = false;

    public Aircraft ac;

    // Use this for initialization
    void Start () {
        rb = gameObject.GetComponent<Rigidbody>();
        north = GameObject.Find("North").GetComponent<Rigidbody>();
        speed = 10f;
        ac = new Aircraft();

        print(get_heading());
    }

    public void Initialize_Aircraft(Rigidbody rb, Aircraft ac)
    {
        ac.rx = rb.transform.rotation.x;
        ac.ry = rb.transform.rotation.y;
        ac.rz = rb.transform.rotation.z; 
    }

    // Update is called once per frame
    void Update () {
        Move();
        if(ijk == false)
        {
            Turn_Controller(ac, 90);
        }
    }

    public void Turn_Controller(Aircraft ac, int degree_turn)
    {
        System.Threading.Thread mThread = new System.Threading.Thread(() => Turn(ac, degree_turn));
        mThread.Start();
    }

    public void Turn(Aircraft ac, int degree_turn)
    {
        while (ac.ry < 90){
            ac.ry += .000001f;
        }
    }

    public Vector3 get_heading()
    {
        var heading = north.position - rb.position;

        return heading;
    }

    public void Move()
    {
        rb.AddRelativeForce(Vector3.forward * speed);
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, speed);
        rb.transform.localEulerAngles = new Vector3(ac.rx, ac.ry, ac.rz);
    }
}
